using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneClear : MonoBehaviour
{
    public bool zoneIsClear = true;
    public int vehiclesColliding = 0;

    void Start()
    {

    }

    void Update()
    {
        if (vehiclesColliding == 0) {
            zoneIsClear = true;
        } else {
            zoneIsClear = false;
        }
    }
    
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        { 
            vehiclesColliding++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        { 
            vehiclesColliding--;
        }
    }
}
