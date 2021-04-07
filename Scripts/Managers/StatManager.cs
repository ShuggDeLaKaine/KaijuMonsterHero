using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    //reference to other required scripts.
    public UIManager ui;

    //ints for current and high scores.
    public int currentScore;
    public int highScore;

    //ints for number of things destroyed in current game.
    public int buildingsDestroyed;
    public int peopleDestroyed;
    public int vehiclesDestroyed;

    //ints for number of things destroyed overall.
    public int totalBuildingsDestroyed;
    public int totalPeopleDestroyed;
    public int totalVehiclesDestroyed;

    //DEBGGING: making sure scores are being saved, displaying in main menu to check.
    public Text highScoreText;
    public Text buildingsDesText;
    public Text vehiclesDesText;
    public Text peopleDesText;


    void Start()
    {
        //grab which scene this is.
        Scene scene = SceneManager.GetActiveScene();

        //load all the necessary stats.
        GetStats();

        if (scene.buildIndex != 0)
        { 
            ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
            currentScore = 0;
            buildingsDestroyed = 0;
            peopleDestroyed = 0;
            vehiclesDestroyed = 0;
        }
        else
        {
            //in the main menu scene.
            //set the main menu highScore to the stored high score value.
            highScoreText.text = highScore.ToString();
            buildingsDesText.text = totalBuildingsDestroyed.ToString();
            vehiclesDesText.text = totalVehiclesDestroyed.ToString();
            peopleDesText.text = totalPeopleDestroyed.ToString();
        }
    }


    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex != 0)
        { 
            UpdateUIScore();
        }
    }


    //function to update UI score text with the current score.
    void UpdateUIScore()
    {
        ui.gameScore = currentScore; 
    }

    //function on start, get all the stored stats. 
    void GetStats()
    {
        //get the high score.
        highScore = PlayerPrefs.GetInt("HighScore");
        //get the stats for total number of things destroyed.
        totalBuildingsDestroyed = PlayerPrefs.GetInt("BuildingsDestroyed");
        totalPeopleDestroyed = PlayerPrefs.GetInt("PeopleDestroyed");
        totalVehiclesDestroyed = PlayerPrefs.GetInt("VehiclesDestroyed");
    }

    //function on game exit, save all those total stats.
    public void SaveStats()
    {
        PlayerPrefs.SetInt("HighScore", highScore);

        PlayerPrefs.SetInt("BuildingsDestroyed", totalBuildingsDestroyed);
        PlayerPrefs.SetInt("PeopleDestroyed", totalPeopleDestroyed);
        PlayerPrefs.SetInt("VehiclesDestroyed", totalVehiclesDestroyed);
    }

    //function to add current game stats to total stats at end of game.
    public void UpdateTotals()
    {
        totalBuildingsDestroyed += buildingsDestroyed;
        totalVehiclesDestroyed += vehiclesDestroyed;
        totalPeopleDestroyed += peopleDestroyed;
    }
}
