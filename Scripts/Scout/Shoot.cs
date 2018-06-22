
using UnityEngine;
using System.Collections;

public class Shoot : Photon.MonoBehaviour
{

    public GameObject[] Ammo;
    public int VelocityModifier;
    public float PrimaryCooldown;
    public float StunCooldown;
    public float PingCooldown;
    public string CurrentAmmoType;

    private GameObject PrimaryFire;   
    private GameObject bullet;
    private string tagName;

    // Use this for initialization
    void Start()
    {
        PrimaryFire = Ammo[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            PrimaryCooldown -= Time.deltaTime;
            StunCooldown -= Time.deltaTime;
            PingCooldown -= Time.deltaTime;
            if (PrimaryCooldown < 0)
            {
                PrimaryCooldown = 0;
            }
            if (StunCooldown < 0)
            {
                StunCooldown = 0;
            }
            if (PingCooldown < 0)
            {
                PingCooldown = 0;
            }
            if (Input.GetMouseButtonDown(0)) //0 for Left Mouse Button, 1 for Right Mouse Button, 2 for Middle Mouse Button
            {
                if (PrimaryCooldown <= 0 && PrimaryFire == Ammo[0])
                {
                    bullet = PhotonNetwork.Instantiate(PrimaryFire.name, transform.position + Vector3.up + new Vector3(0, 0.5f, 0), Quaternion.identity, 0);
                    if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
                    {
                        bullet.tag = "RedShell";                       
                    }
                    else if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
                    {
                        bullet.tag = "BlueShell";
                    }
                    bullet.GetPhotonView().RPC("SetTag", PhotonTargets.All, bullet.tag);
                    Debug.Log(PhotonNetwork.player.NickName + " " + PhotonNetwork.player.GetTeam() + " " + bullet.tag.ToString());

                    bullet.transform.rotation = transform.rotation;
                    bullet.GetComponent<Rigidbody>().AddForce(transform.forward * VelocityModifier);

                    PrimaryCooldown = 3.0f;
                }
                if (StunCooldown <= 0 && PrimaryFire == Ammo[1])
                {
                    bullet = PhotonNetwork.Instantiate(PrimaryFire.name, transform.position + Vector3.up + new Vector3(0, 0.5f, 0), Quaternion.identity, 0);

                    if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
                    {
                        bullet.tag = "RedStunShell";
                    }
                    else if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
                    {
                        bullet.tag = "BlueStunShell";
                    }
                    bullet.GetPhotonView().RPC("SetTag", PhotonTargets.All, bullet.tag);
                    Debug.Log(PhotonNetwork.player.NickName + " " + PhotonNetwork.player.GetTeam() + " " + bullet.tag.ToString());

                    bullet.transform.rotation = transform.rotation;
                    bullet.GetComponent<Rigidbody>().AddForce(transform.forward * VelocityModifier);

                    StunCooldown = 30.0f;
                }
                if (PingCooldown <= 0 && PrimaryFire == Ammo[2])
                {
                    GameObject bullet = PhotonNetwork.Instantiate(PrimaryFire.name, transform.position + Vector3.up, Quaternion.identity, 0);

                    bullet.transform.rotation = transform.rotation;
                    bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1);

                    PingCooldown = 1.0f;
                }
            }
            if (Input.GetMouseButtonDown(1)) //0 for Left Mouse Button, 1 for Right Mouse Button, 2 for Middle Mouse Button
            {
                for (int i = 0; i < Ammo.Length; i++)
                {
                    if (PrimaryFire == Ammo[i])
                    {
                        PrimaryFire = Ammo[i + 1];
                        return;
                    }
                    else if (PrimaryFire == Ammo[Ammo.Length - 1])
                    {
                        PrimaryFire = Ammo[0];
                    }
                }
            }
        }
        if (PrimaryFire == Ammo[0])
        {
            CurrentAmmoType = "AP";
        }
        if (PrimaryFire == Ammo[1])
        {
            CurrentAmmoType = "Stun";
        }
        if (PrimaryFire == Ammo[2])
        {
            CurrentAmmoType = "Ping";
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
