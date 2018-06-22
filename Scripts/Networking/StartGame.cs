using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseDown()
    {
        //if (PhotonNetwork.playerList.Length > 4 && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
            PhotonNetwork.automaticallySyncScene = true;
        }
    }
}
