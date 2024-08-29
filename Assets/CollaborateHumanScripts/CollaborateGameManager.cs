using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class CollaborateGameManager : MonoBehaviourPunCallbacks
{
    public Transform playerListContent; // Hold player name objects
    private List<string> playerNames = new List<string>();
    private List<GameObject> playerNameObjects = new List<GameObject>();
    public GameObject playerNamePrefab;
    public InputField playerNameInputField; // entering new player names
    public Button submitButton; // submit player name

    private DisplayRandomPlayerName displayRandomPlayerName;
    private CardManager cardManager; // Reference to the CardManager script
    private string hostName; 

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
            hostName = PhotonNetwork.NickName; // Set the host's name
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
        displayRandomPlayerName = FindObjectOfType<DisplayRandomPlayerName>();
        if (displayRandomPlayerName != null)
        {
            displayRandomPlayerName.UpdatePlayerNames(GetFilteredPlayerNames());
        }
        cardManager = FindObjectOfType<CardManager>();
    }

    public void AddPlayerNameToList(string playerName)
    {
        if (PlayerNameExists(playerName) || string.IsNullOrWhiteSpace(playerName))
        {
            Debug.Log("Invalid or duplicate player name: " + playerName);
            return;
        }

        playerNames.Add(playerName);
        ClientContent.Instance().allName.Add(playerName);
        Debug.Log("Added player name to list: " + playerName);
        AddPlayerNameToUI(playerName);
        displayRandomPlayerName.AddRefresh();
        photonView.RPC("RPC_AddPlayerName", RpcTarget.AllBuffered, playerName);

        // Update DisplayRandomPlayerName with the new player list
        if (displayRandomPlayerName != null)
        {
            displayRandomPlayerName.UpdatePlayerNames(GetFilteredPlayerNames());
        }
    }

    [PunRPC]
    void RPC_AddPlayerName(string playerName)
    {
        if (!PlayerNameExists(playerName) && !string.IsNullOrWhiteSpace(playerName))
        {
            playerNames.Add(playerName);
            AddPlayerNameToUI(playerName);
            ClientContent.Instance().allName.Add(playerName);
            displayRandomPlayerName.AddRefresh();
            // Update DisplayRandomPlayerName with the new player list
            if (displayRandomPlayerName != null)
            {
                displayRandomPlayerName.UpdatePlayerNames(GetFilteredPlayerNames());
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

            
            RectTransform rectTransform = playerNameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 30); // Set a fixed height
            playerNameText.alignment = TextAnchor.MiddleLeft; // Ensure text alignment is consistent

            playerNameObjects.Add(playerNameObject);

            // Force the layout to update 
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
        return ClientContent.Instance().allName;
    }

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

    [PunRPC]
    public void CheckIfAllPlayersPicked()
    {
        foreach (var count in displayRandomPlayerName.PlayerPickCounts)
        {
            if (count.Key == PhotonNetwork.MasterClient.NickName)
                continue;
            if (count.Value < 2)
            {
                displayRandomPlayerName.photonView.RPC("ShowRandomizeButton", RpcTarget.All);
                return;
            }
        }
        photonView.RPC("RPC_ShowFinalResults", RpcTarget.All);
    }

    [PunRPC]
    void RPC_ShowFinalResults()
    {
        PhotonNetwork.LoadLevel("GameEndScene");
    }

    // Get player names 
    private List<string> GetFilteredPlayerNames()
    {
        return playerNames.FindAll(name => name != hostName);
    }
}
