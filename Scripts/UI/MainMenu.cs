using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public StatManager stm;
    public GameObject statsPanel;
    public GameObject timePanel;
    public GameObject clickSFX;

    private float oneMinute = 60.0f;
    private float twoMinutes = 120.0f;
    private float threeMinutes = 180.0f;

    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if ((scene.buildIndex != 1) && (scene.buildIndex != 2))
        { 
            stm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>();
        }
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().PlayMusic();
        PlayerPrefs.SetFloat("GameTime", 180.0f);
    }


    //click Play button - loads game scene.
    public void OnClickPlay()
    {
        PlayClick();
        SceneManager.LoadScene("Loading");
        //timePanel.SetActive(true);
    }

    //click Stats button - brings up stats panel
    public void OnClickStats()
    {
        Scene scene = SceneManager.GetActiveScene();

        //can only do in the main menu scene
        if (scene.buildIndex == 0)
        {
            PlayClick();
            statsPanel.SetActive(true);
        }
    }

    //button click exit stat panel.
    public void ClickOnExitStatButton()
    {
        PlayClick();
        statsPanel.SetActive(false);
    }

    //button click exit time length panel.
    public void ClickExitTimePanel()
    {
        PlayClick();
        timePanel.SetActive(false);
    }

    public void OneMinuteGame()
    {
        PlayClick();
        PlayerPrefs.SetFloat("GameTime", oneMinute);
        SceneManager.LoadScene("Loading");
    }

    public void TwoMinuteGame()
    {
        PlayClick();
        PlayerPrefs.SetFloat("GameTime", twoMinutes);
        SceneManager.LoadScene("Loading");
    }

    public void ThreeMinuteGame()
    {
        PlayClick();
        PlayerPrefs.SetFloat("GameTime", threeMinutes);
        SceneManager.LoadScene("Loading");
    }

    //click HowToPlay button - takes to scene displaying how to play details.
    public void OnClickHTP()
    {
        PlayClick();
        SceneManager.LoadScene("HowToPlay");
    }

    //click Credits button - takes to scene displaying credits for asset contributors.
    public void OnClickCredits()
    {
        PlayClick();
        SceneManager.LoadScene("Credits");
    }

    //click Exit button - exits application. 
    public void OnClickExit()
    {
        PlayClick();
        //save those total stats (high score etc).
        stm.SaveStats();

        Application.Quit();
    }

    //click To Main Menu button - goes to main menu scene.
    public void OnClickMainMenu()
    {
        PlayClick();
        SceneManager.LoadScene("MainMenu");
    }

    void PlayClick()
    {
        Instantiate(clickSFX, new Vector3(0, 0, 0), Quaternion.identity);
    }


}
