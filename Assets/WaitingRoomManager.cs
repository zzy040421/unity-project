// // using Photon.Pun;
// // using Photon.Realtime;
// // using UnityEngine;
// // using UnityEngine.UI;
// // using System.Collections;
// // using System.Collections.Generic;

// // public class WaitingRoomManager: MonoBehaviourPunCallbacks
// // {
// // public Text roomCodeText;
// // public Text playerListText;
// // public InputField addPlayerInput;
// // public Button addPlayerButton;
// // public Button startGameButton;
// // public Button leaveRoomButton;


// // public Button collaborateHumanButton;
// // public Button collaborateRobotButton;
// // public Button competeModeButton;

// // private List<string> playerNames = new List<string>();

// // void Start()

// // {
// //   // Ensure all required components are assigned
// // if (!roomCodeText || !playerListText || !addPlayerInput || !addPlayerButton ||
// // !startGameButton || !leaveRoomButton || !collaborateHumanButton ||
// // !collaborateRobotButton || !competeModeButton)
// // {
// // Debug.LogError("Some required components are not assigned in the Inspector.");
// // return;
// // }

// //   // Display room code
// // roomCodeText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;

// //   // Update player list initially
// // UpdatePlayerList();

// //   // Setup UI and event listeners based on Master Client or regular player
// // if (PhotonNetwork.IsMasterClient)
// // {
// //     addPlayerButton.onClick.AddListener(OnAddPlayerButtonClicked);
// //     startGameButton.onClick.AddListener(OnStartGameButtonClicked);
// //     leaveRoomButton.gameObject.SetActive(false);
// // }
// // else
// // {
// //     addPlayerInput.gameObject.SetActive(false);
// //     addPlayerButton.gameObject.SetActive(false);
// //     startGameButton.gameObject.SetActive(false);
// //     leaveRoomButton.gameObject.SetActive(true);
// //     leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
// // }

// //   // Hide mode buttons initially
// // collaborateHumanButton.gameObject.SetActive(false);
// // collaborateRobotButton.gameObject.SetActive(false);
// // competeModeButton.gameObject.SetActive(false);
// // }

// // [PunRPC]
// // void AddPlayer(string playerName)
// // {
// //     Debug.Log("AddPlayer RPC called with player name: " + playerName);

// //     // Check for empty or null player name
// //     if (string.IsNullOrEmpty(playerName))
// //     {
// //         Debug.LogWarning("Received null or empty player name in AddPlayer RPC.");
// //         return;
// //     }

// //     // Check if playerName already exists locally
// //     if (playerNames.Contains(playerName))
// //     {
// //         Debug.Log("Player name already exists in the local list: " + playerName);
// //         return;
// //     }
// //     ClientContent.allNameString.Add(playerName);

// //     // Check if playerName exists in PhotonNetwork.PlayerList
// //     bool playerExistsInPhoton = false;
// //     bool hostExistsInPhoton   = false;
// //     // Player exists in Photon
// //     foreach (Player player in PhotonNetwork.PlayerList)
// //     {
// //         if (player.NickName == playerName)
// //         {
// //             playerExistsInPhoton = true;
// //             break;
// //         }
// //     }

// //     // Host exists in Photon
// //     if (PhotonNetwork.MasterClient != null && PhotonNetwork.MasterClient.NickName == playerName)
// //     {
// //         hostExistsInPhoton = true;
// //     }

// //     // If playerName does not exist in either local or Photon player list, add it
// //     if (!playerExistsInPhoton || !hostExistsInPhoton)
// //     {
// //         playerNames.Add(playerName);
// //         Debug.Log("Added player name to local list: " + playerName);

// //           // Update player list UI
// //         UpdatePlayerList();

// //           // Find CollaborateGameManager and add player name to its list
// //         var collaborateGameManager = FindObjectOfType<CollaborateGameManager>();
// //         if (collaborateGameManager != null)
// //         {
// //             collaborateGameManager.AddPlayerNameToList(playerName);
// //             Debug.Log("Added player name to CollaborateGameManager: " + playerName);
// //         }
// //     }
// //     else
// //     {
// //         Debug.Log("Player name already exists in Photon player list: " + playerName);
// //     }
// // }

