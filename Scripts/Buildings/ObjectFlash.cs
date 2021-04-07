using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFlash : MonoBehaviour
{
    PlayerController pc;                    //reference to PlayerController script; needed for target.
    public GameObject selectedObject;       //reference that will take 'target' from PlayerController script.

    public int red;                         //int for colour red.
    public int green;                       //int for colour green.
    public int blue;                        //int for colour blue.

    public bool targetAcquired = false;     //bool for whether there is a target acquired.
    public bool startedFlashing = false;    //bool for whether the object has started flashing.
    public bool flashing = false;            //bool for whether object is currently flashing.

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        AcquiredTarget();
    }

    // Update is called once per frame
    void Update()
    {
        AcquiredTarget();

        if (targetAcquired == true)
        {
            selectedObject.GetComponent<Renderer>().material.color = new Color32((byte)red, (byte)green, (byte)blue, 255);
        }
    }

    //function to check whether there is an acquired target from the target in PlayerController script.
    //if target then sets selectedObject to that object and targetAcquired bool is true, and vice versa if not.
    void AcquiredTarget()
    {
        if(pc.target != null)
        {
            selectedObject = pc.target;
            targetAcquired = true;
            StartFlashing();
        }
        else
        {
            selectedObject = null;
            targetAcquired = false;
            StopFlashing();
        }
    }

    //function to start the flashing of the selected object.
    void StartFlashing()
    {
        if(startedFlashing == false)
        {
            startedFlashing = true;
            StartCoroutine(FlashObject());
        }
    }

    //function to stop the flashing of the selected object.
    void StopFlashing()
    {
        startedFlashing = false;
        StopCoroutine(FlashObject());
        if(targetAcquired == true)
        { 
            selectedObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        }
    }

    //IEnum to flash the material colour of the selected object.
    IEnumerator FlashObject()
    {
        while(targetAcquired == true)
        {
            yield return new WaitForSeconds(0.05f);
            if(flashing == true)
            {
                if (red <= 30)
                {
                    flashing = false;
                }
                else
                {
                    red -= 25;
                    green -= 1;
                }
            }
            if(flashing == false)
            {
                if (red >= 250)
                {
                    flashing = true;
                }
                else
                {
                    red += 25;
                    green += 1;
                }
            }
        }
    }


}
