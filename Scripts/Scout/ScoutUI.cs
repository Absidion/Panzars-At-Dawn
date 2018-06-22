using UnityEngine;
using System.Collections;

public class ScoutUI : Photon.MonoBehaviour {

    public Texture2D crosshairImage;

    public float MaxHealth;
    private float CurrentHealth;
    CursorLockMode wantedMode;
   // private CursorLockMode wantedMode;

    public Texture textureAP;
    public Texture textureStun;
    public Texture texturePing;
    public Texture textureHealth;

    public Font ScoutFont;
    // Use this for initialization
    void Start () {
        CurrentHealth = MaxHealth;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (CurrentHealth <= 0)
        {
            GetComponentInChildren<PlayerMovement>().SetDead(true);
        }
	}

    void SetCursorState()
    {
        //Cursor.lockState = wantedMode;
        //hide cursor when locking
        //  Cursor.visible = (CursorLockMode.Locked != wantedMode);
        Cursor.visible = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (photonView.isMine)
        {
            if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
            {
                if (col.gameObject.CompareTag("RedShell"))
                {
                    int damage = Random.Range(30, 51);
                    CurrentHealth -= damage;
                }
                if (col.gameObject.CompareTag("RedStunShell"))
                {
                    GetComponentInChildren<PlayerMovement>().SetStunned(true);
                }
                if (col.gameObject.CompareTag("RedUnitShell"))
                {
                    int damage = Random.Range(75, 101);
                    CurrentHealth -= damage;
                }
            }
            if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
            {
                if (col.gameObject.CompareTag("BlueShell"))
                {
                    int damage = Random.Range(30, 51);
                    CurrentHealth -= damage;
                }
                if (col.gameObject.CompareTag("BlueStunShell"))
                {
                    GetComponentInChildren<PlayerMovement>().SetStunned(true);
                }
                if (col.gameObject.CompareTag("BlueUnitShell"))
                {
                    int damage = Random.Range(75, 101);
                    CurrentHealth -= damage;
                }
            }
        }
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
    }

