using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class CardManagerScene3 : MonoBehaviourPun, IPunObservable
{
    public GameObject cardContainer; // The container for all cards
    public Sprite cardBack; // The sprite for the back of the card
    public Button shuffleButton; // Button to shuffle the cards again
    public Button doneButton; // Button to finish with the question
    public Text feedbackText; // Text to show feedback prompt
    public Button finishButton; // Button to finish feedback and show randomize button
    public GameObject scorePanel;

    private Animator cardAnimator; // The Animator for the cards
    [SerializeField] private int currentFrontIndex;
    public int currentScore;
    public int targetImage;
    public int targetText;
    public int lastCardIndex = -1; 
    [SerializeField] private Transform topCard;
    [SerializeField] private List<CardData> currentGroup;
    private string currentPickedPlayer;
    private string previousPickedPlayer;
    private string hostName;
    public bool isFirstRound = true;

    public CardDataManagerScene3 cardDataManagerScene3; // Reference to CardDataManager

    private Vector3 originalScale;

    public Sprite detailNone;
    public Sprite detailSelect;
    public Sprite discloseNone;
    public Sprite discloseSelect;
    public Sprite passionNone;
    public Sprite passionSelect;

    public Image detail;
    public Image disclose;
    public Image passion;
    void Start()
    {
        cardAnimator = cardContainer.GetComponent<Animator>(); // Find the Animator component

        shuffleButton.onClick.AddListener(OnShuffleButtonClicked);
        doneButton.onClick.AddListener(OnDoneButtonClicked);
        finishButton.onClick.AddListener(OnFinishButtonClicked);

        if (photonView != null)
        {
            photonView.ObservedComponents = new List<Component> { this };
            photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
        }

        // Initialize the original scale
        if (cardContainer.transform.childCount > 0)
        {
            originalScale = cardContainer.transform.GetChild(0).localScale;
        }

        // Show the card back initially
        ShowCardBack();

        // Hide shuffle, done, and finish buttons initially
        shuffleButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);

        // Load card data from folders
        cardDataManagerScene3.LoadCardDataFromFolders();

        // Load a group of cards
        LoadGroup(0);

        // Initialize host name
        hostName = PhotonNetwork.MasterClient.NickName;
        feedbackText.gameObject.SetActive(false);
    }

    private void LoadGroup(int groupIndex)
    {
        currentGroup = cardDataManagerScene3.cardDatas;
        if (currentGroup != null)
        {
            foreach (var cardData in currentGroup)
            {
                Debug.Log($"Card Front: {cardData.cardFront.name}, Questions: {string.Join(", ", cardData.questions)}, Card Type: {cardData.cardType}");
            }
        }
    }

    private void ShowCardBack()
    {
        cardContainer.SetActive(true);

        foreach (Transform card in cardContainer.transform)
        {
            ResetCardToBack(card);
        }
    }

    private void ResetCardToBack(Transform card)
    {
        card.GetComponent<Image>().sprite = cardBack; // Assign card back sprite
        card.localEulerAngles = new Vector3(0, 180, 0); // Set Y rotation to 180 degrees to show the back
        card.localScale = originalScale; // Reset the scale to original size
        card.gameObject.SetActive(true); // Ensure the card is active

        Debug.Log("Resetting card to back: " + card.name);

        // Hide additional image, text, and button elements
        foreach (Transform child in card)
        {
            Image childImage = child.GetComponent<Image>();
            if (childImage != null)
            {
                childImage.enabled = false;
            }

            Text childText = child.GetComponent<Text>();
            if (childText != null)
            {
                childText.enabled = false;
            }

            Button childButton = child.GetComponent<Button>();
            if (childButton != null)
            {
                childButton.gameObject.SetActive(false);
            }
        }
    }

    private void HideCardContent()
    {
        foreach (Transform card in cardContainer.transform)
        {
            foreach (Transform child in card)
            {
                Image childImage = child.GetComponent<Image>();
                if (childImage != null)
                {
                    childImage.enabled = false;
                }

                Text childText = child.GetComponent<Text>();
                if (childText != null)
                {
                    childText.enabled = false;
                }

                Button childButton = child.GetComponent<Button>();
                if (childButton != null)
                {
                    childButton.gameObject.SetActive(false);
                }
            }
        }
    }

    [PunRPC]
    public void ShuffleAndFlipCard(int frontIndex, int cardIndex, int targetIma, int targetTex, PhotonMessageInfo info)
    {
        currentFrontIndex = frontIndex;
        lastCardIndex = cardIndex;
        targetImage = targetIma;
        targetText = targetTex;

        // Reset the previously flipped card
        if (topCard != null)
        {
            ResetCardToBack(topCard);
        }

        // Hide card content during shuffling
        HideCardContent();

        cardAnimator.SetTrigger("ShuffleTrigger"); // Shuffle animation

        Invoke("PrepareSwitch", 2f); // Wait for the shuffle animation to complete
    }

    private void PrepareSwitch()
    {
        topCard = cardContainer.transform.GetChild(lastCardIndex);
        // Keep the back sprite initially
        topCard.GetComponent<Image>().sprite = cardBack;

        // Bring the top card to the front visually
        topCard.SetAsLastSibling();

        // Switch to the card front
        SwitchToCardFront();
    }

    private void SwitchToCardFront()
    {
        if (topCard != null && currentFrontIndex >= 0 && currentFrontIndex < currentGroup.Count)
        {
            CardData cardData = currentGroup[currentFrontIndex];
            // Set the main image of the card front
            Image cardFrontImage = topCard.GetComponent<Image>();
            if (cardFrontImage != null)
            {
                cardFrontImage.sprite = cardData.cardFront;
                cardFrontImage.enabled = true;
            }

            // Set image, text
            foreach (Transform child in topCard)
            {
                Image childImage = child.GetComponent<Image>();
                Text childText = child.GetComponent<Text>();
                Button childButton = child.GetComponent<Button>();

                if (cardData.cardType == CardData.CardType.Picture || cardData.cardType == CardData.CardType.Art)
                {
                    if (childImage != null && cardData.images.Count > 0)
                    {
                        int imageIndex = targetImage;
                        childImage.sprite = cardData.images[imageIndex];
                        childImage.enabled = true;
                    }

                    if (childText != null && cardData.questions.Count > 0)
                    {
                        int questionIndex = targetText;
                        childText.text = cardData.questions[questionIndex];
                        childText.enabled = true;
                    }
                }
                else if (cardData.cardType == CardData.CardType.Sound)
                {
                    if (childButton != null && cardData.soundClips.Count > 0)
                    {
                        int soundIndex = targetImage;
                        AudioSource audioSource = topCard.GetComponent<AudioSource>();
                        if (audioSource == null)
                        {
                            audioSource = topCard.gameObject.AddComponent<AudioSource>();
                        }
                        audioSource.clip = cardData.soundClips[soundIndex];
                        childButton.gameObject.SetActive(true);
                        childButton.onClick.RemoveAllListeners();
                        childButton.onClick.AddListener(() => audioSource.Play());
                    }

                    if (childText != null && cardData.questions.Count > 0)
                    {
                        int questionIndex = targetText;
                        childText.text = cardData.questions[questionIndex];
                        childText.enabled = true;
                    }
                }
            }

            topCard.localEulerAngles = new Vector3(0, 0, 0);
            topCard.localScale = originalScale * 2;
        }
    }

    private void OnShuffleButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int frontIndex = Random.Range(0, currentGroup.Count);
            int cardIndex;
            do
            {
                cardIndex = Random.Range(0, cardContainer.transform.childCount);
            } while (cardIndex == lastCardIndex);
            int textIndex = Random.Range(0, currentGroup[frontIndex].questions.Count);
            int num;
            if (frontIndex == 2)
            {
                num = Random.Range(0, currentGroup[frontIndex].soundClips.Count);
            }
            else
            {
                num = Random.Range(0, currentGroup[frontIndex].images.Count);
            }
            photonView.RPC("ShuffleAndFlipCard", RpcTarget.All, frontIndex, cardIndex, num, textIndex);
        }
    }

    private void OnDoneButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShowCardBackRPC", RpcTarget.All); // Ensure the card back is shown to all players

            // Hide card content when done button is clicked
            HideCardContent();

            // Hide shuffle and done buttons
            shuffleButton.gameObject.SetActive(false);
            doneButton.gameObject.SetActive(false);

            // Show feedback text and finish button
            photonView.RPC("ShowFeedbackText", RpcTarget.All);
        }
    }

    private void OnFinishButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            scorePanel.SetActive(true);
            currentScore = 0;
            detail.sprite = detailNone;
            disclose.sprite = discloseNone;
            passion.sprite = passionNone;
        }
        
    }
    public void InputScore(int s)
    {
        currentScore = s;
        if (currentScore == 1)
        {
            detail.sprite = detailSelect;
            disclose.sprite = discloseNone;
            passion.sprite = passionNone;
        }
        else if (currentScore == 2)
        {
            detail.sprite = detailNone;
            disclose.sprite = discloseSelect;
            passion.sprite = passionNone;
        }
        else
        {
            detail.sprite = detailNone;
            disclose.sprite = discloseNone;
            passion.sprite = passionSelect;
        }
    }
    public void OnSubmitScore()
    {
        if (currentScore == 0)
            return;
        if (PhotonNetwork.IsMasterClient)
        {
            scorePanel.SetActive(false);
            photonView.RPC("HideFeedbackText", RpcTarget.All);

            DisplayRandomPlayerNameScene3 displayRandomPlayerNameScene3 = FindObjectOfType<DisplayRandomPlayerNameScene3>();
            displayRandomPlayerNameScene3.photonView.RPC("CheckScore", RpcTarget.All, currentScore, currentPickedPlayer);
            CollaborateGameManagerScene3 collaborateGameManagerScene3 = FindObjectOfType<CollaborateGameManagerScene3>();
            if (collaborateGameManagerScene3 != null)
            {
                collaborateGameManagerScene3.photonView.RPC("CheckIfAllPlayersPicked", RpcTarget.All);
            }
            else
            {
                if (displayRandomPlayerNameScene3 != null)
                {
                    displayRandomPlayerNameScene3.photonView.RPC("ShowRandomizeButton", RpcTarget.All);
                }
            }
        }
    }
    [PunRPC]
    private void ShowCardBackRPC()
    {
        // Reset the top card to show the card back
        if (topCard != null)
        {
            ResetCardToBack(topCard);
        }
        HideCardContent();

        Debug.Log("Card back shown");
    }

    [PunRPC]
    private void ShowFeedbackText()
    {
        if (isFirstRound)
        {
            feedbackText.text = $"{hostName}, can you give {currentPickedPlayer} some feedback?";
            isFirstRound = false;
        }
        else
        {
            feedbackText.text = $"{previousPickedPlayer}, can you give {currentPickedPlayer} some feedback?";
        }

        feedbackText.gameObject.SetActive(true);
        finishButton.gameObject.SetActive(true);
    }

    [PunRPC]
    private void HideFeedbackText()
    {
        previousPickedPlayer = currentPickedPlayer; // Update the previous picked player
        feedbackText.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);
    }

    // Implement IPunObservable interface
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    // Update picked player
    [PunRPC]
    public void UpdatePickedPlayer(string playerName)
    {
        currentPickedPlayer = playerName;
    }

    public int GetLastCardIndex()
    {
        return lastCardIndex;
    }
}
