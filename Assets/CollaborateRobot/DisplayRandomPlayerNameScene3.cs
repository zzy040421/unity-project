using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class DisplayRandomPlayerNameScene3 : MonoBehaviourPun
{
    public Text playerNameText;
    public Button randomNameButton;
    public Button shuffleButton;
    public Button doneButton;

    private List<string> playerNamesList = new List<string>();
    public Dictionary<string, int> PlayerPickCounts { get; private set; } = new Dictionary<string, int>();
    public static Dictionary<ScoreType, int> scoreSituation = new Dictionary<ScoreType, int>();
    private List<string> remainingNames = new List<string>();
    public List<string> overNames = new List<string>();
    private CollaborateGameManagerScene3 collaborateGameManagerScene3; // Reference to CollaborateGameManager
    private CardManagerScene3 cardManagerScene3; // Reference to CardManager
    private string lastPickedPlayer;
    private string hostName;
    private bool allPickedOnce = false; // Flag to check if all players have been picked once

    void Start()
    {
        // Find the CollaborateGameManager in the scene
        collaborateGameManagerScene3 = FindObjectOfType<CollaborateGameManagerScene3>();
        foreach (var x in ClientContent.Instance().allName)
        {
            Refresh(x);
        }
        if (collaborateGameManagerScene3 != null)
        {
            // Get the list of player names 
            playerNamesList = collaborateGameManagerScene3.GetPlayerNames();
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

            hostName = PhotonNetwork.MasterClient.NickName;
        }
        else
        {
            playerNameText.text = "CollaborateGameManager not found!";
        }

        // Find the CardManager in the scene
        cardManagerScene3 = FindObjectOfType<CardManagerScene3>();

        if (cardManagerScene3 == null)
        {
            playerNameText.text = "CardManager not found!";
        }

        // hide the shuffle and done buttons
        shuffleButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(false);

        // Update randomize button visibility based on the Client status
        UpdateRandomizeButtonVisibility();
    }
    public void AddRefresh()
    {
        collaborateGameManagerScene3 = FindObjectOfType<CollaborateGameManagerScene3>();
        foreach (var x in ClientContent.Instance().allName)
        {
            Refresh(x);
        }
        if (collaborateGameManagerScene3 != null)
        {
            // Get the list of player names 
            playerNamesList = collaborateGameManagerScene3.GetPlayerNames();
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

            hostName = PhotonNetwork.MasterClient.NickName;
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
    public void CheckScore(int score, string na)
    {
        if (score == 1)
        {
            RecordScoreScene.instance.scoreD[na][ScoreType.detail] += 1;
        }
        else if (score == 2)
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
            if (!allPickedOnce)
            {
                allPickedOnce = true;
                remainingNames = new List<string>(playerNamesList);
            }
            else
            {
                // Reset the remaining names for the second round
                remainingNames = new List<string>(playerNamesList);
            }
        }
*/
        List<string> strs = new List<string>();
        foreach (var x in PlayerPickCounts)
        {
            if (x.Value < 2)
                strs.Add(x.Key);
        }
        if (overNames.Count == ClientContent.Instance().allName.Count)
            overNames.Clear();
        foreach (var x in overNames)
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

        // Increment the pick count for the picked player
        PlayerPickCounts[randomPlayerName]++;
        // Remove the picked name from the remaining names list if they've been picked once and not all have been picked once
        if (PlayerPickCounts[randomPlayerName] == 1 && !allPickedOnce)
        {
            remainingNames.Remove(randomPlayerName);
        }
        // Remove the picked name from the remaining names list if they've been picked twice
        else if (PlayerPickCounts[randomPlayerName] == 2)
        {
            remainingNames.Remove(randomPlayerName);
        }

        // Update the current picked player in CardManager
        if (cardManagerScene3 != null)
        {
            cardManagerScene3.photonView.RPC("UpdatePickedPlayer", RpcTarget.All, randomPlayerName);
        }

        // Trigger the card shuffle and flip
        if (cardManagerScene3 != null)
        {
            int frontIndex = Random.Range(0, cardManagerScene3.cardDataManagerScene3.GetGroup(0).Count);
            int cardIndex;
            do
            {
                cardIndex = Random.Range(0, cardManagerScene3.cardContainer.transform.childCount);
            } while (cardIndex == cardManagerScene3.GetLastCardIndex());
            int textIndex = Random.Range(0, cardManagerScene3.cardDataManagerScene3.GetGroup(0)[frontIndex].questions.Count);
            int num;
            if (frontIndex == 2)
            {
                num = Random.Range(0, cardManagerScene3.cardDataManagerScene3.GetGroup(0)[frontIndex].soundClips.Count);
            }
            else
            {
                num = Random.Range(0, cardManagerScene3.cardDataManagerScene3.GetGroup(0)[frontIndex].images.Count);
            }
            Debug.Log($"Triggering shuffle and flip. Front index: {frontIndex}, Card index: {cardIndex}");
            cardManagerScene3.photonView.RPC("ShuffleAndFlipCard", RpcTarget.All, frontIndex, cardIndex, num, textIndex);
        }

        // Show the shuffle and done buttons
        shuffleButton.gameObject.SetActive(true);
        doneButton.gameObject.SetActive(true);
        playerNameText.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_SetRandomPlayerName(string randomPlayerName)
    {
        // Display the randomly picked player name
        playerNameText.text = randomPlayerName;
    }

    string GetRandomPlayerName(List<string> playerNames)
    {
        List<string> validNames = new List<string>(playerNames);

        // Remove the last picked player from the valid names list
        if (!cardManagerScene3.isFirstRound && validNames.Count > 1)
        {
            validNames.Remove(lastPickedPlayer);
        }

        // Ensure the host is not picked first but included later
        if (cardManagerScene3.isFirstRound && validNames.Contains(hostName))
        {
            validNames.Remove(hostName);
        }

        // If no valid names are left (which should not happen), just return the host name as a fallback
        if (validNames.Count == 0)
        {
            return hostName;
        }

        int randomIndex = Random.Range(0, validNames.Count);
        return validNames[randomIndex];
    }

    public void UpdatePlayerNames(List<string> newPlayerNames)
    {
        playerNamesList = new List<string>(newPlayerNames);
        remainingNames = new List<string>(newPlayerNames);

        // Reset player pick counts
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
        foreach (var count in PlayerPickCounts.Values)
        {
            if (count < 2)
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
        RecordScoreScene.instance.isRobot = true;
        PhotonNetwork.LoadLevel("GameEnd2");
    }
}