    void OnGUI()
    {
        if (photonView.isMine)
        {
            /*
            GUILayout.BeginVertical();

            //Release cursor on escape keypress
            if (Input.GetKeyDown(KeyCode.Escape))
                Cursor.lockState = wantedMode = CursorLockMode.None;

            switch (Cursor.lockState)
            {
                case CursorLockMode.None:
                    GUILayout.Label("Cursor is normal");
                    if (GUILayout.Button("Lock cursor"))
                        wantedMode = CursorLockMode.Locked;
                    if (GUILayout.Button("Confine cursor"))
                        wantedMode = CursorLockMode.Confined;

                    break;
                case CursorLockMode.Confined:
                    GUILayout.Label("Cursor is confined");
                    if (GUILayout.Button("Lock cursor"))
                        wantedMode = CursorLockMode.Locked;
                    if (GUILayout.Button("Release cursor"))
                        wantedMode = CursorLockMode.None;
                    break;
                case CursorLockMode.Locked:
                    GUILayout.Label("Cursor is locked");
                    if (GUILayout.Button("Unlock cursor"))
                        wantedMode = CursorLockMode.None;
                    if (GUILayout.Button("Confine cursor"))
                        wantedMode = CursorLockMode.Confined;
                    break;
            }
            GUILayout.EndVertical();
            */

            GUIStyle scoutGuiStyle = new GUIStyle();
            scoutGuiStyle.fontSize = 25;
            scoutGuiStyle.font = ScoutFont;
            scoutGuiStyle.normal.textColor = Color.white;

            GUIStyle APGuiStyle = new GUIStyle();
            APGuiStyle.fontSize = 25;
            APGuiStyle.font = ScoutFont;
            APGuiStyle.normal.textColor = Color.white;

            GUIStyle StunGuiStyle = new GUIStyle();
            StunGuiStyle.fontSize = 25;
            StunGuiStyle.font = ScoutFont;
            StunGuiStyle.normal.textColor = Color.white;

            GUIStyle PingGuiStyle = new GUIStyle();
            PingGuiStyle.fontSize = 25;
            PingGuiStyle.font = ScoutFont;
            PingGuiStyle.normal.textColor = Color.white;

            GUIStyle HealthGuiStyle = new GUIStyle();
            HealthGuiStyle.fontSize = 25;
            HealthGuiStyle.font = ScoutFont;
            HealthGuiStyle.normal.textColor = Color.green;

            float PCool = Mathf.Round(GetComponentInChildren<Shoot>().PrimaryCooldown * 10.0f) / 10.0f;
            float SCool = Mathf.Round(GetComponentInChildren<Shoot>().StunCooldown * 10.0f) / 10.0f;
            float PingCool = Mathf.Round(GetComponentInChildren<Shoot>().PingCooldown * 10.0f) / 10.0f;

            if (PCool > 0)
            {
                APGuiStyle.normal.textColor = Color.red;
            }
            else
            {
                APGuiStyle.normal.textColor = Color.white;
            }
            if (SCool > 0)
            {
                StunGuiStyle.normal.textColor = Color.red;
            }
            else
            {
                StunGuiStyle.normal.textColor = Color.white;
            }
            if (PingCool > 0)
            {
                PingGuiStyle.normal.textColor = Color.red;
            }
            else
            {
                PingGuiStyle.normal.textColor = Color.white;
            }

            if (CurrentHealth < 250)
            {
                HealthGuiStyle.normal.textColor = Color.yellow;
                if (CurrentHealth < 100)
                {
                    HealthGuiStyle.normal.textColor = Color.red;
                }
            }

            if (GetComponentInChildren<Shoot>().CurrentAmmoType == "AP")
            {
                scoutGuiStyle.normal.textColor = Color.yellow;
            }
            else if (GetComponentInChildren<Shoot>().CurrentAmmoType == "Stun")
            {
                scoutGuiStyle.normal.textColor = Color.cyan;
            }
            else if (GetComponentInChildren<Shoot>().CurrentAmmoType == "Ping")
            {
                scoutGuiStyle.normal.textColor = Color.green;
            }

            GUI.Label(new Rect(0, 10, textureAP.width, textureAP.height), textureAP);
            GUI.Label(new Rect(textureAP.width + 5, (textureAP.height +10) / 2, 50, 50), PCool.ToString(), APGuiStyle);
            float labelDrop = textureAP.height + 5;

            GUI.Label(new Rect(0, labelDrop, textureStun.width, textureStun.height), textureStun);
            labelDrop += textureStun.height + 5;
            GUI.Label(new Rect(textureAP.width + 5, labelDrop * 0.75f, 50, 50), SCool.ToString(), StunGuiStyle);
            

            GUI.Label(new Rect(35, labelDrop, texturePing.width, texturePing.height), texturePing);
            labelDrop += texturePing.height + 5;
            GUI.Label(new Rect(textureAP.width + 5, labelDrop * 0.75f, 50, 50), PingCool.ToString(), PingGuiStyle);

            labelDrop += textureHealth.height / 2 * 5;
            GUI.Label(new Rect(35, labelDrop - textureHealth.height / 2 - 40, textureHealth.width / 2, textureHealth.height / 2), textureHealth);
            labelDrop += textureHealth.height / 2 + 5;
            GUI.Label(new Rect(textureHealth.width + 5, labelDrop * 0.75f, 50, 50), CurrentHealth.ToString(), HealthGuiStyle);

            labelDrop += textureAP.height / 2 + 20;
            GUI.Label(new Rect(5, labelDrop * 0.75f, 50, 50), "Current Ammo: " + GetComponentInChildren<Shoot>().CurrentAmmoType, scoutGuiStyle);
            SetCursorState();
        }
        //crosshair is locked to center X
        //float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
       // crosshair follows cursor
      //  float xMin = Screen.width  - (Screen.width -  Input.mousePosition.x) - (crosshairImage.width / 2 );
        float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage.height / 2);
        float yMax = ((Screen.height - Input.mousePosition.y) - (crosshairImage.height / 2)/2);
        if(yMin < yMax)
        {
            yMin = yMax;
        }

     //   GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);

    }
    [PunRPC]
    void SetTag(string objectTag)
    {
        this.tag = objectTag;
    }
}
