using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    PlayerController pc;
    SpawnManager sm;
    StatManager stm;
    PlayerManager pm;

    private Transform thisTransform;
    public Animator buildingAnim;

    public GameObject damageFX;
    public GameObject destructionFX;
    public GameObject destroyedBuilding;

    public GameObject attackFromObject;
    public Vector3 attackPosition;

    public AudioSource damageAudio;
    public AudioSource destrutionAudio;

    public int buildingHealth;
    public int destructionScore;
    public int specialScore;
    private float yOffset = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = GetComponent<Transform>();
        buildingAnim = GetComponent<Animator>();

        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        stm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>();

        attackPosition = attackFromObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        BuildingHealthCheck();
    }

    void BuildingHealthCheck()
    {
        if (buildingHealth <= 0)
        {
            DestroyThis();
        }
    }

    void DestroyThis()
    {
        //send this objects destruction score, special and stats to the StatManager.
        stm.currentScore += destructionScore;
        stm.buildingsDestroyed++;
        pm.playerCurrentSpecial += specialScore;

        //check whether it is a building or a factory.
        if(gameObject.tag == "Factory")
        {
            //get FXs from its object pool and bring into scene at this objects position.
            sm.SpawnFXs(sm.factDesFXPool, sm.selectedFactDesFX, gameObject);
            sm.SpawnFXs(sm.sfxFactDesPool, sm.selectedSFXFactDes, gameObject);
        }
        else
        {
            //get FXs from its object pool and bring into scene at this objects position.
            sm.SpawnFXs(sm.buildExFXPool, sm.selectedBuildExFX, gameObject);
            sm.SpawnFXs(sm.sfxBuildDesPool, sm.selectedSFXBuildDes, gameObject);
        }

        //replace ok building prefab with destroyed building prefab.
        gameObject.SetActive(false);
        //destroyedBuilding.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(pc.attackMode == true && pc.target == gameObject)
            {
                ContactPoint contact = collision.contacts[0];

                //SpawnFXs function needs a gameObject for the location
                //need to put the objects transform.position at point of contact and use that as location for spawn. 
                GameObject contactObject = new GameObject();
                contactObject.transform.position = contact.point;

                //get that building damage FX from its pool.
                sm.SpawnFXs(sm.buildDAMFXPool, sm.selectedBuildDAMFX, contactObject);
                sm.SpawnFXs(sm.sfxBuildDamPool, sm.selectedSFXBuildDam, contactObject);

                buildingHealth -= pc.damageToGive;
            }
        }
    }

}