// // void UpdatePlayerList()
// // {
// //     Debug.Log("Updating player list...");
// //     playerListText.text = "Players:\n";
// //     ClientContent.Instance().allName.Clear();

// //     // Add host's name (master client)
// //     playerListText.text += (ClientContent.Instance().name + "\n");
// //     ClientContent.Instance().allName.Add(ClientContent.Instance().name); 

// //     //playerNames.Add(PhotonNetwork.MasterClient.NickName);

// //     Debug.Log("Added host's name: " + PhotonNetwork.MasterClient.NickName);

// //     // Add PhotonNetwork players (excluding the host)
// //      foreach (Player player in PhotonNetwork.PlayerListOthers)
// //     {
// //         playerListText.text += (player.NickName + "\n");
// //         ClientContent.Instance().allName.Add(player.NickName);
// //         Debug.Log("Added player from PhotonNetwork: " + player.NickName);
// //     } 

// //     // Add local player names
// //      foreach (string playerName in playerNames)
// //     {
// //         if (!IsPlayerInPhotonList(playerName))
// //         {
// //             playerListText.text += (playerName + "\n");
// //             Debug.Log("Added local player name: " + playerName);
// //         }
// //     }

// //     Debug.Log("Updated player list: " + playerListText.text);
// // } 

// //   /* void UpdatePlayerList()
// // {
// //     Debug.Log("Updating player list...");
// //     playerListText.text = "Players:\n";

// //       // Add PhotonNetwork players (excluding the host initially)
// //     foreach (Player player in PhotonNetwork.PlayerList)
// //     {
// //         if (player != PhotonNetwork.MasterClient)
// //         {
// //             playerListText.text += player.NickName + "\n";
// //             Debug.Log("Added player from PhotonNetwork: " + player.NickName);
// //         }
// //     }

// //       // Add host's name if it's not already in the list
// //     if (!playerNames.Contains(PhotonNetwork.MasterClient.NickName))
// //     {
// //         playerListText.text += PhotonNetwork.MasterClient.NickName + "\n";
// //         playerNames.Add(PhotonNetwork.MasterClient.NickName);
// //         Debug.Log("Added host's name to playerNames list: " + PhotonNetwork.MasterClient.NickName);
// //     }

// //       // Add local player names
// //     foreach (string playerName in playerNames)
// //     {
// //           // Skip adding the host's name again (already added above)
// //         if (playerName != PhotonNetwork.MasterClient.NickName)
// //         {
// //             playerListText.text += playerName + "\n";
// //             Debug.Log("Added local player name: " + playerName);
// //         }
// //     }

// //     Debug.Log("Updated player list: " + playerListText.text);
// // }
// //  */

// // bool IsPlayerInPhotonList(string playerName)
// // {
// //     foreach (Player player in PhotonNetwork.PlayerListOthers)
// //     {
// //         if (player.NickName == playerName)
// //         {
// //             return true;
// //         }
// //     }
// //     return false;
// // }

// // public void OnAddPlayerButtonClicked()
// // {
    
// //     string playerName = addPlayerInput.text.Trim();
    
// //       /* if (string.IsNullOrEmpty(playerName))
// //     {
// //         Debug.Log("Player name is empty or null.");
// //         addPlayerButton.interactable = true;  // Ensure button is enabled if input is empty
// //         return;
// //     }
// //  */
// //         Debug.Log("Attempting to add player with name: " + playerName);

// //       // Disable the button while processing
// //       //addPlayerButton.interactable = false;

// //       // Check if playerName already exists
// //     if (playerNames.Contains(playerName))
// //     {
// //         Debug.Log("Player name already exists in the local list: " + playerName);
// //           //addPlayerButton.interactable = true; // Re-enable button
// //         return;
// //     }

    

// //       // Clear input field and activate it again
// //     addPlayerInput.text = "";
// //     addPlayerInput.ActivateInputField();

// //       // Call RPC to add player across network
// //     photonView.RPC("AddPlayer", RpcTarget.All, playerName);
// // }






