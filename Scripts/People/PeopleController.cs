using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeopleController : MonoBehaviour
{
    public SpawnManager sm;
    public StatManager stm;
    PlayerManager pm;

    //references for the required object components. 
    private Rigidbody ppRB;             
    private NavMeshAgent ppAgent;       
    private Animator ppAnim;            
    private Transform ppTrans;          
    private Transform monTrans;         

    //references to FXs and SFXs.
    public ParticleSystem deathFX;      
    public AudioSource walkingAudio;    
    public AudioSource panicAudio;      
    public AudioSource deathAudio;      

    //Vector3 to be used in movement functions.
    private Vector3 currentPosition;
    private Vector3 nextPosition;

    //floats and int for fine movement including range and offsets.
    private int moveRange = 10;
    private float transformOffset = 0.2f;
    private float yOffset = 1.0f;

    //floats for movement countdown, used for 'people stuck' prevention. After countdown new destination chosen.
    private float countdownTimer;
    private float countdownTime = 4.0f;

    //bool if monster seen and float distance to run.
    public bool monsterSeen;
    public float EnemyRunDistance = 10.0f;
    public float runSpeed;
    public float runAcceleration;

    //floats for collision force, to be used to add force to object on collision with player.
    public float colForce;
    private float minColForce;
    private float maxColForce;

    //bools to be used for animation states.
    public bool b_isIdle;
    public bool b_isWalking;
    public bool b_isRunning;
    public bool b_isDead;

    //ints to take the Animator bools, using Hash int doesn't create garbage like string does.
    private int isIdle;             
    private int isWalking;
    private int isRunning;
    private int isDead;
    
    //score for destroying this object.
    public int destructionScore;
    public int specialScore;

    //DEBUGGING STUFF
    public bool isOnNavMesh;        //used to check that when brought in from pool that the object can detect the navmesh.


    void Start()
    {
        InitialisePeopleController();
    }


    void Update()
    {
        if (ppAgent.isOnNavMesh == false)
        {
            Debug.Log(this.gameObject.name + " is not on a NavMesh!");
            isOnNavMesh = false;
        }
        else
        {
            isOnNavMesh = true;
        }

        IsWalkingCheck();
        IsRunningCheck();

        AnimationCheck();

        if (monsterSeen == true)
        {
            RunFromMonster();
        }
        else
        {
            WalkInMesh();
        }
    }

    void InitialisePeopleController()
    {
        ppRB = GetComponent<Rigidbody>();
        ppAgent = GetComponent<NavMeshAgent>();
        ppTrans = GetComponent<Transform>();

        monTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        //sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        stm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>();

        b_isIdle = true;
        b_isWalking = false;
        b_isRunning = false;
        b_isDead = false;

        ppAnim = GetComponent<Animator>();
        isIdle = Animator.StringToHash("isIdle");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        isDead = Animator.StringToHash("isDead");

        currentPosition = ppTrans.position;
        countdownTimer = countdownTime;

        minColForce = 1.5f;
        maxColForce = 5.0f;
        colForce = Random.Range(minColForce, maxColForce);

        runSpeed = ppAgent.speed * 4;
        runAcceleration = ppAgent.acceleration * 3;

        monsterSeen = false;
    }

    //
    void AnimationCheck()
    {
        if(b_isIdle == true && (ppAnim.GetBool(isIdle) != true))
        {
            ppAnim.SetBool(isIdle, true);

            ppAnim.SetBool(isWalking, false);
            ppAnim.SetBool(isRunning, false);
        }
        else if (b_isWalking == true && (ppAnim.GetBool(isWalking) != true))
        {
            ppAnim.SetBool(isWalking, true);

            ppAnim.SetBool(isIdle, false);
            ppAnim.SetBool(isRunning, false);

        }
        else if (b_isRunning == true && (ppAnim.GetBool(isRunning) != true))
        {
            ppAnim.SetBool(isRunning, true);

            ppAnim.SetBool(isIdle, false);
            ppAnim.SetBool(isWalking, false);
        }
        else if (b_isDead == true)
        {
            //NEEDED??? could be taken care of in death coroutine.
            ppAnim.SetBool(isDead, true);
        }
    }

    //
    void IsWalkingCheck()
    {
        if (ppTrans.position.x > (currentPosition.x + transformOffset)
            || ppTrans.position.x < (currentPosition.x - transformOffset))
        {
            b_isWalking = true;
            b_isIdle = false;
            MoveTimer();
        }
        else
        {
            b_isWalking = false;
            b_isIdle = true;
        }
    }

    //
    void IsRunningCheck()
    {
        if(monsterSeen == true)
        { 
            if (ppTrans.position.x > (currentPosition.x + transformOffset)
                || ppTrans.position.x < (currentPosition.x - transformOffset))
            {
                b_isRunning = true;
                b_isIdle = false;
                MoveTimer();
            }
            else
            {
                b_isRunning = false;
                b_isIdle = true;
            }
        }
    }

    //
    void WalkInMesh()
    {
        //
        if (b_isWalking == false && ppTrans.position != ppAgent.destination)
        {
            currentPosition = ppTrans.position;

            Vector3 randomPosition = Random.insideUnitSphere * moveRange;
            randomPosition.y = ppTrans.position.y;
            nextPosition = currentPosition + randomPosition;
            NavMeshHit hitInfo;

            if (NavMesh.SamplePosition(nextPosition, out hitInfo, moveRange, 
                (NavMesh.GetAreaFromName("PeopleWalkable"))))
            {
                ppAgent.SetDestination(nextPosition);
            }
            else
            {
                Debug.Log("NavMesh.SamplePosition() FAILED");
            }
            currentPosition = nextPosition;
        }
    }

    void RunFromMonster()
    {
        b_isRunning = true;
        b_isWalking = false;
        b_isIdle = false;

        //get them moving FAST!
        ppAgent.acceleration = ppAgent.acceleration + 3;
        ppAgent.speed = + 3;

        //Work out the direction away from the player/monster.
        Vector3 directionToMonster = ppTrans.position - monTrans.position;
        Vector3 runPosition = ppTrans.position + directionToMonster;

        //if not at that new run away position, then go there. 
        if (ppTrans.position != ppAgent.destination)
        {
            ppAgent.SetDestination(runPosition);
        }
        else
        {
            //otherwise you are there and find a new position to do one.
            directionToMonster = ppTrans.position - monTrans.position;
            runPosition = ppTrans.position + directionToMonster;
            ppAgent.SetDestination(runPosition);
        }
    }

    //for testing.
    void WalkDestinationFX(ParticleSystem fx)
    {
        Quaternion newQuaternion = new Quaternion();
        newQuaternion.Set(1.0f, 0.0f, 0.0f, 1.0f);
        Instantiate(fx, ppAgent.destination, newQuaternion);
    }

    
    //function to check if it has been too long since person has selected new destination (suggests they're stuck).
    void MoveTimer()
    {
        countdownTimer = countdownTime;
        countdownTimer -= Time.deltaTime;

        //
        if (countdownTimer <= 0.0f)
        {
            countdownTimer = 0.0f;
            ppAgent.SetDestination(ppTrans.position);
            b_isWalking = false;
            b_isIdle = true;
        }
    }

    private int IntRandomRangeBetween(int a, int b)
    {
        int result;
        result = Random.Range(a, b);
        return result;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the other is the player 
        if (other.gameObject.tag == "Player")
        {
            PeopleDeath();
        }
    }

    public void PeopleDeath()
    {
        DestroyThisPerson();
    }

    public void DestroyThisPerson()
    {
        //send this objects destruction score, stat update and special to the StatManager.
        stm.currentScore += destructionScore;
        stm.peopleDestroyed++;
        pm.playerCurrentSpecial += specialScore;

        //add force to rb of object to boot it out of the way.
        //ppRB.AddForce(0.0f, colForce, 0.0f);

        //bring in a death FX from pool.
        sm.SpawnFXs(sm.ppDesFXPool, sm.selectedPpDesFX, gameObject);

        //bring in a death SFX from pool.
        sm.SpawnFXs(sm.sfxPpDesPool, sm.selectedSFXPpDes, gameObject);

        //deactivate and pop back into people pool.
        sm.BackToPool(sm.peoplePool, sm.peoplePoolPosition, this.gameObject);
    }


    public IEnumerator DeathSequence()
    {
        //send this objects destruction score, stat update and special to the StatManager.
        stm.currentScore += destructionScore;
        stm.peopleDestroyed++;
        pm.playerCurrentSpecial += specialScore;

        //add force to rb of object to boot it out of the way.
        //ppRB.AddForce(0.0f, colForce, 0.0f);

        //bring in a death FX from pool.
        sm.SpawnFXs(sm.ppDesFXPool, sm.selectedPpDesFX, gameObject);

        yield return new WaitForSeconds(0.5f);

        //bring in a death SFX from pool.
        sm.SpawnFXs(sm.sfxPpDesPool, sm.selectedSFXPpDes, gameObject);

        //deactivate and pop back into people pool.
        sm.BackToPool(sm.peoplePool, sm.peoplePoolPosition, this.gameObject);

        yield return null;
    }


}
