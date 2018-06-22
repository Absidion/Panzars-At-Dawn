using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JoinLobby : Photon.MonoBehaviour {

    public InputField Input;

    // Use this for initialization
    void Start () {
            
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnMouseDown()
    {
        PhotonNetwork.player.NickName = Input.text;

        PhotonNetwork.JoinLobby();
    }
}
