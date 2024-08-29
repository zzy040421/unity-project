using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using System.Xml.Serialization;
using Unity.VisualScripting;

public class DisplayRandomPlayerName : MonoBehaviourPun
{
    public Text playerNameText;
    public Button randomNameButton;
    public Button shuffleButton;
    public Button doneButton;

    private List<string> playerNamesList = new List<string>();
    public Dictionary<string, int> PlayerPickCounts { get; private set; } = new Dictionary<string, int>();
    public static Dictionary<ScoreType, int> scoreSituation = new Dictionary<ScoreType, int>();
    public static bool isOver;
    public List<string> remainingNames = new List<string>();
    public List<string> overNames = new List<string>();
    private CollaborateGameManager collaborateGameManager; // Reference to CollaborateGameManager
    private CardManager cardManager; // Reference to CardManager
    private string lastPickedPlayer;

    void Start()
    {
        // Find the CollaborateGameManager in the scene
        collaborateGameManager = FindObjectOfType<CollaborateGameManager>();
        foreach(var x in ClientContent.Instance().allName)
        {
            Refresh(x);
        }
        
        if (collaborateGameManager != null)
        {
            // Get the list of player names 
            playerNamesList = collaborateGameManager.GetPlayerNames();
            remainingNames = new List<string>();
            foreach(var x in ClientContent.Instance().allName)
            {
                remainingNames.Add(x);
            }
            // Initialize player pick counts
            foreach (var playerName in playerNamesList)
            {
                if (!PlayerPickCounts.ContainsKey(playerName))
                    PlayerPickCounts.Add(playerName,0);
                PlayerPickCounts[playerName] = 0;
            }
        }
        else
        {
            playerNameText.text = "CollaborateGameManager not found!";
        }

        // Find the CardManager in the scene
        cardManager = FindObjectOfType<CardManager>();

        if (cardManager == null)
        {
            playerNameText.text = "CardManager not found!";
        }

        // hide the shuffle and done buttons
        shuffleButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(false);

        // Update randomize button visibility 
        UpdateRandomizeButtonVisibility();
    }
    public void AddRefresh()
    {
        collaborateGameManager = FindObjectOfType<CollaborateGameManager>();
        foreach (var x in ClientContent.Instance().allName)
        {
            Refresh(x);
        }

        if (collaborateGameManager != null)
        {
            // Get the list of player names 
            playerNamesList = collaborateGameManager.GetPlayerNames();
            remainingNames = new List<string>();
            foreach (var x in ClientContent.Instance().allName)
            {
                remainingNames.Add(x);
            }
            // Initialize player pick counts
            foreach (var playerName in playerNamesList)
            {
                if (!PlayerPickCounts.ContainsKey(playerName))
                    PlayerPickCounts.Add(playerName, 0);
                PlayerPickCounts[playerName] = 0;
            }
        }
        else
        {
            playerNameText.text = "CollaborateGameManager not found!";
        }
    }
    public void Refresh(string na)
    {
        if (!RecordScoreScene.instance.scoreD.ContainsKey(na))
            RecordScoreScene.instance.scoreD.Add(na, new Dictionary<ScoreType, int>());
        if (!RecordScoreScene.instance.scoreD[na].ContainsKey(ScoreType.detail))
            RecordScoreScene.instance.scoreD[na].Add(ScoreType.detail, 0);
        else
            RecordScoreScene.instance.scoreD[na][ScoreType.detail] = 0;
        if (!RecordScoreScene.instance.scoreD[na].ContainsKey(ScoreType.disclose))
            RecordScoreScene.instance.scoreD[na].Add(ScoreType.disclose, 0);
        else
            RecordScoreScene.instance.scoreD[na][ScoreType.disclose] = 0;
        if (!RecordScoreScene.instance.scoreD[na].ContainsKey(ScoreType.passion))
            RecordScoreScene.instance.scoreD[na].Add(ScoreType.passion, 0);
        else
            RecordScoreScene.instance.scoreD[na][ScoreType.passion] = 0;
    }
    [PunRPC]
    public void CheckScore(int score,string na)
    {
        if(score==1)
        {
            RecordScoreScene.instance.scoreD[na][ScoreType.detail] += 1;
        }else if(score == 2)
        {
            RecordScoreScene.instance.scoreD[na][ScoreType.disclose] += 1;
        }
        else
        {
            RecordScoreScene.instance.scoreD[na][ScoreType.passion] += 1;
        }
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

        /*if (remainingNames.Count == 0)
        {
            // Reset the remaining names
            remainingNames = new List<string>(playerNamesList);
        }
*/
        List<string> strs = new List<string>();
        foreach(var x in PlayerPickCounts)
        {
            if (x.Value < 2)
                strs.Add(x.Key);
        }
        if (overNames.Count == ClientContent.Instance().allName.Count-1)
            overNames.Clear();
        foreach(var x in overNames)
        {
            if (strs.Contains(x))
                strs.Remove(x);
        }
        string randomPlayerName = GetRandomPlayerName(strs);
        Debug.Log("random: " + randomPlayerName);
        overNames.Add(randomPlayerName);
        // Store the last picked player
        lastPickedPlayer = randomPlayerName;

        // Update the player name for all clients
        photonView.RPC("RPC_SetRandomPlayerName", RpcTarget.All, randomPlayerName);
        foreach(var x in remainingNames)
        {
        }
        // Increment the pick count for the picked player
        PlayerPickCounts[randomPlayerName]++;
        foreach (var x in PlayerPickCounts)
        {
        }
        // Remove the picked name from the remaining names list if they've been picked twice
        if (PlayerPickCounts[randomPlayerName] == 2)
        {
            remainingNames.Remove(randomPlayerName);
        }
        // Update the current picked player in CardManager
        if (cardManager != null)
        {
            cardManager.photonView.RPC("UpdatePickedPlayer", RpcTarget.All, randomPlayerName);
        }

        // Trigger the card shuffle and flip
        if (cardManager != null)
        {
            int frontIndex = Random.Range(0, cardManager.cardDataManager.GetGroup(0).Count);
            int cardIndex;
            do
            {
                cardIndex = Random.Range(0, cardManager.cardContainer.transform.childCount);
            } while (cardIndex == cardManager.GetLastCardIndex());
            int textIndex = Random.Range(0, cardManager.cardDataManager.GetGroup(0)[frontIndex].questions.Count);
            int num;
            if (frontIndex == 2)
            {
                num = Random.Range(0, cardManager.cardDataManager.GetGroup(0)[frontIndex].soundClips.Count);
            }
            else
            {
                num = Random.Range(0, cardManager.cardDataManager.GetGroup(0)[frontIndex].images.Count);
            }
            Debug.Log($"Triggering shuffle and flip. Front index: {frontIndex}, Card index: {cardIndex}");
            cardManager.photonView.RPC("ShuffleAndFlipCard", RpcTarget.All, frontIndex, cardIndex, num, textIndex);
        }

        // Show the shuffle and done buttons
        shuffleButton.gameObject.SetActive(true);
        doneButton.gameObject.SetActive(true);
        playerNameText.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_SetRandomPlayerName(string randomPlayerName)
    {
        // Display the picked player name
        playerNameText.text = randomPlayerName;
    }

    string GetRandomPlayerName(List<string> playerNames)
    {
        List<string> validNames = new List<string>(playerNames);
        // Remove the last picked player from the names list
            validNames.Remove(PhotonNetwork.MasterClient.NickName);
        if (!cardManager.isFirstRound && validNames.Count > 1)//(lastPickedPlayer != null && validNames.Count > 1)
        {

            validNames.Remove(lastPickedPlayer);
        }
        int randomIndex = Random.Range(0, validNames.Count);
        return validNames[randomIndex];
    }

    public void UpdatePlayerNames(List<string> newPlayerNames)
    {
        playerNamesList = new List<string>(newPlayerNames);
        remainingNames = new List<string>(newPlayerNames);

        // Reset pick counts
        foreach (var playerName in newPlayerNames)
        {
            PlayerPickCounts[playerName] = 0;
        }
    }

    [PunRPC]
    public void ShowRandomizeButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            randomNameButton.gameObject.SetActive(true);
        }
    }

    public void CheckIfAllPlayersPicked()
    {
        foreach (var count in PlayerPickCounts)
        {
            if (count.Key == PhotonNetwork.MasterClient.NickName)
                continue;
            if (count.Value < 2)
            {
                photonView.RPC("ShowRandomizeButton", RpcTarget.All);
                return;
            }
        }
        photonView.RPC("RPC_ShowFinalResults", RpcTarget.All);
    }

    [PunRPC]
    void RPC_ShowFinalResults()
    {
        RecordScoreScene.instance.isHuman = true;
        PhotonNetwork.LoadLevel("GameEndScene");
    }
}
public enum ScoreType
{
    passion,
    detail,
    disclose,
}
