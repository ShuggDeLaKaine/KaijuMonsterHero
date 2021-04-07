using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXDestroy : MonoBehaviour
{
    SpawnManager sm;
    private ParticleSystem myPS;        //reference to ParticleSystem that will be placed on object.


    void Start()
    {
        myPS = GetComponent<ParticleSystem>();      //attaching the ParticleSustem component to object.
        sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
    }

    void Update()
    {
        //check whether WalkIcon, if so destroy after 4 seconds.  
        if (gameObject.tag == "WalkIcon")
        {
            Destroy(gameObject, 2.0f);
        }
        //otherwise not WalkIcon, check if it has stopped playing and send back to pool.
        else if (myPS.isStopped)
        {
            //scenery destruction
            if (gameObject.tag == "ScnDesFX")
            {
                sm.BackToPool(sm.scnDesFXPool, sm.scnDesFXPoolPosition, gameObject);
            }
            //vehicle destruction
            else if (gameObject.tag == "CarExFX")
            {
                sm.BackToPool(sm.carExFXPool, sm.carExFXPoolPosition, gameObject);
            }
            //this is for the building damage
            else if (gameObject.tag == "BuildExFX")
            {
                sm.BackToPool(sm.buildExFXPool, sm.buildExFXPoolPosition, gameObject);
            }
            //building destruction
            else if (gameObject.tag == "BuildDesFX")
            {
                sm.BackToPool(sm.buildDAMFXPool, sm.buildDAMFXPoolPosition, gameObject);
            }
            //factory destruction
            else if (gameObject.tag == "FactDesFX")
            {
                sm.BackToPool(sm.factDesFXPool, sm.factDesFXPoolPosition, gameObject);
            }
            else if (gameObject.tag == "PeopleDesFX")
            {
                sm.BackToPool(sm.ppDesFXPool, sm.ppDesFXPoolPosition, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }



}
