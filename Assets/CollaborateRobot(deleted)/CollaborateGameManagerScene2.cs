using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class CollaborateGameManagerScene2 : MonoBehaviourPunCallbacks
{
    public Transform playerListContent; // Parent transform to hold player name objects
    private List<string> playerNames = new List<string>();
    private List<GameObject> playerNameObjects = new List<GameObject>();
    public GameObject playerNamePrefab;
    public InputField playerNameInputField; // InputField for entering new player names
    public Button submitButton; // Button to submit new player name
    public Text roomCodeText; // Text component to display the room code

    private DisplayRandomPlayerNameScene2 displayRandomPlayerNameScene2;
    private CardManagerScene2 cardManagerScene2; // Reference to the CardManager script

    public void Start()
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing.");
            return;
        }

        submitButton.onClick.AddListener(OnSubmitButtonClicked);
        playerNameInputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            playerNameInputField.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(true);
        }

        // Add existing names from ClientContent
        foreach (var name in ClientContent.Instance().allName)
        {
            AddPlayerNameToUI(name);
            playerNames.Add(name); // Add to playerNames list
        }
        foreach (var name in ClientContent.allNameString)
        {
            AddPlayerNameToUI(name);
            playerNames.Add(name); // Add to playerNames list
        }

        // Find the DisplayRandomPlayerName component
        displayRandomPlayerNameScene2 = FindObjectOfType<DisplayRandomPlayerNameScene2>();
        if (displayRandomPlayerNameScene2 != null)
        {
            displayRandomPlayerNameScene2.UpdatePlayerNames(playerNames);
        }
        cardManagerScene2 = FindObjectOfType<CardManagerScene2>();
        if (cardManagerScene2 != null)
        {
            UpdateRequiredScore();
        }
    }

    public void AddPlayerNameToList(string playerName)
    {
        if (PlayerNameExists(playerName) || string.IsNullOrWhiteSpace(playerName))
        {
            Debug.Log("Invalid or duplicate player name: " + playerName);
            return;
        }

        playerNames.Add(playerName);
        Debug.Log("Added player name to list: " + playerName);
        AddPlayerNameToUI(playerName);
        photonView.RPC("RPC_AddPlayerName", RpcTarget.AllBuffered, playerName);

        // Update DisplayRandomPlayerName with the new player list
        if (displayRandomPlayerNameScene2 != null)
        {
            displayRandomPlayerNameScene2.UpdatePlayerNames(playerNames);
        }
        // Update the required score when a new player is added
        if (cardManagerScene2 != null)
        {
            UpdateRequiredScore();
        }
    }

    [PunRPC]
    void RPC_AddPlayerName(string playerName)
    {
        if (!PlayerNameExists(playerName) && !string.IsNullOrWhiteSpace(playerName))
        {
            playerNames.Add(playerName);
            AddPlayerNameToUI(playerName);

            // Update DisplayRandomPlayerName with the new player list
            if (displayRandomPlayerNameScene2 != null)
            {
                displayRandomPlayerNameScene2.UpdatePlayerNames(playerNames);
            }
            // Update the required score when a new player is added
            if (cardManagerScene2 != null)
            {
                UpdateRequiredScore();
            }
        }
    }

    void AddPlayerNameToUI(string playerName)
    {
        Debug.Log("Adding player name to UI: " + playerName);
        if (playerNamePrefab != null && playerListContent != null)
        {
            GameObject playerNameObject = Instantiate(playerNamePrefab, playerListContent);
            Text playerNameText = playerNameObject.GetComponent<Text>();
            playerNameText.text = playerName;

            // Ensure no extra space is added
            RectTransform rectTransform = playerNameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 30); // Set a fixed height
            playerNameText.alignment = TextAnchor.MiddleLeft; // Ensure text alignment is consistent

            playerNameObjects.Add(playerNameObject);

            // Force the layout to update immediately
            LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContent.GetComponent<RectTransform>());
        }
        else
        {
            Debug.LogError("Player Name Prefab or Player List Content is not assigned in the Inspector.");
        }
    }

    private bool PlayerNameExists(string playerName)
    {
        return playerNames.Contains(playerName);
    }

    void OnSubmitButtonClicked()
    {
        string newPlayerName = playerNameInputField.text.Trim();
        if (!string.IsNullOrEmpty(newPlayerName) && !PlayerNameExists(newPlayerName))
        {
            AddPlayerNameToList(newPlayerName);
            playerNameInputField.text = string.Empty;
        }
        else
        {
            Debug.LogWarning("Player name is either empty or already exists.");
        }
    }

    private void OnPlayerNameClicked(string playerName)
    {
        Debug.Log("Player name clicked: " + playerName);
    }

    public List<string> GetPlayerNames()
    {
        return playerNames;
    }

    // Method to clear player names and UI objects
    public void ClearPlayerList()
    {
        foreach (GameObject obj in playerNameObjects)
        {
            Destroy(obj);
        }
        playerNameObjects.Clear();
        playerNames.Clear();
    }

    // LoadGameMode method with [PunRPC] attribute
    [PunRPC]
    public void LoadGameMode(string mode)
    {
        Debug.Log("Loading game mode: " + mode);
        PhotonNetwork.LoadLevel(mode);
    }

    public void OnModeButtonClicked(string mode)
    {
        photonView.RPC("LoadGameMode", RpcTarget.All, mode);
    }
    private void UpdateRequiredScore()
    {
        if (cardManagerScene2 != null)
        {
            int playerCount = playerNames.Count;
            int requiredScore = 9 * playerCount;
            cardManagerScene2.UpdateRequiredScore(requiredScore);
        }
    }
}
