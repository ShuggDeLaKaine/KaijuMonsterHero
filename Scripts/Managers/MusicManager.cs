using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource music;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if(objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        music = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if(music.isPlaying)
        {
            return;
        }
        music.Play();
    }

    public void StopMusic()
    {
        music.Stop();
    }

}
