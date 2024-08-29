using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class CardManagerScene2 : MonoBehaviourPun, IPunObservable
{
    public GameObject cardContainer; // The container for all cards
    public Sprite cardBack; // The sprite for the back of the card
    public Button shuffleButton; // Button to shuffle the cards again
    public Button doneButton; // Button to finish with the question
    public GameObject scorePanel; // Panel for scoring
    public Text scoreDisplayText; // Text to display the score

    private Animator cardAnimator; // The Animator for the cards
    [SerializeField] private int currentFrontIndex;
    public int targetImage;
    public int targetText;
    public int lastCardIndex = -1; // Changed to public
    [SerializeField]private Transform topCard;
    [SerializeField] private int totalScore = 0;
    [SerializeField]private List<CardData> currentGroup;
    private int requiredScore;

    public CardDataManagerScene2 cardDataManagerScene2; // Reference to CardDataManager
    private Vector3 originalScale;

    void Start()
    {
        cardAnimator = cardContainer.GetComponent<Animator>(); // Find the Animator component

        shuffleButton.onClick.AddListener(OnShuffleButtonClicked);
        doneButton.onClick.AddListener(OnDoneButtonClicked);

        // Ensure the PhotonView observes this component
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

        // Hide shuffle, done buttons, and score panel initially
        shuffleButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
        scorePanel.SetActive(false);

        // Initialize the score display text
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        requiredScore = 9 * playerCount;
        scoreDisplayText.text = $"0/{requiredScore}";

        // Load card data from folders
        cardDataManagerScene2.LoadCardDataFromFolders();

        // Load a group of cards
        LoadGroup(0);
    }

    private void LoadGroup(int groupIndex)
    {
        //currentGroup = cardDataManager.GetGroup(groupIndex);
        currentGroup = cardDataManagerScene2.cardDatas;
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
    public void ShuffleAndFlipCard(int frontIndex, int cardIndex,int targetIma,int targetTex, PhotonMessageInfo info)
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

        cardAnimator.SetTrigger("ShuffleTrigger"); // Trigger the shuffle animation

        Invoke("PrepareSwitch", 2f); // Wait for the shuffle animation to complete
    }

    private void PrepareSwitch()
    {
        topCard = cardContainer.transform.GetChild(lastCardIndex);
        // Keep the back sprite initially
        topCard.GetComponent<Image>().sprite = cardBack;

        // Bring the top card to the front visually
        topCard.SetAsLastSibling();

        // Directly switch to the card front
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

                /*if (childImage != null)
                {
                    childImage.enabled = false; // Hide image initially
                }

                if (childText != null)
                {
                    childText.enabled = false; // Hide text initially
                }

                if (childButton != null)
                {
                    childButton.gameObject.SetActive(false); // Hide button initially
                }*/

                if (cardData.cardType == CardData.CardType.Picture || cardData.cardType == CardData.CardType.Art)
                {
                    if (childImage != null && cardData.images.Count > 0)
                    {
                        //int imageIndex = Random.Range(0, cardData.images.Count);
                        int imageIndex = targetImage;
                        childImage.sprite = cardData.images[imageIndex];
                        childImage.enabled = true;
                    }

                    if (childText != null && cardData.questions.Count > 0)
                    {
                        //int questionIndex = Random.Range(0, cardData.questions.Count);
                        int questionIndex = targetText;
                        childText.text = cardData.questions[questionIndex];
                        childText.enabled = true;
                    }
                }
                else if (cardData.cardType == CardData.CardType.Sound)
                {
                    if (childButton != null && cardData.soundClips.Count > 0)
                    {
                        //int soundIndex = Random.Range(0, cardData.soundClips.Count);
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
            if(frontIndex==2)
            {
                num = Random.Range(0, currentGroup[frontIndex].soundClips.Count);
            }else
            {
                num = Random.Range(0, currentGroup[frontIndex].images.Count);
            }
            photonView.RPC("ShuffleAndFlipCard", RpcTarget.All, frontIndex, cardIndex,num,textIndex);
        }
    }

    private void OnDoneButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShowCardBackRPC", RpcTarget.All); // Ensure the card back is shown to all players

            // Show the scoring panel
            scorePanel.SetActive(true);

            // Hide card content when done button is clicked
            HideCardContent();
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
    }

    public void SetScore(int score)
    {
        photonView.RPC("UpdateScoreRPC", RpcTarget.All, score);
    }

    [PunRPC]
    private void UpdateScoreRPC(int score)
    {
        totalScore += score;
        scoreDisplayText.text = $"{totalScore}/{requiredScore}";

        // Hide the score panel after setting the score
        scorePanel.SetActive(false);

        if (totalScore >= requiredScore)
        {
            photonView.RPC("GameEnd", RpcTarget.All);
        }
        else
        {
            shuffleButton.gameObject.SetActive(false);
            doneButton.gameObject.SetActive(false);
            DisplayRandomPlayerNameScene2 randomPlayerNameScriptScene2 = FindObjectOfType<DisplayRandomPlayerNameScene2>();
            if (randomPlayerNameScriptScene2 != null)
            {
                randomPlayerNameScriptScene2.photonView.RPC("ShowRandomizeButton", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void GameEnd()
    {
        PhotonNetwork.LoadLevel("GameEndScene");
    }

    // Implement the IPunObservable interface
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other players
            stream.SendNext(totalScore);
        }
        else
        {
            // Receive data from other players
            totalScore = (int)stream.ReceiveNext();
            scoreDisplayText.text = $"{totalScore}/{requiredScore}";
        }
    }

    public int GetLastCardIndex()
    {
        return lastCardIndex;
    }
    // Method to update the required score
    public void UpdateRequiredScore(int newRequiredScore)
    {
        requiredScore = newRequiredScore;
        scoreDisplayText.text = $"{totalScore}/{requiredScore}";
    }
}