// // public void OnStartGameButtonClicked()
// // {
// //     Debug.Log("Start Game button clicked");
// //     addPlayerInput.gameObject.SetActive(false);
// //     addPlayerButton.gameObject.SetActive(false);
// //     startGameButton.gameObject.SetActive(false);


// //     collaborateHumanButton.gameObject.SetActive(true);
// //     collaborateRobotButton.gameObject.SetActive(true);
// //     competeModeButton.gameObject.SetActive(true);

// //     // Set listeners for mode buttons
// //     collaborateHumanButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateHumanMC"));
// //     collaborateRobotButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateRobotMC"));
// //     competeModeButton.onClick.AddListener(() => OnModeButtonClicked("CompeteMode"));
// // }

// // public void OnLeaveRoomButtonClicked()
// // {
// //     Debug.Log("Leave Room button clicked");
// //     playerNames.Remove(PhotonNetwork.NickName);
// //     UpdatePlayerList();


// //     // Leave room using Photon
// //     PhotonNetwork.LeaveRoom();
// // }

// // public override void OnLeftRoom()
// // {
// //   // Load scene after leaving room
// //     UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
// // }

// // public void OnModeButtonClicked(string mode)
// // {
// //     Debug.Log("Mode button clicked: " + mode);
// //     photonView.RPC("LoadGameMode", RpcTarget.All, mode);
// // }

// // [PunRPC]
// // public void LoadGameMode(string mode)
// // {
// //     Debug.Log("Loading game mode: " + mode);
// //     PhotonNetwork.LoadLevel(mode);
// // }

// // public override void OnPlayerEnteredRoom(Player newPlayer)
// // {
// //     Debug.Log("Player entered room: " + newPlayer.NickName);
// //     UpdatePlayerList();
// // }

// // public override void OnPlayerLeftRoom(Player otherPlayer)
// // {
// //     Debug.Log("Player left room: " + otherPlayer.NickName);
// //     UpdatePlayerList();
// // }

// // public List<string> GetPlayerNames()
// // {
// //     return playerNames;
// // }
// // }


// // using Photon.Pun;
// // using Photon.Realtime;
// // using UnityEngine;
// // using UnityEngine.UI;
// // using System.Collections;
// // using System.Collections.Generic;

// // public class WaitingRoomManager : MonoBehaviourPunCallbacks
// // {
// //     public Text roomCodeText;
// //     public Text playerListText;
// //     public InputField addPlayerInput;
// //     public Button addPlayerButton;
// //     public Button startGameButton;
// //     public Button leaveRoomButton;

// //     public Button collaborateHumanButton;
// //     public Button collaborateRobotButton;
// //     public Button competeModeButton;

// //     private List<string> playerNames = new List<string>();

// //     void Start()
// //     {
// //         // Ensure all required components are assigned
// //         if (!roomCodeText || !playerListText || !addPlayerInput || !addPlayerButton ||
// //             !startGameButton || !leaveRoomButton || !collaborateHumanButton ||
// //             !collaborateRobotButton || !competeModeButton)
// //         {
// //             Debug.LogError("Some required components are not assigned in the Inspector.");
// //             return;
// //         }

// //         // Display room code
// //         roomCodeText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;

// //         // Update player list initially
// //         UpdatePlayerList();

// //         // Setup UI and event listeners based on Master Client or regular player
// //         if (PhotonNetwork.IsMasterClient)
// //         {
// //             addPlayerButton.onClick.AddListener(OnAddPlayerButtonClicked);
// //             startGameButton.onClick.AddListener(OnStartGameButtonClicked);
// //             leaveRoomButton.gameObject.SetActive(false);
// //         }
// //         else
// //         {
// //             addPlayerInput.gameObject.SetActive(false);
// //             addPlayerButton.gameObject.SetActive(false);
// //             startGameButton.gameObject.SetActive(false);
// //             leaveRoomButton.gameObject.SetActive(true);
// //             leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
// //         }

// //         // Hide mode buttons initially
// //         collaborateHumanButton.gameObject.SetActive(false);
// //         collaborateRobotButton.gameObject.SetActive(false);
// //         competeModeButton.gameObject.SetActive(false);
// //     }

