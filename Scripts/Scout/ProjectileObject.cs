using UnityEngine;
using System.Collections;

public class ProjectileObject : MonoBehaviour {

    public float LifeTime;
    // Use this for initialization
    void Start () {
        Destroy(this.gameObject, LifeTime);
               
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision col)
    {
        this.gameObject.GetPhotonView().RPC("DestroyProjectile", PhotonTargets.All);
    }

    [PunRPC]
    void SetTag(string ObjectTag)
    {
        this.tag = ObjectTag;
    }

    [PunRPC]

    void DestroyProjectile()
    {
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.Destroy(this.gameObject);
    }
}
