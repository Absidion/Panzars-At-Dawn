using UnityEngine;
using System.Collections;

public class GameMenu : Photon.PunBehaviour {

    public GameObject scoutPrefab;
    public GameObject CommanderPrefab;
    public GameObject CommanderCameraPrefab;

    private string SelectedRole;
    private GameObject SavedObject;

    private Vector3 RedCommanderSpawn = new Vector3(-134, 8, -130);
    private Vector3 RedCommanderCamera = new Vector3(-134, 40, -130);
    private Vector3 BlueCommanderSpawn = new Vector3(637, -8, -148);
    private Vector3 BlueCommanderCamera = new Vector3(637, 40, -148);
    private Vector3 RedScoutSpawn = new Vector3(-134, 1, -125);
    private Vector3 BlueScoutSpawn = new Vector3(627, -8, -165);

    // Use this for initialization
    void Start()
    {
        SavedObject = GameObject.Find("SharedValues");
        SelectedRole = SavedObject.GetComponent<SharedVariables>().SelectedRole.ToString();

        if (SelectedRole == "Scout")
        {
            SpawnScout();
        }

        if (SelectedRole == "Commander")
        {
            SpawnCommander();
        }

        DestroyObject(SavedObject);

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.playerList[i].NickName + " " + PhotonNetwork.playerList[i].GetTeam().ToString());
        }
    }

    void SpawnScout()
    {
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
        {
            GameObject newPlayer = PhotonNetwork.Instantiate(scoutPrefab.name, RedScoutSpawn, Quaternion.identity, 0);

            CharacterController controller = newPlayer.GetComponent<CharacterController>();
            controller.enabled = true;
            Camera scoutCamera = newPlayer.GetComponentInChildren<Camera>();
            scoutCamera.enabled = true;
            newPlayer.GetComponentInChildren<PlayerMovement>().SetRespawnLocation(RedScoutSpawn);
            newPlayer.tag = "RedScout";
            newPlayer.GetPhotonView().RPC("SetTag", PhotonTargets.All, newPlayer.tag);
        }

        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
        {
            GameObject newPlayer = PhotonNetwork.Instantiate(scoutPrefab.name, BlueScoutSpawn, Quaternion.identity, 0);

            CharacterController controller = newPlayer.GetComponent<CharacterController>();
            controller.enabled = true;
            Camera scoutCamera = newPlayer.GetComponentInChildren<Camera>();
            scoutCamera.enabled = true;
            newPlayer.GetComponentInChildren<PlayerMovement>().SetRespawnLocation(BlueScoutSpawn);
            newPlayer.tag = "BlueScout";
            newPlayer.GetPhotonView().RPC("SetTag", PhotonTargets.All, newPlayer.tag);
        }

    }

    void SpawnCommander()
    {
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
        {
            CommanderPrefab.transform.eulerAngles.Set(90, 0, 0);

            GameObject commanderCamera = PhotonNetwork.Instantiate(CommanderCameraPrefab.name, RedCommanderCamera, CommanderCameraPrefab.transform.rotation, 0);
            Camera ComCamera = commanderCamera.GetComponent<Camera>();
            ComCamera.enabled = true;

            Vector3 SpawnPos;
            SpawnPos = RedCommanderSpawn;

            SpawnPos.z += 20;
            GameObject commanderUnit1 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = RedCommanderSpawn;
            SpawnPos.z -= 20;
            GameObject commanderUnit2 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = RedCommanderSpawn;
            SpawnPos.x += 10;
            SpawnPos.z += 10;
            GameObject commanderUnit3 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = RedCommanderSpawn;
            SpawnPos.x -= 10;
            SpawnPos.z += 10;
            GameObject commanderUnit4 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = RedCommanderSpawn;
            SpawnPos.x += 10;
            SpawnPos.z -= 10;
            GameObject commanderUnit5 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = RedCommanderSpawn;
            SpawnPos.x -= 10;
            SpawnPos.z -= 10;
            GameObject commanderUnit6 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);
            
        }

        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
        {
            CommanderPrefab.transform.eulerAngles.Set(90, 0, 0);

            GameObject commanderCamera = PhotonNetwork.Instantiate(CommanderCameraPrefab.name, BlueCommanderCamera, CommanderCameraPrefab.transform.rotation, 0);
            Camera ComCamera = commanderCamera.GetComponent<Camera>();
            ComCamera.enabled = true;

            Vector3 SpawnPos;
            SpawnPos = BlueCommanderSpawn;

            SpawnPos.z += 20;
            GameObject commanderUnit1 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = BlueCommanderSpawn;
            SpawnPos.z -= 20;
            GameObject commanderUnit2 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = BlueCommanderSpawn;
            SpawnPos.x += 10;
            SpawnPos.z += 10;
            GameObject commanderUnit3 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = BlueCommanderSpawn;
            SpawnPos.x -= 10;
            SpawnPos.z += 10;
            GameObject commanderUnit4 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = BlueCommanderSpawn;
            SpawnPos.x += 10;
            SpawnPos.z -= 10;
            GameObject commanderUnit5 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);

            SpawnPos = BlueCommanderSpawn;
            SpawnPos.x -= 10;
            SpawnPos.z -= 10;
            GameObject commanderUnit6 = PhotonNetwork.Instantiate(CommanderPrefab.name, SpawnPos, Quaternion.identity, 0);
        }    
    }
}