// //     [PunRPC]
// //     void AddPlayer(string playerName)
// //     {
// //         Debug.Log("AddPlayer RPC called with player name: " + playerName);

// //         // Check for empty or null player name
// //         if (string.IsNullOrEmpty(playerName))
// //         {
// //             Debug.LogWarning("Received null or empty player name in AddPlayer RPC.");
// //             return;
// //         }

// //         // Check if playerName already exists locally or in Photon player list
// //         if (playerNames.Contains(playerName) || IsPlayerInPhotonList(playerName) || playerName == PhotonNetwork.MasterClient.NickName)
// //         {
// //             Debug.Log("Player name already exists: " + playerName);
// //             return;
// //         }

// //         // Add the player name to the local list
// //         playerNames.Add(playerName);
// //         Debug.Log("Added player name to local list: " + playerName);

// //         // Update player list UI
// //         UpdatePlayerList();

// //         // Find CollaborateGameManager and add player name to its list
// //         var collaborateGameManager = FindObjectOfType<CollaborateGameManager>();
// //         if (collaborateGameManager != null)
// //         {
// //             collaborateGameManager.AddPlayerNameToList(playerName);
// //             Debug.Log("Added player name to CollaborateGameManager: " + playerName);
// //         }
// //     }

// //     void UpdatePlayerList()
// //     {
// //         Debug.Log("Updating player list...");
// //         playerListText.text = "Players:\n";

// //         // Add host's name (master client)
// //         playerListText.text += PhotonNetwork.MasterClient.NickName + "\n";

// //         // Add PhotonNetwork players (excluding the host)
// //         foreach (Player player in PhotonNetwork.PlayerListOthers)
// //         {
// //             playerListText.text += player.NickName + "\n";
// //             Debug.Log("Added player from PhotonNetwork: " + player.NickName);
// //         }

// //         // Add local player names
// //         foreach (string playerName in playerNames)
// //         {
// //             if (!IsPlayerInPhotonList(playerName))
// //             {
// //                 playerListText.text += playerName + "\n";
// //                 Debug.Log("Added local player name: " + playerName);
// //             }
// //         }

// //         Debug.Log("Updated player list: " + playerListText.text);
// //     }

// //     bool IsPlayerInPhotonList(string playerName)
// //     {
// //         foreach (Player player in PhotonNetwork.PlayerList)
// //         {
// //             if (player.NickName == playerName)
// //             {
// //                 return true;
// //             }
// //         }
// //         return false;
// //     }

// //     public void OnAddPlayerButtonClicked()
// // {
// //     string playerName = addPlayerInput.text.Trim();

// //     // Check if playerName is empty or null only when the button is clicked
// //     if (string.IsNullOrEmpty(playerName))
// //     {
// //         Debug.Log("Player name is empty or null.");
// //         return;
// //     }

// //     // Check if playerName already exists
// //     if (playerNames.Contains(playerName) || IsPlayerInPhotonList(playerName) || playerName == PhotonNetwork.MasterClient.NickName)
// //     {
// //         Debug.Log("Player name already exists: " + playerName);
// //         return;
// //     }

// //     Debug.Log("Attempting to add player with name: " + playerName);

// //      // Call RPC to add player across network
// //     photonView.RPC("AddPlayer", RpcTarget.All, playerName);

// //     // Disable the button while processing
// //     addPlayerButton.interactable = false;

// //     // Clear input field and activate it again
// //     addPlayerInput.text = "";
// //     addPlayerInput.ActivateInputField();

// //     // Re-enable the button after the process
// //     addPlayerButton.interactable = true;
// // }


// //     public void OnStartGameButtonClicked()
// //     {
// //         Debug.Log("Start Game button clicked");
// //         addPlayerInput.gameObject.SetActive(false);
// //         addPlayerButton.gameObject.SetActive(false);
// //         startGameButton.gameObject.SetActive(false);

// //         collaborateHumanButton.gameObject.SetActive(true);
// //         collaborateRobotButton.gameObject.SetActive(true);
// //         competeModeButton.gameObject.SetActive(true);

// //         // Set listeners for mode buttons
// //         collaborateHumanButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateHumanMC"));
// //         collaborateRobotButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateRobotMC"));
// //         competeModeButton.onClick.AddListener(() => OnModeButtonClicked("CompeteMode"));
// //     }

