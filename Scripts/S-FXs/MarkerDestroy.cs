using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDestroy : MonoBehaviour
{
    PlayerController pc;
    UIManager ui;

    private ParticleSystem myPS;
    public GameObject selectedObject;

    void Start()
    {
        //
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        //
        myPS = GetComponent<ParticleSystem>();
        selectedObject = pc.target;
    }


    void Update()
    {
        //if the PlayerController target is null, then destroy this object after half a second.
        if(pc.target == null)
        {
            ui.markerCreated = false;
            Destroy(this.gameObject);
        }
        //check if the object this is on is still the target, if not then destroy.
        else if (selectedObject != pc.target)
        {
            ui.markerCreated = false;
            Destroy(this.gameObject);
        }
        //check if this is still playing, if it has stopped, then destroy.
        if(myPS.isStopped)
        {
            ui.markerCreated = false;
            Destroy(this.gameObject);
        }
    }
}
