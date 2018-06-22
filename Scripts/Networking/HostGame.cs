using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HostGame : Photon.MonoBehaviour {

    // Use this for initialization
    // Get Lobby
    public GameObject Lobby;
    private string serverName;
	void Start () {      
        PhotonNetwork.automaticallySyncScene = true;

    }
    public void OnMouseDown()
    {
        if (Lobby.GetComponentInChildren<InputField>().name == "Enter Server Name")
        {
            serverName = Lobby.GetComponentInChildren<InputField>().text.ToString();
        }

        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(serverName, room, null);

    }
}
