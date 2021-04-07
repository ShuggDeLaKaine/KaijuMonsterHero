using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnManager sm;

    public bool canSpawn;
    public bool justSpawned;

    float cooldown = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        this.canSpawn = true;
        this.justSpawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCanSpawn();
    }

    //function that sets whether this spawner is ready to spawn again.
    void CheckCanSpawn()
    {
        if(this.justSpawned == true)
        {
            this.canSpawn = false;
            StartCoroutine(Cooldown());
        }
        else
        {
            this.canSpawn = true;
        }

    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        this.justSpawned = false;
        yield return new WaitForSeconds(cooldown);
        this.canSpawn = true;
    }
}


