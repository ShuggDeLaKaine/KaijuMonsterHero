using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{

    //public Image progressBar;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().PlayMusic();

        StartCoroutine(LoadingAsyncOperation());
    }

    IEnumerator LoadingAsyncOperation()
    {
        //create async operation to load Game scene.
        AsyncOperation game = SceneManager.LoadSceneAsync("Game");

        /*
        //while progress of load not complete, aka less than 1.
        while(game.progress < 1)
        {
            Debug.Log("load progress is: " + game.progress);
            //make the fill image of the loading bar equal to load progress value.
            progressBar.fillAmount = game.progress;
            yield return new WaitForEndOfFrame();
        }
        */

        yield return new WaitForEndOfFrame();

    }
}
