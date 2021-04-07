using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryManager : MonoBehaviour
{
    public StatManager stm;
    public SpawnManager sm;

    private Transform thisTransform;        //reference to Transform that will attached to player.
    private float yOffset = 1.0f;           //offset to be applied to FX instaniation so it slightly raised off ground.

    public GameObject destructionFX;        //reference to the FX gameobject that will be instantiated on collision with player.
    public AudioSource destructionAudio;    //reference to AudioSource to be attached to object

    public int destructionScore;            //score value of object to be passed to game manager onn destruction.


    void Start()
    {
        InitialiseObject();
    }


    void InitialiseObject()
    {
        stm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>();
        sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        thisTransform = GetComponent<Transform>();      //attaching Transform component to object.
        //isDestroyed = Animator.StringToHash("isDestroyed"); //setting animator bool isDestroyed from string to int, creates less garbage.
    }


    //OnCollisionEnter to check whether the object has collided with the players collision box.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            DestroyThis();
        }
    }


    void DestroyThis()
    {
        //send this objects destruction score to the StatManager
        stm.currentScore += destructionScore;

        //get FXs from its object pool and bring into scene.
        sm.SpawnFXs(sm.scnDesFXPool, sm.selectedScnDesFX, gameObject);

        //get SFX from its pool and bring into scene.
        sm.SpawnFXs(sm.sfxScnDesPool, sm.selectedSFXScnDes, gameObject);

        //deactive the object.
        gameObject.SetActive(false);
    }
}
