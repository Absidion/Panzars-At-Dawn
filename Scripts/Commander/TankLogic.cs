using UnityEngine;
using System.Collections;

public class TankLogic : MonoBehaviour {
    public float Health;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Health <= 0)
        {
            GameObject camera = GameObject.FindGameObjectWithTag("CommanderCamera");
            CommanderLogic cameraScripts = camera.GetComponent("CommanderLogic") as CommanderLogic;
            cameraScripts.RemoveTankFromArray(this.gameObject);
        }
	}

    [PunRPC]
    void SetTeamTag(string tag)
    {
        this.tag = tag;
    }

    void OnCollisionEnter(Collision col)
    { 
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
        {
            if (col.gameObject.CompareTag("BlueUnitShell"))
            {
                Health -= 20;
            }

            if (col.gameObject.CompareTag("BlueShell"))
            {
                Health -= 10;
            }

            if (col.gameObject.CompareTag("BlueStunShell"))
            {
                GameObject camera = GameObject.FindGameObjectWithTag("CommanderCamera");
                CommanderLogic cameraScripts = camera.GetComponent("CommanderLogic") as CommanderLogic;

                cameraScripts.SetStunned(true);
            }
        }

        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
        {
            if (col.gameObject.CompareTag("RedUnitShell"))
            {
                Health -= 20;
            }

            if (col.gameObject.CompareTag("RedShell"))
            {
                Health -= 10;
            }

            if (col.gameObject.CompareTag("RedStunShell"))
            {
                GameObject camera = GameObject.FindGameObjectWithTag("CommanderCamera");
                CommanderLogic cameraScripts = camera.GetComponent("CommanderLogic") as CommanderLogic;

                cameraScripts.SetStunned(true);
            }
        }
    }
}