// //     public void OnLeaveRoomButtonClicked()
// //     {
// //         Debug.Log("Leave Room button clicked");
// //         playerNames.Remove(PhotonNetwork.NickName);
// //         UpdatePlayerList();

// //         // Leave room using Photon
// //         PhotonNetwork.LeaveRoom();
// //     }

// //     public override void OnLeftRoom()
// //     {
// //         // Load scene after leaving room
// //         UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
// //     }

// //     public void OnModeButtonClicked(string mode)
// //     {
// //         Debug.Log("Mode button clicked: " + mode);
// //         photonView.RPC("LoadGameMode", RpcTarget.All, mode);
// //     }

// //     [PunRPC]
// //     public void LoadGameMode(string mode)
// //     {
// //         Debug.Log("Loading game mode: " + mode);
// //         PhotonNetwork.LoadLevel(mode);
// //     }

// //     public override void OnPlayerEnteredRoom(Player newPlayer)
// //     {
// //         Debug.Log("Player entered room: " + newPlayer.NickName);
// //         UpdatePlayerList();
// //     }

// //     public override void OnPlayerLeftRoom(Player otherPlayer)
// //     {
// //         Debug.Log("Player left room: " + otherPlayer.NickName);
// //         UpdatePlayerList();
// //     }

// //     public List<string> GetPlayerNames()
// //     {
// //         return playerNames;
// //     }
// // }







// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Pun;
// using Photon.Realtime;
// using System.Collections;
// using System.Collections.Generic;

// public class WaitingRoomManager : MonoBehaviourPunCallbacks
// {
//     public Text roomCodeText;
//     public Text playerListText;
//     public InputField addPlayerInput;
//     public Button addPlayerButton;
//     public Button startGameButton;
//     public Button leaveRoomButton;
//     public Button collaborateHumanButton;
//     public Button collaborateRobotButton;
//     public Button competeModeButton;

//     private List<string> playerNames = new List<string>();

//     void Start()
//     {
//         // Ensure all required components are assigned
//         if (!roomCodeText || !playerListText || !addPlayerInput || !addPlayerButton ||
//             !startGameButton || !leaveRoomButton || !collaborateHumanButton ||
//             !collaborateRobotButton || !competeModeButton)
//         {
//             Debug.LogError("Some required components are not assigned in the Inspector.");
//             return;
//         }

//         // Display room code
//         roomCodeText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;

//         // Update player list initially
//         UpdatePlayerList();

//         // Setup UI and event listeners based on Master Client or regular player
//         if (PhotonNetwork.IsMasterClient)
//         {
//             addPlayerButton.onClick.AddListener(OnAddPlayerButtonClicked);
//             startGameButton.onClick.AddListener(OnStartGameButtonClicked);
//             leaveRoomButton.gameObject.SetActive(false);
//         }
//         else
//         {
//             addPlayerInput.gameObject.SetActive(false);
//             addPlayerButton.gameObject.SetActive(false);
//             startGameButton.gameObject.SetActive(false);
//             leaveRoomButton.gameObject.SetActive(true);
//             leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
//         }

//         // Hide mode buttons initially
//         collaborateHumanButton.gameObject.SetActive(false);
//         collaborateRobotButton.gameObject.SetActive(false);
//         competeModeButton.gameObject.SetActive(false);

//         // Add existing names from ClientContent.Instance().allName
//         foreach (var name in ClientContent.Instance().allName)
//         {
//             AddPlayer(name);
//         }
//     }

//     [PunRPC]
//     void AddPlayer(string playerName)
//     {
//         Debug.Log("AddPlayer RPC called with player name: " + playerName);

//         // Check for empty or null player name
//         if (string.IsNullOrEmpty(playerName))
//         {
//             Debug.LogWarning("Received null or empty player name in AddPlayer RPC.");
//             return;
//         }

//         // Check if playerName already exists locally or in Photon player list
//         if (playerNames.Contains(playerName) || IsPlayerInPhotonList(playerName) || playerName == PhotonNetwork.MasterClient.NickName)
//         {
//             Debug.Log("Player name already exists: " + playerName);
//             return;
//         }

