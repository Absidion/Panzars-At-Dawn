using UnityEngine;
using System.Collections;

public class PlayerMovement : Photon.MonoBehaviour {

    CharacterController characterController;
    public float MaxStunnedTime;
    public float MaxDeadTime;
    public float MoveSpeed = 5;

    private float DeathTimer;
    private float StunTimer;
    private bool isStunned = false;
    private bool isDead = false;
    private Vector3 RespawnLocation;

    private Vector3 forwardDirection = Vector3.zero;

    private float gravity = 9.8f;

    //Used for moving faster
    private float RunSpeed;

    //If player is on the ground, they can player
    private bool grounded;

    // Use this for initialization
    void Start () {
        characterController = GetComponent<CharacterController>();
        StunTimer = MaxStunnedTime;
        DeathTimer = MaxDeadTime;
    }
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    RunSpeed = MoveSpeed * 2;
                else
                    RunSpeed = MoveSpeed;
            }

            else if (Input.GetAxis("Vertical") < -0.1f)
            {
                RunSpeed = MoveSpeed / 2;
            }
        }

    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            if (StunTimer < 0)
            {
                StunTimer = MaxStunnedTime;
                isStunned = false;
                
            }
            if (DeathTimer < 0)
            {
                transform.position = RespawnLocation;
                DeathTimer = 10;
                isDead = false;
                GetComponentInChildren<ScoutUI>().Reset();
            }

            if (grounded)
            {
                if (isStunned)
                {
                    transform.position = new Vector3(transform.position.x + (Random.insideUnitCircle.x * 0.05f), transform.position.y, transform.position.z);
                    StunTimer -= Time.deltaTime;
                }
                else if (isDead)
                {
                    transform.position = new Vector3(transform.position.x + (Random.insideUnitCircle.x * 0.05f), transform.position.y, transform.position.z);
                    DeathTimer -= Time.deltaTime;
                }
                else
                {
                    float yAxis = Input.GetAxis("Horizontal") * 2;
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + yAxis,
                        transform.eulerAngles.z);


                    forwardDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
                    forwardDirection = transform.TransformDirection(forwardDirection);
                }
            }

            forwardDirection.y -= gravity * Time.deltaTime;
            grounded = (characterController.Move(forwardDirection *
                        (Time.deltaTime * RunSpeed)) & CollisionFlags.Below) != 0;
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
    public void SetDead(bool flag)
    {
        isDead = flag;
    }

    public void SetStunned(bool flag)
    {
        isStunned = flag;
    }

    public void SetRespawnLocation(Vector3 loc)
    {
        RespawnLocation = loc;
    }
}

