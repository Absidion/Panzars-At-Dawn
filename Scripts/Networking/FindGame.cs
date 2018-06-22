using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FindGame : Photon.MonoBehaviour {

    public GameObject Lobby;
    public SharedVariables saveObject;

    // Use this for initialization
    void Start () {

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("0.3");
        }
    }
    public void OnMouseDown()
    {
        saveObject.SelectedRole = Lobby.GetComponentInChildren<Dropdown>().captionText.text;
        PhotonNetwork.JoinRandomRoom();
    }
}
