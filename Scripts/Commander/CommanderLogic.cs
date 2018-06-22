using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class CommanderLogic : Photon.MonoBehaviour {
    public Camera commanderCamera;
    public GameObject Ammo;
    public float attackRadius;
    public float chaseRadius;
    public float attackSpeed;
    public float missileSpeed;

    private bool isStunned;
    private float StunTimer = 5;
    private bool isSelecting = false;
    private Vector3 mousePosition;

    private List<Unit> units = new List<Unit>();
    private List<Unit> selectedUnits = new List<Unit>();
    private List<Collider> unitsInAttackRange = new List<Collider>();
    private List<Collider> unitsInChaseRange = new List<Collider>();
    
    // Use this for initialization
    void Start ()
    {
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Unit"))
            {
                Unit newUnit = new Unit();
                newUnit.Tank = gameObject;
                newUnit.Tank.tag = "BlueUnit";
                newUnit.Tank.GetPhotonView().RPC("SetTeamTag", PhotonTargets.All, newUnit.Tank.tag);
                units.Add(newUnit);
            }
        }
        else if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Unit"))
            {
                Unit newUnit = new Unit();
                newUnit.Tank = gameObject;
                newUnit.Tank.tag = "RedUnit";
                newUnit.Tank.GetPhotonView().RPC("SetTeamTag", PhotonTargets.All, newUnit.Tank.tag);
                units.Add(newUnit);
            }
        }
	}

    // Update is called once per frame
    void Update()
    {
        if (StunTimer < 0)
        {
            StunTimer = 5;
            isStunned = false;
        }
        if (isStunned == true && StunTimer > 0)
        {
            StunTimer -= Time.deltaTime;
        }
        if (photonView.isMine)
        {
            //destroy any tanks that are dead
            for (int i = units.Count - 1; i > -1; i--)
            {
                if (units[i].isDead)
                {
                    for (int j = selectedUnits.Count - 1; j > -1; j--)
                    {
                        if (selectedUnits[j].Tank == units[i].Tank)
                        {
                            selectedUnits.RemoveAt(j);
                        }
                    }
                    PhotonNetwork.Destroy(units[i].Tank);
                    units.RemoveAt(i);
                }
            }

            //Check if all tanks are dead, if so you win/lose
            if (units.Count == 0)
            {
                this.gameObject.GetPhotonView().RPC("VictoryOrDefeatScreen", PhotonTargets.All, PhotonNetwork.player.GetTeam());
            }

            //if left mouse button pressed, save mouse location and begin selection
            if (Input.GetMouseButtonDown(0) == true)
            {
                //When you click, all units are deselected initially
                //Note: Need to add a way to just click on one unit
                for (int i = selectedUnits.Count - 1; i > -1; i--)
                {
                    ToggleSelectionCircle(selectedUnits[i], false);
                    selectedUnits.RemoveAt(i);
                }

                SelectSingleUnit(); //if any unit at mouse position, select that unit
                isSelecting = true;
                mousePosition = Input.mousePosition;
            }

            //if  we let go of left mouse button, end selection
            if (Input.GetMouseButtonUp(0) == true)
            {
                //when mouse button is released, look through all units.
                //if they are within the bounds of the selection box, they are now selected units
                for (int i = units.Count - 1; i > -1; i--)
                {
                    if (isWithinSelectionBounds(units[i].Tank) == true)
                    {
                        selectedUnits.Add(units[i]);
                        
                    }
                }

                isSelecting = false;
            }

            //any units that are selected change color to siginify they are selected
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                ToggleSelectionCircle(selectedUnits[i], true);
            }

            //right mouse button clicked
            if (Input.GetMouseButton(1) == true && selectedUnits.Count > 0)
            {
                //if you click anywhere, reset attack and change states for all selected units
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].AttackState = false;
                    selectedUnits[i].ChaseState = false;
                }

                MoveToLocation(); //move selected tanks to locations clicked
            }
           
            SearchForEnemies(); //search if any enemies are in attack/chase radius and react accordingly

            //reset all units that were previously in range when calculations are done
            unitsInAttackRange.Clear();
            unitsInChaseRange.Clear();
        }
    }

    public void SearchForEnemies()
    {
        //Search if any enemy units are in attack radius
        for (int i = 0; i < units.Count; i++)
        {
            NavMeshAgent checkAgent = units[i].Tank.GetComponent<NavMeshAgent>();
           
            //Check is player is moving first before searching for enemies
            if ((checkAgent.hasPath && HasReachedDestination(checkAgent)) || checkAgent.hasPath == false)
            {
                Collider[] attackRangeColliders = Physics.OverlapSphere(units[i].Tank.transform.position, attackRadius);
                Collider[] chaseRangeColliders = Physics.OverlapSphere(units[i].Tank.transform.position, chaseRadius);

                bool enemiesInAttackRange = false;
                bool enemiesInChaseRange = false;

                foreach (Collider collider in attackRangeColliders)
                {
                    string team = string.Empty;
                    string enemyteam = string.Empty;
                    if (units[i].Tank.tag.Contains("Red"))
                    {
                        team = "Red";
                    }
                    else if (units[i].Tank.tag.Contains("Blue"))
                    {
                        team = "Blue";
                    }

                    if(collider.tag.Contains("Red") && collider.tag.Contains("Shell") == false)
                    {
                        enemyteam = "Red";
                    }
                    else if (collider.tag.Contains("Blue") && collider.tag.Contains("Shell") == false)
                    {
                        enemyteam = "Blue";
                    }

                    if (enemyteam != string.Empty && enemyteam != team)
                    {
                        unitsInAttackRange.Add(collider);
                        units[i].AttackState = true;
                        units[i].ChaseState = false;
                        enemiesInAttackRange = true;
                    }
                }

                //if no enemies in attack range, check if any are within chase range
                if (enemiesInAttackRange == false && units[i].AttackState == true)
                {
                    foreach (Collider collider in chaseRangeColliders)
                    {
                        string team = string.Empty;
                        string enemyteam = string.Empty;
                        if (units[i].Tank.tag.Contains("Red"))
                        {
                            team = "Red";
                        }
                        else if (units[i].Tank.tag.Contains("Blue"))
                        {
                            team = "Blue";
                        }

                        if (collider.tag.Contains("Red") && collider.tag.Contains("Shell") == false)
                        {
                            enemyteam = "Red";
                        }
                        else if (collider.tag.Contains("Blue") && collider.tag.Contains("Shell") == false)
                        {
                            enemyteam = "Blue";
                        }

                        if (enemyteam != string.Empty && enemyteam != team)
                        {
                            unitsInChaseRange.Add(collider);
                            units[i].AttackState = false;
                            units[i].ChaseState = true;
                            enemiesInChaseRange = true;
                        }
                    }
                }

                if (enemiesInAttackRange == false && enemiesInChaseRange == false)
                {
                    units[i].AttackState = false;
                    units[i].ChaseState = false;
                    units[i].AttackTimer = 0;
                }

                if (units[i].AttackState == true)
                {
                    Vector3 nearestEnemy = unitsInAttackRange[0].transform.position; //stores the position of the nearest enemy

                    for (int j = 0; j < unitsInAttackRange.Count; j++) //go through each enemy in range and check which one is the nearest
                    {
                        float oldDistance = Vector3.Distance(units[i].Tank.transform.position, nearestEnemy);
                        float newDistance = Vector3.Distance(units[i].Tank.transform.position, unitsInAttackRange[j].transform.position);

                        if (newDistance < oldDistance) //if the next unit in the array is closer than the previous one, replace the value
                        {
                            nearestEnemy = unitsInAttackRange[j].transform.position;
                        }
                    }

                    //attack nearest enemy
                    ShootBullet(units[i], nearestEnemy);
                }
                else if (units[i].ChaseState == true)
                {
                    Vector3 nearestEnemy = unitsInChaseRange[0].transform.position;

                    for (int j = 0; j < unitsInChaseRange.Count; j++)
                    {
                        float oldDistance = Vector3.Distance(units[i].Tank.transform.position, nearestEnemy);
                        float newDistance = Vector3.Distance(units[i].Tank.transform.position, unitsInChaseRange[j].transform.position);

                        if (newDistance < oldDistance)
                        {
                            nearestEnemy = unitsInChaseRange[j].transform.position;
                        }
                    }

                    //keep attack timer running until it hits 0 again
                    if (units[i].AttackTimer != 0)
                    {
                        units[i].AttackTimer += Time.deltaTime;
                        if (units[i].AttackTimer > (2 / attackSpeed))
                        {
                            units[i].AttackTimer = 0;
                        }
                    }

                    //chase nearest enemy
                    units[i].Tank.transform.LookAt(nearestEnemy);
                    units[i].Tank.GetComponent<NavMeshAgent>().SetDestination(nearestEnemy);
                }
            }
        }
    }

    public void ShootBullet(Unit unit, Vector3 enemy)
    {
        unit.AttackTimer += Time.deltaTime;

        if (unit.AttackTimer > (2 / attackSpeed)) //attack speed interval so player doesnt attack every frame
        {
            unit.Tank.transform.LookAt(enemy); //rotate tank towards enemy
            Vector3 bulletLocation = unit.Tank.transform.position + unit.Tank.transform.forward * 6;
            GameObject bullet = PhotonNetwork.Instantiate(Ammo.name, bulletLocation, Quaternion.identity, 0);
            if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
            {
                bullet.tag = "RedUnitShell";
            }
            else if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
            {
                bullet.tag = "BlueUnitShell";
            }
            bullet.GetPhotonView().RPC("SetTag", PhotonTargets.All, bullet.tag);

            bullet.transform.LookAt(enemy); //rotate bullet forward vector towards enemy
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * missileSpeed);
            unit.AttackTimer = 0;
            Destroy(bullet, 1.0f);
        }
    }

    public void SelectSingleUnit()
    {
        //Raycasting to check if a single unit was selected
        RaycastHit hit; //stores the data of what we hit
        Ray ray = commanderCamera.ScreenPointToRay(Input.mousePosition); //our ray shoots out from our mouse

        if (Physics.Raycast(ray, out hit)) //if we hit something with our ray
        {
            if (hit.collider.transform.tag.Contains("Unit")) //check if the object we hit is a unit
            {
                for (int i = units.Count - 1; i > -1; i--)
                {
                    if (hit.collider.gameObject == units[i].Tank) //if so, loop through all units to find out the index of the unit we collided with
                    {
                        selectedUnits.Add(units[i]);
                        ToggleSelectionCircle(units[i], true);
                    }
                }
            }
        }
    }
    public void MoveToLocation()
    {
        RaycastHit hit; //stores the data of what we hit
        Ray ray = commanderCamera.ScreenPointToRay(Input.mousePosition); //our ray shoots out from our mouse

        if (Physics.Raycast(ray, out hit)) //if we hit something with our ray
        {
            if (hit.collider.transform.tag == "Ground")
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    if (isStunned)
                    {
                        transform.position = new Vector3(transform.position.x + (Random.insideUnitCircle.x * 0.05f), transform.position.y, transform.position.z);
                    }
                    else
                    {
                        selectedUnits[i].Tank.transform.LookAt(hit.point);
                        selectedUnits[i].Tank.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                    }
                }
            }
        }
    }

    public bool HasReachedDestination(NavMeshAgent agent)
    {
        float destinationThreshold = 0.1f; //determines how offset the tank has to be from destination before it returns true

        if (agent.remainingDistance <= agent.stoppingDistance + destinationThreshold)
        {
            return true;
        }
        return false;
    }

    public void ToggleSelectionCircle(Unit unit, bool flag)
    {
        unit.Tank.transform.GetComponentInChildren<Projector>().enabled = flag;
    }

    public bool isWithinSelectionBounds(GameObject obj)
    {
        if (isSelecting == false)
        {
            return false;
        }

        var viewportBounds = Utilities.GetViewportBounds(commanderCamera, mousePosition, Input.mousePosition);

        return viewportBounds.Contains(commanderCamera.WorldToViewportPoint(obj.transform.position));     
    }

    public void RemoveTankFromArray(GameObject tank)
    {
        if (photonView.isMine)
        {
            for (int i= 0; i < units.Count; i++)
            {
                if (units[i].Tank == tank)
                {
                    units[i].isDead = true;
                }
            }
        }
    }

    [PunRPC]
    public void VictoryOrDefeatScreen(PunTeams.Team losingTeam)
    {
        PunTeams.Team playerTeam = PhotonNetwork.player.GetTeam();

        if (losingTeam == PunTeams.Team.blue)
        {
            if (playerTeam == PunTeams.Team.red)
            {
                GameObject.Find("VictoryCamera").GetComponent<Camera>().enabled = true;
            }
            else if (playerTeam == PunTeams.Team.blue)
            {
                GameObject.Find("DefeatCamera").GetComponent<Camera>().enabled = true;
            }
        }
        else if (losingTeam == PunTeams.Team.red)
        {
            if (playerTeam == PunTeams.Team.blue)
            {
                GameObject.Find("VictoryCamera").GetComponent<Camera>().enabled = true;
            }
            else if (playerTeam == PunTeams.Team.red)
            {
                GameObject.Find("DefeatCamera").GetComponent<Camera>().enabled = true;
            }
        }
    }

    void OnGUI()
    {
        if (photonView.isMine)
        {
            if (isSelecting == true)
            {
                //Create a rect from both mouse positions, where you originally clicked and where mouse currently is
                var rect = Utilities.GetScreenRect(mousePosition, Input.mousePosition);
                Utilities.DrawScreenRect(rect, new Color(0.0f, 1.0f, 0.0f, 0.25f));
                Utilities.DrawScreenRectBorder(rect, 2, new Color(0.0f, 1.0f, 0.0f, 0.95f));
            }
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

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
    }
}

public class Unit
{
    public GameObject Tank;
    public bool AttackState;
    public bool ChaseState;
    public float AttackTimer = 0;
    public bool isDead = false;
}