//         // Add the player name to the local list
//         playerNames.Add(playerName);
//         Debug.Log("Added player name to local list: " + playerName);

//         // Update ClientContent.Instance().allName if not already present
//         if (!ClientContent.Instance().allName.Contains(playerName))
//         {
//             ClientContent.Instance().allName.Add(playerName);
//             Debug.Log("Added player name to ClientContent.Instance().allName: " + playerName);
//         }

//         // Update player list UI
//         UpdatePlayerList();

//         // Find CollaborateGameManager and add player name to its list
//         var collaborateGameManager = FindObjectOfType<CollaborateGameManager>();
//         if (collaborateGameManager != null)
//         {
//             collaborateGameManager.AddPlayerNameToList(playerName);
//             Debug.Log("Added player name to CollaborateGameManager: " + playerName);
//         }
//     }

//     void UpdatePlayerList()
//     {
//         Debug.Log("Updating player list...");
//         playerListText.text = "Players:\n";

//         // Add host's name (master client)
//         if (PhotonNetwork.IsMasterClient)
//         {
//             playerListText.text += "(Host) " + PhotonNetwork.MasterClient.NickName + "\n";
//         }
//         else
//         {
//             playerListText.text += PhotonNetwork.MasterClient.NickName + "\n";
//         }

//         // Add PhotonNetwork players (excluding the host)
//         foreach (Player player in PhotonNetwork.PlayerListOthers)
//         {
//             playerListText.text += player.NickName + "\n";
//             Debug.Log("Added player from PhotonNetwork: " + player.NickName);
//         }

//         // Add local player names
//         foreach (string playerName in playerNames)
//         {
//             if (!IsPlayerInPhotonList(playerName) && playerName != PhotonNetwork.MasterClient.NickName)
//             {
//                 playerListText.text += playerName + "\n";
//                 Debug.Log("Added local player name: " + playerName);
//             }
//         }

//         Debug.Log("Updated player list: " + playerListText.text);
//     }

//     bool IsPlayerInPhotonList(string playerName)
//     {
//         foreach (Player player in PhotonNetwork.PlayerList)
//         {
//             if (player.NickName == playerName)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     public void OnAddPlayerButtonClicked()
//     {
//         string playerName = addPlayerInput.text.Trim();

//         // Check if playerName is empty or null only when the button is clicked
//         if (string.IsNullOrEmpty(playerName))
//         {
//             Debug.Log("Player name is empty or null.");
//             return;
//         }

//         // Check if playerName already exists
//         if (playerNames.Contains(playerName) || IsPlayerInPhotonList(playerName) || playerName == PhotonNetwork.MasterClient.NickName)
//         {
//             Debug.Log("Player name already exists: " + playerName);
//             return;
//         }

//         Debug.Log("Attempting to add player with name: " + playerName);

//         // Call RPC to add player across network
//         photonView.RPC("AddPlayer", RpcTarget.All, playerName);

//         // Disable the button while processing
//         addPlayerButton.interactable = false;

//         // Clear input field and activate it again
//         addPlayerInput.text = "";
//         addPlayerInput.ActivateInputField();

//         // Re-enable the button after the process
//         addPlayerButton.interactable = true;
//     }

//     public void OnStartGameButtonClicked()
//     {
//         Debug.Log("Start Game button clicked");
//         addPlayerInput.gameObject.SetActive(false);
//         addPlayerButton.gameObject.SetActive(false);
//         startGameButton.gameObject.SetActive(false);

//         collaborateHumanButton.gameObject.SetActive(true);
//         collaborateRobotButton.gameObject.SetActive(true);
//         competeModeButton.gameObject.SetActive(true);

//         // Set listeners for mode buttons
//         collaborateHumanButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateHumanMC"));
//         collaborateRobotButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateRobotMC"));
//         competeModeButton.onClick.AddListener(() => OnModeButtonClicked("CompeteMode"));
//     }

//     public void OnLeaveRoomButtonClicked()
//     {
//         Debug.Log("Leave Room button clicked");
//         playerNames.Remove(PhotonNetwork.NickName);
//         UpdatePlayerList();

