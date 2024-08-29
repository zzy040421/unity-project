using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class DisplayRandomPlayerNameScene2 : MonoBehaviourPun
{
    public Text playerNameText;
    public Button randomNameButton;
    public Button shuffleButton; 
    public Button doneButton; 

    private List<string> playerNamesList = new List<string>();
    private List<string> remainingNames = new List<string>();
    private CollaborateGameManagerScene2 collaborateGameManagerScene2; // Reference to CollaborateGameManager
    private CardManagerScene2 cardManagerScene2; // Reference to CardManager

    void Start()
    {
        // Find the CollaborateGameManager in the scene
        collaborateGameManagerScene2 = FindObjectOfType<CollaborateGameManagerScene2>();

        if (collaborateGameManagerScene2 != null)
        {
            // Get the list of player names 
            playerNamesList = collaborateGameManagerScene2.GetPlayerNames();
            remainingNames = new List<string>(playerNamesList);
        }
        else
        {
            playerNameText.text = "CollaborateGameManager not found!";
        }

        // Find the CardManager in the scene
        cardManagerScene2 = FindObjectOfType<CardManagerScene2>();

        if (cardManagerScene2 == null)
        {
            playerNameText.text = "CardManager not found!";
        }

        // hide the shuffle and done buttons
        shuffleButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);

        // Update randomize button visibility based on the Client status
        UpdateRandomizeButtonVisibility();
    }

    void UpdateRandomizeButtonVisibility()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            randomNameButton.gameObject.SetActive(true); // Show the randomize button if is host
            randomNameButton.onClick.RemoveAllListeners(); // Ensure no duplicate listeners
            randomNameButton.onClick.AddListener(OnRandomNameButtonClicked); // Add event listener for the host
        }
        else
        {
            randomNameButton.gameObject.SetActive(false); // Hide the randomize button for joiners
        }
    }

    void OnRandomNameButtonClicked()
    {
        randomNameButton.gameObject.SetActive(false); // Hide the randomize button

        if (remainingNames.Count == 0)
        {
            // Reset the remaining names list when all names have been displayed
            remainingNames = new List<string>(playerNamesList);
        }

        if (remainingNames.Count == 0)
        {
            playerNameText.text = "No players available";
            return;
        }

        // Randomly pick a player name from the remaining names
        string randomPlayerName = GetRandomPlayerName(remainingNames);
        Debug.Log("random: " + randomPlayerName);

        // Update the player name for all clients
        photonView.RPC("RPC_SetRandomPlayerName", RpcTarget.All, randomPlayerName);

        // Remove the picked name from the remaining names list
        remainingNames.Remove(randomPlayerName);

        // Trigger the card shuffle and flip
        if (cardManagerScene2 != null)
        {
            int frontIndex = Random.Range(0, cardManagerScene2.cardDataManagerScene2.GetGroup(0).Count);
            int cardIndex;
            do
            {
                cardIndex = Random.Range(0, cardManagerScene2.cardContainer.transform.childCount);
            } while (cardIndex == cardManagerScene2.GetLastCardIndex());
            int textIndex = Random.Range(0, cardManagerScene2.cardDataManagerScene2.GetGroup(0)[frontIndex].questions.Count);
            int num;
            if (frontIndex == 2)
            {
                num = Random.Range(0, cardManagerScene2.cardDataManagerScene2.GetGroup(0)[frontIndex].soundClips.Count);
            }
            else
            {
                num = Random.Range(0, cardManagerScene2.cardDataManagerScene2.GetGroup(0)[frontIndex].images.Count);
            }
            Debug.Log($"Triggering shuffle and flip. Front index: {frontIndex}, Card index: {cardIndex}");
            cardManagerScene2.photonView.RPC("ShuffleAndFlipCard", RpcTarget.All, frontIndex, cardIndex,num,textIndex);
        }

        // Show the shuffle and done buttons
        shuffleButton.gameObject.SetActive(true);
        doneButton.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_SetRandomPlayerName(string randomPlayerName)
    {
        // Display the randomly picked player name
        playerNameText.text = randomPlayerName;
    }

    string GetRandomPlayerName(List<string> playerNames)
    {
        int randomIndex = Random.Range(0, playerNames.Count);
        return playerNames[randomIndex];
    }

    public void UpdatePlayerNames(List<string> newPlayerNames)
    {
        playerNamesList = new List<string>(newPlayerNames);
        remainingNames = new List<string>(newPlayerNames);
    }

    [PunRPC]
    public void ShowRandomizeButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            randomNameButton.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void HideRandomizeButton()
    {
        randomNameButton.gameObject.SetActive(false);
    }
}
