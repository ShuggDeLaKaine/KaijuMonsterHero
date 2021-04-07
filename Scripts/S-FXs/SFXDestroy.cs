using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXDestroy : MonoBehaviour
{
    private AudioSource myAS;       //reference to AudioSource that will placed on object.

    void Awake()
    {
        myAS = GetComponent<AudioSource>();     //attaching the AudioSource component to object
        AudioRandomiser();
        myAS.Play();
    }

    void Update()
    {
        DestroyCheck();
    }

    //Function to take an audio source and apply a constrained random effect to pitch and volume.
    void AudioRandomiser()
    {
        float pitchMin = 0.5f;
        float pitchMax = 1.5f;
        float volumeMin = 0.8f;
        float volumeMax = 1.2f;

        myAS.pitch = Random.Range(pitchMin, pitchMax);
        myAS.volume = Random.Range(volumeMin, volumeMax);
    }

    void DestroyCheck()
    {
        //if audio souce is not playing.
        if (!myAS.isPlaying)
        {
            //then destroy.
            Destroy(gameObject);
        }
    }
}