//         // Leave room using Photon
//         PhotonNetwork.LeaveRoom();
//     }

//     public override void OnLeftRoom()
//     {
//         // Load scene after leaving room
//         UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
//     }

//     public void OnModeButtonClicked(string mode)
//     {
//         Debug.Log("Mode button clicked: " + mode);
//         photonView.RPC("LoadGameMode", RpcTarget.All, mode);
//     }

//     [PunRPC]
//     public void LoadGameMode(string mode)
//     {
//         Debug.Log("Loading game mode: " + mode);
//         PhotonNetwork.LoadLevel(mode);
//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         Debug.Log("Player entered room: " + newPlayer.NickName);
//         UpdatePlayerList();
//     }

//     public override void OnPlayerLeftRoom(Player otherPlayer)
//     {
//         Debug.Log("Player left room: " + otherPlayer.NickName);
//         UpdatePlayerList();
//     }

//     public List<string> GetPlayerNames()
//     {
//         return playerNames;
//     }
// }
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
public Text roomCodeText;
public Text playerListText;
public InputField addPlayerInput;
public Button addPlayerButton;
public Button startGameButton;
public Button leaveRoomButton;


public Button collaborateHumanButton;
public Button collaborateRobotButton;
public Button competeModeButton;

private List<string> playerNames = new List<string>();

void Start()

{
// Ensure all required components are assigned
if (!roomCodeText || !playerListText || !addPlayerInput || !addPlayerButton ||
!startGameButton || !leaveRoomButton || !collaborateHumanButton ||
!collaborateRobotButton || !competeModeButton)
{
Debug.LogError("Some required components are not assigned in the Inspector.");
return;
}

// Display room code
roomCodeText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;

// Update player list initially
UpdatePlayerList();

// Setup UI and event listeners based on Master Client or regular player
if (PhotonNetwork.IsMasterClient)
{
    addPlayerButton.onClick.AddListener(OnAddPlayerButtonClicked);
    startGameButton.onClick.AddListener(OnStartGameButtonClicked);
    leaveRoomButton.gameObject.SetActive(false);
}
else
{
    addPlayerInput.gameObject.SetActive(false);
    addPlayerButton.gameObject.SetActive(false);
    startGameButton.gameObject.SetActive(false);
    leaveRoomButton.gameObject.SetActive(true);
    leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
}

// Hide mode buttons initially
collaborateHumanButton.gameObject.SetActive(false);
collaborateRobotButton.gameObject.SetActive(false);
competeModeButton.gameObject.SetActive(false);
UpdatePlayerList();

}

void UpdatePlayerList()
{
    Debug.Log("Updating player list...");
    playerListText.text = "Players:\n";
    ClientContent.Instance().allName.Clear();
    // Add host's name (master client)
    playerListText.text += (ClientContent.Instance().name + "\n");
    ClientContent.Instance().allName.Add(ClientContent.Instance().name);
        Debug.Log("Added host's name: " + PhotonNetwork.MasterClient.NickName);

    // Add PhotonNetwork players (excluding the host)
    foreach (Player player in PhotonNetwork.PlayerListOthers)
    {
        playerListText.text += (player.NickName + "\n");
            ClientContent.Instance().allName.Add(player.NickName);
        Debug.Log("Added player from PhotonNetwork: " + player.NickName);
    }

    // Add local player names
    foreach (string playerName in playerNames)
    {
        // Skip adding the host's name again (already added above)
        if (playerName != PhotonNetwork.MasterClient.NickName && !IsPlayerInPhotonList(playerName))
        {
            playerListText.text += (playerName + "\n");
            Debug.Log("Added local player name: " + playerName);
        }
    }

    Debug.Log("Updated player list: " + playerListText.text);
}


bool IsPlayerInPhotonList(string playerName)
{
    foreach (Player player in PhotonNetwork.PlayerListOthers)
    {
        if (player.NickName == playerName)
        {
            return true;
        }
    }
    return false;
}

