using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarke : MonoBehaviour
{
    PlayerController pc;


    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pc.target == null)
        {
            Destroy(gameObject);
        }
    }
}
