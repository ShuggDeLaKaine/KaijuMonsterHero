using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeopleManager : MonoBehaviour
{
    public Rigidbody ppRB;
    private Transform ppTrans;
    private Transform monTrans;
    private NavMeshAgent ppAgent;
    public AudioSource deathAudio;
    public ParticleSystem deathFX;
    public GameObject deathObject;

    public bool monsterSeen;
    public bool isRunning;

    public float EnemyRunDistance = 10.0f;
    public float colForce;
    private float minColForce;
    private float maxColForce;

    private float yOffset = 1.0f;       //offset to be applied to FX instaniation so it slightly raised off ground.
    public int killScore;


    // Start is called before the first frame update
    void Start()
    {
        ppRB = GetComponent<Rigidbody>();
        ppTrans = GetComponent<Transform>();
        ppAgent = GetComponent<NavMeshAgent>();
        monTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        minColForce = 1.5f;
        maxColForce = 5.0f;
        colForce = Random.Range(minColForce, maxColForce);

        monsterSeen = false;
        isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckForMonster()
    {
        if(monsterSeen == true)
        {
            Vector3 directionToMonster = ppTrans.position - monTrans.position;
            Vector3 newPosition = ppTrans.position + directionToMonster;
            ppAgent.SetDestination(newPosition);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //if the other is the player 
        //MAYBE also in stomp mode
        if(other.gameObject.name == "Player")
        {
            Debug.Log(this.gameObject.name + "collided with " + other.gameObject.name);
            StartCoroutine(DeathSequence());
        }


    }

    IEnumerator DeathSequence()
    {
        //add force to rb of object to boot it out of the way.
        //ppRB.AddForce(0.0f, colForce, 0.0f);

        //instantiate death FX.
        //Instantiate(deathFX, new Vector3(ppTrans.position.x, (ppTrans.position.y + yOffset), ppTrans.position.z), Quaternion.identity);

        //instantiate deaht object.
        //Instantiate(deathObject, ppTrans.position, Quaternion.identity);

        yield return new WaitForSeconds(2.0f);

        //Instantiate death SFX
        //Instantiate(deathAudio, ppTrans.position, Quaternion.identity);

        //destroying game object.
        Destroy(this.gameObject);

        yield return null;
    }

}