public void OnAddPlayerButtonClicked()
{
    
    string playerName = addPlayerInput.text.Trim();
    
   
        Debug.Log("Attempting to add player with name: " + playerName);

    // Disable the button while processing
    //addPlayerButton.interactable = false;

    // Check if playerName already exists
    if (playerNames.Contains(playerName))
    {
        Debug.Log("Player name already exists in the local list: " + playerName);
        //addPlayerButton.interactable = true; // Re-enable button
        return;
    }

    

    // Clear input field and activate it again
    addPlayerInput.text = "";
    addPlayerInput.ActivateInputField();

    // Call RPC to add player across network
    photonView.RPC("AddPlayer", RpcTarget.All, playerName);
}


[PunRPC]
void AddPlayer(string playerName)
{
    Debug.Log("AddPlayer RPC called with player name: " + playerName);

    // Check for empty or null player name
    if (string.IsNullOrEmpty(playerName))
    {
        Debug.LogWarning("Received null or empty player name in AddPlayer RPC.");
        return;
    }

    // Check if playerName already exists locally
    if (playerNames.Contains(playerName))
    {
        Debug.Log("Player name already exists in the local list: " + playerName);
        return;
    }
        ClientContent.allNameString.Add(playerName);
        ClientContent.Instance().allName.Add(playerName);
        // Check if playerName exists in PhotonNetwork.PlayerList
        bool playerExistsInPhoton = false;
    bool hostExistsInPhoton = false;
    foreach (Player player in PhotonNetwork.PlayerList)
    {
        if (player.NickName == playerName)
        {
            playerExistsInPhoton = true;
            break;
        }
    }

 if (PhotonNetwork.MasterClient != null && PhotonNetwork.MasterClient.NickName == playerName)
{
    hostExistsInPhoton = true;
}

    // If playerName does not exist in either local or Photon player list, add it
    if (!playerExistsInPhoton || !hostExistsInPhoton)
    {
        playerNames.Add(playerName);
        Debug.Log("Added player name to local list: " + playerName);

        // Update player list UI
        UpdatePlayerList();

        // Find CollaborateGameManager and add player name to its list
        var collaborateGameManager = FindObjectOfType<CollaborateGameManager>();
        if (collaborateGameManager != null)
        {
            collaborateGameManager.AddPlayerNameToList(playerName);
            Debug.Log("Added player name to CollaborateGameManager: " + playerName);
        }
    }
    else
    {

        Debug.Log("Player name already exists in Photon player list: " + playerName);
    }
}



public void OnStartGameButtonClicked()
{
Debug.Log("Start Game button clicked");
addPlayerInput.gameObject.SetActive(false);
addPlayerButton.gameObject.SetActive(false);
startGameButton.gameObject.SetActive(false);


collaborateHumanButton.gameObject.SetActive(true);
collaborateRobotButton.gameObject.SetActive(true);
competeModeButton.gameObject.SetActive(true);

// Set listeners for mode buttons
collaborateHumanButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateHumanMC"));
collaborateRobotButton.onClick.AddListener(() => OnModeButtonClicked("CollaborateRobotMC"));
competeModeButton.onClick.AddListener(() => OnModeButtonClicked("CompeteMode"));
}

public void OnLeaveRoomButtonClicked()
{
Debug.Log("Leave Room button clicked");
playerNames.Remove(PhotonNetwork.NickName);
UpdatePlayerList();


// Leave room using Photon
PhotonNetwork.LeaveRoom();
}

public override void OnLeftRoom()
{
// Load scene after leaving room
UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
}

public void OnModeButtonClicked(string mode)
{
Debug.Log("Mode button clicked: " + mode);
photonView.RPC("LoadGameMode", RpcTarget.All, mode);
}

[PunRPC]
public void LoadGameMode(string mode)
{
Debug.Log("Loading game mode: " + mode);
PhotonNetwork.LoadLevel(mode);
}

public override void OnPlayerEnteredRoom(Player newPlayer)
{
Debug.Log("Player entered room: " + newPlayer.NickName);
UpdatePlayerList();
}

public override void OnPlayerLeftRoom(Player otherPlayer)
{
Debug.Log("Player left room: " + otherPlayer.NickName);
UpdatePlayerList();
}

public List<string> GetPlayerNames()
{
return playerNames;
}
}