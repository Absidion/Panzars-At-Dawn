using UnityEngine;
using System.Collections;

public class EnableScoutCamera : MonoBehaviour {

    private PhotonView myPhotonView;
    // Use this for initialization
    void Start () {
        myPhotonView = gameObject.GetComponent<PhotonView>();

    }
	
	// Update is called once per frame
	void Update () {
        if (myPhotonView.isMine)
        {
            ScoutCamera cameraScript = Camera.main.GetComponent<ScoutCamera>();
            cameraScript.OnStartFollowing();
        }
    }
}
