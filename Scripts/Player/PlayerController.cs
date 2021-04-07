using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody myRB;              //reference to Rigidbody of the player object.
    public Transform myTrans;           //reference to Transform of the player object.

    UIManager ui;                       //reference to the UIManager script.
    CameraController cc;                //reference to the CameraController script.
    PlayerManager pm;

    public Camera mainCamera;           //camera to hold reference for main camera object; used in movement and target selection.
    public Camera longCamera;           //camera to hold reference for long camera object; used in movement and target selection.

    //using physics raycast hit from a mouse click (and mobile phone screen tap) to set the agents destination.
    NavMeshAgent myAgent;
    RaycastHit hitInfo = new RaycastHit();
    Vector3 point1;

    public bool walkMode;               //bool for Mode walk
    public bool attackMode;             //bool for Mode attack
    public bool specialMode;            //bool for Mode special

    public bool isMoving;               //bool to check whether the player is moving, related to animation.
    public bool attacking;              //bool to check whether the player is attacking, related to animation.
    public bool special;                //bool to check whether the player is doing the special move, related to animation.
    public bool targetInRange;          //bool to check whether target is in range to attack target.

    private Animator myAnim;            //reference to Animator that will be on the player.
    private int isWalking;              //an int to take the Animator bool isWalking, using Hash int doesn't create garbage like string does.
    private int isAttacking;            //an int to take the Animator bool isAttacking, less garabage this way.
    private int isSpecialAttack;        //an int to take the Animator bool isSpecial, less garabage this way.

    public int damageToGive = 50;       //int for damage to give to other game objects.
    public int specialDamageToGive = 25;
    private float rotSpeed = 0.5f;      //float for rotational speed when turning to look at something.
    private Vector3 veloRot;            //reference for Vec3 velocity of rotation, for turnings.

    public GameObject target;           //reference to game object that has been targetted.

    public ParticleSystem walkToFX;     //reference for PS walk to indicator.
    public GameObject specialAttackFX;  //reference to PS for attacking objects.
    public GameObject specialAttackSFX; //reference to audiosource for attaking objects sound.


    void Start()
    {
        InitPlayerController();
    }


    void Update()
    {
        MovingCheck();
        SphereCast();
        TargetUpdate();
        ResetTarget();
        PlayerMode();
    }


    //Function to initalise all that needs to be done at the beginning of the scene. Including getting components and setting starting positions.
    void InitPlayerController()
    {
        //grabbing reference from required components. 
        myAgent = GetComponent<NavMeshAgent>();
        myRB = GetComponent<Rigidbody>();
        myTrans = GetComponent<Transform>();
        myAnim = GetComponent<Animator>();
        pm = GetComponent<PlayerManager>();

        //setting this Vec3 to be center of player to cast sphere out around from.
        point1 = myTrans.position;

        //setting camera related references up; script and two camera objects.
        cc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        longCamera = GameObject.FindGameObjectWithTag("LongCamera").GetComponent<Camera>();

        //setting walk mode to be true to start with and other modes false.
        walkMode = true;
        attackMode = false;
        specialMode = false;

        //setting all animation to be false so starts in idle animation state.
        isMoving = false;
        attacking = false;
        special = false;

        //setting animator bool isWalking from string to int, creates less garbage.
        isWalking = Animator.StringToHash("isWalking");         
        isAttacking = Animator.StringToHash("isAttacking");     
        isSpecialAttack = Animator.StringToHash("isSpecialAttack");

        //player can be large on screen, don't want raycasts to hit this and reduce world interaction.
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    //function checking whether mode the player is in and hence which functions are called.
    void PlayerMode()
    {
        if (walkMode == true)
        {
            //player movement
            PlayerMovement();
        }
        else if (attackMode == true)
        {
            //select target
            SelectTarget();
            //player attack
            PlayerAttack();
        } 
        else if (specialMode == true)
        {
            //player special attack
            SpecialAttackHere();
        }
    }


    //casting a sphere around player, makeslist of colliders, check if people, if so sets their seenMonster bool to true.
    void SphereCast()
    {
        Collider[] hitColliders = Physics.OverlapSphere(point1, 40.0f);

        if(hitColliders != null)
        { 
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].tag == "People")
                {
                    hitColliders[i].GetComponent<PeopleController>().monsterSeen = true;
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }
    }

    void SpecialAttackSphereCast()
    {
        Collider[] hitColliders = Physics.OverlapSphere(point1, 40.0f);

        if(hitColliders != null)
        {
            int i = 0;
            while(i < hitColliders.Length)
            {
                if(hitColliders[i].tag == "People")
                {
                    hitColliders[i].GetComponent<PeopleController>().PeopleDeath();
                    i++;
                }
                else if (hitColliders[i].tag == "Vehicle")
                {
                    hitColliders[i].GetComponent<VehicleController>().DestroyThisVehicle();
                    i++;
                }
                else if (hitColliders[i].tag == "Building")
                {
                    hitColliders[i].GetComponent<BuildingManager>().buildingHealth -= specialDamageToGive;
                    i++;
                }
                else if (hitColliders[i].tag == "Factory")
                {
                    hitColliders[i].GetComponent<BuildingManager>().buildingHealth -= specialDamageToGive;
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }
    }

    //function to check if the raycast of the screen click has hit a UI object, 
    //stops it there and prevents clicking behind UI, mainly a problem on andriod platform as raycast hit worked for PC but not mobile.
    private bool IsClickOnUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }


    //function for moving player with mouse click or screen tap.
    //uses raycast hit from the click/tap in relation to main camera and passing the NavMeshAgent's destination.
    void PlayerMovement()
    {
        //make sure the player is able to move.
        StartMovement();

        //checking if mouse click / screen tap.
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsClickOnUIObject())
            {
                //checking which camera is enabled and using that for the mouse click position reference for raycast.
                if (cc.mainCamera.enabled == true)
                {
                    //raycast var that takes the mouse input position in relation to the camera screen.
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                    {
                        //setting navmeshagent's destination to the point of click on screen in relation to hit point of navmesh layer.
                        myAgent.destination = hitInfo.point;

                        //instantiate the walkToFX, with new Quarternion so it's at a 90 degree angle.
                        Quaternion newQuaternion = new Quaternion();
                        newQuaternion.Set(1.0f, 0.0f, 0.0f, 1.0f);
                        Instantiate(walkToFX, myAgent.destination, newQuaternion);
                    }
                }
                else if (cc.longCamera.enabled == true)
                {
                    //raycast var that takes the mouse input position in relation to the camera screen.
                    Ray ray = longCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                    {
                        //setting navmeshagent's destination to the point of click on screen in relation to hit point of navmesh layer.
                        myAgent.destination = hitInfo.point;

                        //instantiate the walkToFX, with new Quarternion so it's at a 90 degree angle.
                        Quaternion newQuaternion = new Quaternion();
                        newQuaternion.Set(1.0f, 0.0f, 0.0f, 1.0f);
                        Instantiate(walkToFX, myAgent.destination, newQuaternion);
                    }
                }
            }
        }
    }

    void TargetUpdate()
    {
        if(target == null)
        {
            targetInRange = false;
        }
    }

    //function to take mouse click / screen tap, test if it's selected a building and set that to target.
    void SelectTarget()
    {
        //checking if mouse click / screen tap.
        if(Input.GetMouseButtonDown(0))
        {
            if (!IsClickOnUIObject())
            {
                //checking which camera is enabled and using that for the mouse click position reference for raycast.
                if (cc.mainCamera.enabled == true)
                {
                    //raycast var that takes the mouse input position in relation to the camera screen.
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        //check whether the tag of clicked object is a building
                        if (hitInfo.transform.tag == "Building")
                        {
                            //if so, then set this building game object to the gameobject target.
                            target = hitInfo.collider.gameObject;
                        }
                        else if (hitInfo.transform.tag == "Factory")
                        {
                            //if so, then set this building game object to the gameobject target.
                            target = hitInfo.collider.gameObject;
                        }
                    }
                }
                else if (cc.longCamera.enabled == true)
                {
                    //raycast var that takes the mouse input position in relation to the camera screen.
                    Ray ray = longCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        //check whether the tag of clicked object is a building
                        if (hitInfo.transform.tag == "Building")
                        {
                            //if so, then set this building game object to the gameobject target.
                            target = hitInfo.collider.gameObject;
                        }
                        else if (hitInfo.transform.tag == "Factory")
                        {
                            //if so, then set this building game object to the gameobject target.
                            target = hitInfo.collider.gameObject;
                        }
                    }
                }
            }
        }
    }

    void ResetTarget()
    {
        if(target != null)
        { 
            if(target.activeSelf == false)
            {
                target = null;
            }
        }
    }

    //function to move towards target.
    public void MoveToTarget()
    {
        StartMovement();

        if(target != null)
        { 
            //if no target in range.
            if (!targetInRange)
            {
                myAgent.destination = target.GetComponent<BuildingManager>().attackPosition;
                //myAgent.SetDestination(target.GetComponent<BuildingManager>().attackPosition);
            }
        }
    }

    //function
    public void PlayerAttack()
    {
        //check if there is a target.
        if(target != null)
        {
            //then that the target is in range.
            if (targetInRange == true)
            {
                //if so, start the attack sequence against that target.
                StartCoroutine(AttackSequence(target));
            }
            else if (targetInRange == false)
            {
                StopCoroutine(AttackSequence(target));
                //function to move towards target.
                MoveToTarget();
            }
        }
    }

    //function
    public void SpecialAttackHere()
    {
        StopMovement();
        if (special == false)
        {
            special = true;
            StartCoroutine(SpecialAttack());
        }
    }

    //function to stop player moving by setting destination to current position.
    public void StopMovement()
    {
        myAgent.isStopped = true;
    }

    //
    private void StartMovement()
    {
        myAgent.isStopped = false;
    }

    //function to check whether the player is moving and whether the walking or idle animation is required.
    //uses the velocity from the NavMeshAgent to see whether player is moving.
    void MovingCheck()
    {
        if(myAgent.velocity.sqrMagnitude < 0.1f) {
            myAnim.SetBool(isWalking, false);
        } else {
            myAnim.SetBool(isWalking, true);
        }
    }


    //function 
    public void AttackTargetPosition(GameObject targetPosition)
    {
        StopMovement();
        if (attacking == false)
        {
            attacking = true;
            StartCoroutine(Attack(targetPosition));
        }
    }

    //function to have player face towards target
    public void FaceTowardsTarget(Vector3 targetPos)
    {
        Vector3 faceTarget = new Vector3(targetPos.x, targetPos.y, targetPos.z);
        faceTarget = Vector3.SmoothDamp(myTrans.position, faceTarget, ref veloRot, rotSpeed);
        myTrans.LookAt(faceTarget);
    }

    //function for enter collision between player sphere collider and target collider, setting targetInRange to true.
    private void OnTriggerEnter(Collider other)
    {
        if (target != null)
        {
            if (other.name == target.name)
            {
                targetInRange = true;
            }
        }
    }

    //function for exit collision between player sphere collider and target collider, setting targetInRange to false.
    private void OnTriggerExit(Collider other)
    {
        if (target != null)
        {
            if (other.name == target.name)
            {
                targetInRange = false;
            }
        }
    }

    //
    IEnumerator AttackSequence(GameObject thisTarget)
    {
        if(targetInRange == true)
        {
            //look towards the target
            FaceTowardsTarget(thisTarget.transform.position);
            //stop player moving.
            StopMovement();
            //attack
            StartCoroutine(Attack(thisTarget));
            yield return null;
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator SpecialAttackSequence()
    {
        StopMovement();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SpecialAttack());
    }

    //
    IEnumerator Attack(GameObject thisTarget)
    {
        float attackCooldown = 1.0f;
        //second wait before starting the attack.
        while(attackCooldown > 0 && thisTarget != null)
        {
            FaceTowardsTarget(thisTarget.transform.position);
            attackCooldown -= Time.deltaTime;
            yield return null;
        }

        //DO animation of attack.
        myAnim.SetBool(isAttacking, true);

        //another 1 sec wait before continuing.
        attackCooldown = 1.0f;
        while(attackCooldown > 0.0f)
        {
            attackCooldown -= Time.deltaTime;
            yield return null;
        }

        //out of attack animation...
        myAnim.SetBool(isAttacking, false);

        attacking = false;
    }

    //
    IEnumerator SpecialAttack()
    {
        float attackCooldown = 1.0f;

        //second wait before starting the attack.
        while (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            yield return null;
        }

        //set the special attack animation to true.
        myAnim.SetBool(isSpecialAttack, true);

        yield return new WaitForSeconds(0.75f);

        //play the attack particles fx.
        Instantiate(specialAttackFX, new Vector3(myTrans.position.x, (myTrans.position.y + 1.0f), myTrans.position.z), Quaternion.identity);

        //play special attack SFX.
        Instantiate(specialAttackSFX, myTrans.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        //add the damage to those in the sphere cast.
        SpecialAttackSphereCast();

        //out of attack animation...
        myAnim.SetBool(isSpecialAttack, false);

        //resetting special bar to 0.
        pm.playerCurrentSpecial = 0.0f;

        //setting attacking back to false, so can attack again.
        special = false;
        //back to walking mode.
        walkMode = true;
    }


}

