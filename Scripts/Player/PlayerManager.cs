using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    public float playerMaxHealth;
    public float playerCurrentHealth;

    public float playerMaxSpecial;
    public float playerCurrentSpecial;

    // Start is called before the first frame update
    void Start()
    {
        playerMaxHealth = 250.0f;
        playerCurrentHealth = playerMaxHealth;

        playerMaxSpecial = 100.0f;
        playerCurrentSpecial = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        CapSpecialValue();
    }

    //function to cap player special bar to max value of special bar.
    void CapSpecialValue()
    {
        if(playerCurrentSpecial >= playerMaxSpecial)
        {
            playerCurrentSpecial = playerMaxSpecial;
        }
    }
}
