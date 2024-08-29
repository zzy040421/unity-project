using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public Button returnButton; // Reference to the Return button

    void Start()
    {

        returnButton.onClick.AddListener(ReturnToLobby);
    }

    void ReturnToLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
    }


    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

}
