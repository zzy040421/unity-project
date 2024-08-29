// using Photon.Pun;
// using Photon.Realtime;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class JoinRoomManager : MonoBehaviourPunCallbacks
// {
//     public InputField roomCodeInput;
//     public InputField playerNameInput;
//     public Button submitRoomCodeButton;
//     public Button submitNameButton;
//     public Text errorText;

//     public static string roomToJoin;

//     void Start()
//     {
//         submitRoomCodeButton.onClick.AddListener(OnSubmitRoomCodeButtonClicked);
//         submitNameButton.onClick.AddListener(OnSubmitNameButtonClicked);

//         // Initially hide the player name input field, submit button, and error text
//         playerNameInput.gameObject.SetActive(false);
//         submitNameButton.gameObject.SetActive(false);
//         errorText.gameObject.SetActive(false);

//         if (!PhotonNetwork.IsConnected)
//         {
//             Debug.Log("Connecting to Photon...");
//             PhotonNetwork.ConnectUsingSettings();
//         }
//     }

//     public void OnSubmitRoomCodeButtonClicked()
//     {
//         Debug.Log("Submit Room Code button clicked");

//         if (roomCodeInput == null)
//         {
//             Debug.LogError("RoomCodeInput is not assigned.");
//             return;
//         }

//         roomToJoin = roomCodeInput.text; // Get the room code from the input field

//         Debug.Log("Room code entered: " + roomToJoin);

//         if (string.IsNullOrEmpty(roomToJoin))
//         {
//             Debug.LogError("Room code is empty. Please enter a room code.");
//             errorText.text = "Room code is empty. Please enter a room code.";
//             errorText.gameObject.SetActive(true);
//             return;
//         }

//         // Hide room code input field and submit button
//         roomCodeInput.gameObject.SetActive(false);
//         submitRoomCodeButton.gameObject.SetActive(false);

//         // Show player name input field and submit button
//         playerNameInput.gameObject.SetActive(true);
//         submitNameButton.gameObject.SetActive(true);
//         errorText.gameObject.SetActive(false); // Hide error text
//     }

//     public void OnSubmitNameButtonClicked()
//     {
//         Debug.Log("Submit Name button clicked");

//         if (playerNameInput == null)
//         {
//             Debug.LogError("PlayerNameInput is not assigned.");
//             return;
//         }

//         string playerName = playerNameInput.text;
//         if (string.IsNullOrEmpty(playerName))
//         {
//             Debug.LogError("Player name is empty. Please enter a name.");
//             errorText.text = "Player name is empty. Please enter a name.";
//             errorText.gameObject.SetActive(true);
//             return;
//         }

//         PhotonNetwork.NickName = playerName;

//         if (PhotonNetwork.IsConnectedAndReady)
//         {
//             Debug.Log("Connected and ready. Attempting to join room: " + roomToJoin);
//             //PhotonNetwork.JoinRoom(roomToJoin);
//             UnityEngine.SceneManagement.SceneManager.LoadScene("UserWaitingRoom");

//         }
//         else
//         {
//             StartCoroutine(ConnectAndJoinRoom(playerName));
//         }
//     }

//     private IEnumerator ConnectAndJoinRoom(string playerName)
//     {
//         // Ensure we are connected to the Photon network
//         if (!PhotonNetwork.IsConnected)
//         {
//             PhotonNetwork.ConnectUsingSettings();
//         }

//         // Wait until connected to Photon
//         while (!PhotonNetwork.IsConnectedAndReady)
//         {
//             yield return null;
//         }

//         // Once connected, join the room
//         PhotonNetwork.NickName = playerName;
//         Debug.Log("Connected to Photon. Joining room: " + roomToJoin);
//         UnityEngine.SceneManagement.SceneManager.LoadScene("UserWaitingRoom");
//         //PhotonNetwork.JoinRoom(roomToJoin);
//     }

//     public override void OnJoinedRoom()
//     {
//         Debug.Log("Joined room successfully. Loading UserWaitingRoom scene.");
//         // Load the Waiting Room scene when a room is joined
//         UnityEngine.SceneManagement.SceneManager.LoadScene("UserWaitingRoom");
//     }

//     public override void OnDisconnected(DisconnectCause cause)
//     {
//         Debug.LogError("Disconnected from Photon with reason " + cause.ToString());
//         errorText.text = "Disconnected from Photon with reason: " + cause.ToString();
//         errorText.gameObject.SetActive(true);
//     }

//     public override void OnJoinRoomFailed(short returnCode, string message)
//     {
//         Debug.LogError("Join room failed: " + message);
//         errorText.text = "Join room failed: " + message;
//         errorText.gameObject.SetActive(true);
//     }
// }
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomManager : MonoBehaviourPunCallbacks
{
    public InputField roomCodeInput;
    public InputField playerNameInput;
    public Button submitRoomCodeButton;
    public Button submitNameButton;
    public Text errorText;

    private string roomToJoin;

    void Start()
    {
        submitRoomCodeButton.onClick.AddListener(OnSubmitRoomCodeButtonClicked);
        submitNameButton.onClick.AddListener(OnSubmitNameButtonClicked);

        // Initially hide the player name input field, submit button, and error text
        playerNameInput.gameObject.SetActive(false);
        submitNameButton.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

    public void OnSubmitRoomCodeButtonClicked()
    {
        Debug.Log("Submit Room Code button clicked");

        if (roomCodeInput == null)
        {
            Debug.LogError("RoomCodeInput is not assigned.");
            return;
        }

        roomToJoin = roomCodeInput.text; // Get the room code from the input field

        Debug.Log("Room code entered: " + roomToJoin);

        if (string.IsNullOrEmpty(roomToJoin))
        {
            Debug.LogError("Room code is empty. Please enter a room code.");
            errorText.text = "Room code is empty. Please enter a room code.";
            errorText.gameObject.SetActive(true);
            return;
        }

        // Hide room code input field and submit button
        roomCodeInput.gameObject.SetActive(false);
        submitRoomCodeButton.gameObject.SetActive(false);

        // Show player name input field and submit button
        playerNameInput.gameObject.SetActive(true);
        submitNameButton.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false); // Hide error text
    }

    public void OnSubmitNameButtonClicked()
    {
        Debug.Log("Submit Name button clicked");

        if (playerNameInput == null)
        {
            Debug.LogError("PlayerNameInput is not assigned.");
            return;
        }

        string playerName = playerNameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name is empty. Please enter a name.");
            errorText.text = "Player name is empty. Please enter a name.";
            errorText.gameObject.SetActive(true);
            return;
        }

        PhotonNetwork.NickName = playerName;
        ClientContent.Instance().name = playerName;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(roomToJoin);
        }
        else
        {
            Debug.LogError("Not connected to Photon. Please check your connection.");
            errorText.gameObject.SetActive(false);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully. Loading WaitingRoom scene.");
        // Load the Waiting Room scene when a room is joined
        UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoom");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon with reason " + cause.ToString());
        errorText.text = "Disconnected from Photon with reason: " + cause.ToString();
        errorText.gameObject.SetActive(true);
    }
}
