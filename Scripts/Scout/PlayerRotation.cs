using UnityEngine;
using System.Collections;

public class PlayerRotation : Photon.MonoBehaviour {

    public bool RotateX;
    public bool RotateY;
    public float RotationSpeed;
    private Vector3 CurrentRot = new Vector3(0, 0, 0);
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (photonView.isMine)
        {
            if (RotateX)
            {
                CurrentRot.y += (Input.GetAxis("Mouse X") * RotationSpeed);
            }

            if (RotateY)
            {
                CurrentRot.x -= Input.GetAxis("Mouse Y") * RotationSpeed;
                CurrentRot.x = Mathf.Clamp(CurrentRot.x, -25.0f, 7.5f);
            }

            this.transform.eulerAngles = CurrentRot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            if (photonView.isMine)
            {
                // We own this player: send the others our data
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
        }
        else if (stream.isReading)
        {
            if (!photonView.isMine)
            {
                // Network player, receive data
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

}
