using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Photon.MonoBehaviour {

    public GameObject Title;
    public GameObject Lobby;
    public GameObject PreGameLobby;

    public SharedVariables saveObject;
    private PhotonPlayer[] PlayersCurrently;


    // Use this for initialization
    void Start ()
    {      
        Title.SetActive(true);
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("0.4");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (i == 0 || i == 2)
                {
                    PhotonNetwork.playerList[i].SetTeam(PunTeams.Team.red);
                }
                else if (i == 1 || i == 3)
                {
                    PhotonNetwork.playerList[i].SetTeam(PunTeams.Team.blue);
                }
            }
        }

    }

    void OnJoinedLobby()
    {
        Title.SetActive(false);
        Lobby.SetActive(true);
    }

    void OnJoinedRoom()
    {     
        Lobby.SetActive(false);
        PreGameLobby.SetActive(true);

        foreach (Text text in PreGameLobby.GetComponentsInChildren<Text>())
        {
            if (text.name == "Server Name: ")
            {
                text.text = ("Server Name: " + PhotonNetwork.room.Name);
            }

            if (text.name == "Host User: ")
            {
                text.text = ("Host Name: " + PhotonNetwork.masterClient.NickName.ToString());
            }
        }

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            PreGameLobby.GetComponentInChildren<ScrollRect>().viewport.GetComponentInChildren<Text>().text += (player.NickName + "\n");
        }

        saveObject.SelectedRole = Lobby.GetComponentInChildren<Dropdown>().captionText.text;
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        PreGameLobby.GetComponentInChildren<ScrollRect>().viewport.GetComponentInChildren<Text>().text += (newPlayer.NickName + "\n");       
    }

    void OnReceivedRoomListUpdate()
    {
        if (Lobby.GetActive() == true)
        {
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                Lobby.GetComponentInChildren<ScrollRect>().viewport.GetComponentInChildren<Text>().text = (game.Name + " " + game.PlayerCount + "/" + game.MaxPlayers);
            }
        }        
    }
}
