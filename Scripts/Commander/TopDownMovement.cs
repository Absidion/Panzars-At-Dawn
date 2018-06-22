using UnityEngine;
using System.Collections;

public class TopDownMovement : Photon.MonoBehaviour {
    public int scrollSpeed = 50;
    public float scrollZone = 30;
    public float zoomSpeed = 1;
    public float maxZoom = 10;

    private Vector3 position = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        if (photonView.isMine)
        {
            //reset camera position change
            position.x = 0;
            position.y = 0;
            position.z = 0;

            float speed = scrollSpeed * Time.deltaTime;

            //Move camera left
            if (Input.mousePosition.x < scrollZone)
            {
                position.x -= speed;
            }
            //Move camera right
            else if (Input.mousePosition.x > Screen.width - scrollZone)
            {
                position.x += speed;
            }

            //Move camera down
            if (Input.mousePosition.y < scrollZone)
            {
                position.z -= speed;
            }
            //Move camera up
            else if (Input.mousePosition.y > Screen.height - scrollZone)
            {
                position.z += speed;
            }

            //Zooming in and out
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && transform.position.y > 1)
            {
                position.y -= speed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.y < maxZoom)
            {
                position.y += speed;
            }


            //Update camera's position
            transform.position += position;
        }
    }
}
