// using Photon.Pun;
// using Photon.Realtime;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class LobbyManager : MonoBehaviourPunCallbacks
// {
// public InputField playerNameInput;
// public Button submitNameButton;
// public Button createRoomButton;
// public Button joinRoomButton;


// private bool isConnecting;
// private string roomToJoin;

// void Start()
// {
//     Debug.Log("Initializing LobbyManager...");


//     // Add listeners to buttons
//     submitNameButton.onClick.AddListener(OnSubmitNameButtonClicked);
//     createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
//     joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);

//     // Initially hide the Submit Name button and input field
//     playerNameInput.gameObject.SetActive(false);
//     submitNameButton.gameObject.SetActive(false);

//     Debug.Log("Assigning buttons and inputs...");
//     Debug.Log("playerNameInput: " + playerNameInput);
//     Debug.Log("submitNameButton: " + submitNameButton);
//     Debug.Log("createRoomButton: " + createRoomButton);
//     Debug.Log("joinRoomButton: " + joinRoomButton);

//     // Ensure we are connected to the Photon network
//     if (!PhotonNetwork.IsConnected)
//     {
//         PhotonNetwork.ConnectUsingSettings();
//     }
// }

// public override void OnConnectedToMaster()
// {
//     Debug.Log("Connected to Master");
//     if (isConnecting)
//     {
//         if (!string.IsNullOrEmpty(roomToJoin))
//         {
//             PhotonNetwork.JoinRoom(roomToJoin);
//         }
//     else
//     {
//         CreateRoom();
//     }
//     isConnecting = false;
// }
// }

// void OnCreateRoomButtonClicked()
// {
//     Debug.Log("Create Room button clicked");


//     // Hide Create Room and Join Room buttons
//     createRoomButton.gameObject.SetActive(false);
//     joinRoomButton.gameObject.SetActive(false);

//     // Show Submit Name button and input field
//     playerNameInput.gameObject.SetActive(true);
//     submitNameButton.gameObject.SetActive(true);
// }

// void OnSubmitNameButtonClicked()
// {
//     Debug.Log("Submit Name button clicked");


//     string playerName = playerNameInput.text;

//     if (string.IsNullOrEmpty(playerName))
//     {
//         Debug.LogError("Player name is empty. Please enter a name.");
//         return;
// }

// PhotonNetwork.NickName = playerName;
// ClientContent.Instance().name = playerNameInput.text;
// if (PhotonNetwork.IsConnectedAndReady)
// {
//     CreateRoom();
// }
// else if (!PhotonNetwork.IsConnected)
// {
//     isConnecting = true;
//     PhotonNetwork.ConnectUsingSettings();
// }
// }

// void CreateRoom()
// {
//     string roomCode = GenerateRoomCode();
//     Debug.Log("Creating room with code: " + roomCode);
//     PhotonNetwork.CreateRoom(roomCode, new RoomOptions { MaxPlayers = 4 });
// }

// string GenerateRoomCode()
// {
//     System.Random random = new System.Random();
//     int roomCode = random.Next(1000, 10000); // Generate a 4-digit numeric room code
//     return roomCode.ToString();
// }

// void OnJoinRoomButtonClicked()
// {
//     Debug.Log("Join Room button clicked");


//     // Load the Join Room scene
//     UnityEngine.SceneManagement.SceneManager.LoadScene("JoinRoom");
//     }

//     public override void OnJoinedRoom()
//     {
//     Debug.Log("Joined room successfully. Loading WaitingRoom scene.");
//     // Load the Waiting Room scene when a room is joined or created
//     UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoom");
// }

// public override void OnDisconnected(DisconnectCause cause)
// {
//     Debug.LogError("Disconnected from Photon with reason " + cause.ToString());
//     isConnecting = false;
// }

// public string GetCode()
//     {
//         return roomToJoin;
//     }

// }

// public class ClientContent
// {
//     private static ClientContent instance;
//     public static List<string> allNameString = new List<string>();
//     public static ClientContent Instance()
//     {
//         if (instance == null)
//             instance = new ClientContent();
//         return instance;
//     }
//     public string name;
//     public List<string> allName = new List<string>();
// }
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
public InputField playerNameInput;
public Button submitNameButton;
public Button createRoomButton;
public Button joinRoomButton;


private bool isConnecting;
private string roomToJoin;

void Start()
{
Debug.Log("Initializing LobbyManager...");


// Add listeners to buttons
submitNameButton.onClick.AddListener(OnSubmitNameButtonClicked);
createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);

// Initially hide the Submit Name button and input field
playerNameInput.gameObject.SetActive(false);
submitNameButton.gameObject.SetActive(false);

Debug.Log("Assigning buttons and inputs...");
Debug.Log("playerNameInput: " + playerNameInput);
Debug.Log("submitNameButton: " + submitNameButton);
Debug.Log("createRoomButton: " + createRoomButton);
Debug.Log("joinRoomButton: " + joinRoomButton);

// Ensure we are connected to the Photon network
if (!PhotonNetwork.IsConnected)
{
    PhotonNetwork.ConnectUsingSettings();
}
}

public override void OnConnectedToMaster()
{
Debug.Log("Connected to Master");
if (isConnecting)
{
if (!string.IsNullOrEmpty(roomToJoin))
{
PhotonNetwork.JoinRoom(roomToJoin);
}
else
{
CreateRoom();
}
isConnecting = false;
}
}

void OnCreateRoomButtonClicked()
{
Debug.Log("Create Room button clicked");


// Hide Create Room and Join Room buttons
createRoomButton.gameObject.SetActive(false);
joinRoomButton.gameObject.SetActive(false);

// Show Submit Name button and input field
playerNameInput.gameObject.SetActive(true);
submitNameButton.gameObject.SetActive(true);
}

void OnSubmitNameButtonClicked()
{
Debug.Log("Submit Name button clicked");


string playerName = playerNameInput.text;

if (string.IsNullOrEmpty(playerName))
{
    Debug.LogError("Player name is empty. Please enter a name.");
    return;
}

PhotonNetwork.NickName = playerName;
ClientContent.Instance().name = playerNameInput.text;
if (PhotonNetwork.IsConnectedAndReady)
{
    CreateRoom();
}
else if (!PhotonNetwork.IsConnected)
{
    isConnecting = true;
    PhotonNetwork.ConnectUsingSettings();
}
}

void CreateRoom()
{
string roomCode = GenerateRoomCode();
Debug.Log("Creating room with code: " + roomCode);
PhotonNetwork.CreateRoom(roomCode, new RoomOptions { MaxPlayers = 4 });
}

string GenerateRoomCode()
{
System.Random random = new System.Random();
int roomCode = random.Next(1000, 10000); // Generate a 4-digit numeric room code
return roomCode.ToString();
}

void OnJoinRoomButtonClicked()
{
Debug.Log("Join Room button clicked");


// Load the Join Room scene
UnityEngine.SceneManagement.SceneManager.LoadScene("JoinRoom");
}

public override void OnJoinedRoom()
{
Debug.Log("Joined room successfully. Loading WaitingRoom scene.");
// Load the Waiting Room scene when a room is joined or created
UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoom");
}

public override void OnDisconnected(DisconnectCause cause)
{
Debug.LogError("Disconnected from Photon with reason " + cause.ToString());
isConnecting = false;
}
}
public class ClientContent
{
    private static ClientContent instance;
    public static List<string> allNameString = new List<string>();
    public static ClientContent Instance()
    {
        if (instance == null)
            instance = new ClientContent();
        return instance;
    }
    public string name;
    public List<string> allName = new List<string>();
}