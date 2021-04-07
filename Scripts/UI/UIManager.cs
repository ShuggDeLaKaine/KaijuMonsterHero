using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //references to relevant other scripts.
    public PlayerManager pm;
    public PlayerController pc;
    public CameraController cc;
    public StatManager stm;
    public MainMenu mm;

    //references for the various UI pop-up panels.
    public GameObject menuPopUpPanel;
    public GameObject areYouSurePanel;
    public GameObject statsPanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public GameObject notMutedButton;
    public GameObject mutedButton;

    //objects for the Target Marker FX
    public GameObject selectedObject;
    public GameObject targetMarker;
    public bool markerCreated;

    //texts for score, highscore and time.
    public Text scoreText;
    public Text timeText;
    public Text endScoreText;
    public Text highScoreText;

    //references to text elements in the statisitcs panel.
    public Text currentScoreText;
    public Text buildDesText;
    public Text vehDesText;
    public Text ppDesText;
    public Text totalScoreText;
    public Text totalBuildDesText;
    public Text totalVehDesText;
    public Text totalPpDesText;

    //player interactable objects on sidebar.
    public GameObject newHighScore;
    public GameObject walkText;
    public GameObject attackText;
    public GameObject specialText01;
    public GameObject specialText02;

    //shaded out unavailable buttons.
    public GameObject walkShade;
    public GameObject attackShade;
    public GameObject specialShade;

    //reference to click SFX.
    public GameObject clickSFX;

    //references to sliders.
    public Slider healthSlider;
    public Slider specialSlider;

    //vars to store time and score values.
    public int gameScore;
    public float timeLeft;
    private string minutes;
    private string seconds;

    //bools for pausing, muting and game over.
    public bool isPaused = false;
    public static bool isMuted = false;
    public bool timesUp;

    public bool attackPress = false;
    public bool specialPress = false;
    public bool canSpecialAttack = false;


    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex != 0)
        { 
            InitScene();
        } 
    }


    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex != 0)
        { 
            if(timesUp == false)
            {
                AcquiredTarget();
                UpdateScoreAndTime();
                UpdateStatBars();
                UpdateCanSpecialAttack();
            }
        }
    }


    //function to ran at beginning of scene, initiates all that needs to be.
    void InitScene()
    {
        //attaching referenced PlayerManager, PlayerController & Camera Controller scripts.
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        //stm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>();
        //mm = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<MainMenu>();

        //finding and attaching references for the score and time texts.
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();

        //finding and attaching references for the health and special sliders.
        healthSlider = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        specialSlider = GameObject.FindGameObjectWithTag("SpecialBar").GetComponent<Slider>();

        //setting max and current values of sliders to the relevant values in PlayerManager script.
        UpdateStatBars();

        //
        walkText.SetActive(true);
        attackText.SetActive(false);
        specialText01.SetActive(false);
        specialText02.SetActive(true);

        //
        walkShade.SetActive(false);
        attackShade.SetActive(true);
        specialShade.SetActive(true);

        //
        markerCreated = false;

        //setting the text for score to the value of gameScore.
        scoreText.text = gameScore.ToString();

        //populate the statistic with historic total amounts.
        TotalStatisticsPopulate();

        //getting the time length of game chosen by player.
        timeLeft = PlayerPrefs.GetFloat("GameTime");
        timesUp = false;

        //setting timeLeft to minutes and seconds and then setting to time text string. 
        minutes = Mathf.Floor(timeLeft / 60).ToString("0");
        seconds = Mathf.Floor(timeLeft % 60).ToString("00");
        timeText.text = minutes + ":" + seconds;

        //check whether muted or not and set relevant mute/unmute button active in menu panel.
        if(isMuted == false)
        {
            notMutedButton.SetActive(true);
            mutedButton.SetActive(false);
        }
        else if (isMuted == true)
        {
            notMutedButton.SetActive(false);
            mutedButton.SetActive(true);
        }

        //opening to check to see if menu panel enabled, if so disable.
        if (menuPopUpPanel.activeSelf == true)
        {
            menuPopUpPanel.SetActive(false);
        }

        //checking if the game is paused, if it is then unpause.
        if (Time.timeScale == 0.0f)
        {
            Resume();
        }
    }

    //function to be called in Update that matches the special and health stats in PlayerManager
    //script the the health and special sliders.
    void UpdateStatBars()
    {
        //setting max and current values of sliders to the relevant values in PlayerManager script.
        healthSlider.maxValue = pm.playerMaxHealth;
        healthSlider.value = pm.playerCurrentHealth;
        specialSlider.maxValue = pm.playerMaxSpecial;
        specialSlider.value = pm.playerCurrentSpecial;
    }

    //function to calculate time and put into minutes:seconds, plus score updater.
    void UpdateScoreAndTime()
    {
        //setting the text for score to the value of gameScore.
        scoreText.text = gameScore.ToString();

        //setting timeLeft to minutes and seconds and then setting to time text string.
        if (timeLeft > 0.0f)
        {
            timesUp = false;
            timeLeft -= Time.deltaTime;
            minutes = Mathf.Floor(timeLeft / 60).ToString("0");
            seconds = Mathf.Floor(timeLeft % 60).ToString("00");
            timeText.text = minutes + ":" + seconds;
        }
        else
        {
            timeText.text = "0:00";
            timesUp = true;
            GamesOver();
        }
    }

    //function to get total stats and high and fill those at beginning of scene.
    void TotalStatisticsPopulate()
    {
        totalScoreText.text = stm.highScore.ToString();
        totalBuildDesText.text = stm.totalBuildingsDestroyed.ToString();
        totalVehDesText.text = stm.totalVehiclesDestroyed.ToString();
        totalPpDesText.text = stm.totalPeopleDestroyed.ToString();
    }

    //function to fill out current game stats in teh statistics panel.
    public void StatisticsPanelFill()
    {
        currentScoreText.text = stm.currentScore.ToString();
        buildDesText.text = stm.buildingsDestroyed.ToString();
        vehDesText.text = stm.vehiclesDestroyed.ToString();
        ppDesText.text = stm.peopleDestroyed.ToString();
    }

    //function for when the time is up and it's game over. 
    void GamesOver()
    {
        //first pause game
        Pause();

        //set the current and high scores.
        endScoreText.text = gameScore.ToString();
        highScoreText.text = stm.highScore.ToString();

        //pop up game over menu
        gameOverPanel.SetActive(true);

        //checking whether it's a new high score and displaying relevant text if it is.
        if(gameScore > stm.highScore)
        {
            newHighScore.SetActive(true);
            stm.highScore = gameScore;
            PlayerPrefs.SetInt("HighScore", stm.highScore);
            
        }

        //update total stats with added stats from current game.
        stm.UpdateTotals();

        //saving stats earnt during game.
        stm.SaveStats();
    }

    //function checking whether the special attack bar is full and changing UI if it is.
    void UpdateCanSpecialAttack()
    {
        if(pm.playerCurrentSpecial == pm.playerMaxSpecial)
        {
            canSpecialAttack = true;
            specialText01.SetActive(true);
            specialText02.SetActive(false);
            specialShade.SetActive(false);
        }
        else
        {
            canSpecialAttack = false;
            specialText01.SetActive(false);
            specialText02.SetActive(true);
            specialShade.SetActive(true);
        }
    }

    void PlayClick()
    {
        Instantiate(clickSFX, new Vector3(0, 0, 0), Quaternion.identity);
    }

    //function for clicking on 'Menu Button'
    public void ClickOnMenuButton()
    {
        PlayClick();
        //check that the gameObject reference menuPopUpPanel is not set to nothing.
        if(menuPopUpPanel != null)
        {
            //pop-up panel enabled
            menuPopUpPanel.SetActive(true);
            //pause game
            Pause();
        }
    }

    //function for clicking on the 'Exit Menu Button'
    public void ClickOnMenuExitButton()
    {
        PlayClick();
        //check Menu Pop-Up Panel is active.
        if (menuPopUpPanel.activeSelf == true)
        {
            //set menu panel to disabled.
            menuPopUpPanel.SetActive(false);
            //unpause the game.
            Resume();
        }
    }

    //function to bring Are You Sure pop-up panel when selecting to quit the game.
    public void AreYouSure()
    {
        PlayClick();
        if (menuPopUpPanel.activeSelf == true)
        {
            //deactive menu panel
            menuPopUpPanel.SetActive(false);
            //activate are you sure panel
            areYouSurePanel.SetActive(true);
        }
    }

    //function for No button selection in areYouSurePanel.
    public void NoReturnToGame()
    {
        PlayClick();
        if (areYouSurePanel.activeSelf == true)
        {
            //deactivate are you sure panel.
            areYouSurePanel.SetActive(false);
            //activate menu pop up panel.
            menuPopUpPanel.SetActive(true);
        }
    }

    //function for button click on walk mode.
    public void ClickOnWalkButton()
    {
        pc.walkMode = true;
        pc.attackMode = false;
        pc.specialMode = false;

        walkText.SetActive(true);
        attackText.SetActive(false);
        specialText01.SetActive(false);

        walkShade.SetActive(false);
        attackShade.SetActive(true);
        specialShade.SetActive(true);
    }

    //function for button click into attack mode.
    public void ClickOnAttackButton()
    {
        PlayClick();

        pc.walkMode = false;
        pc.attackMode = true;
        pc.specialMode = false;

        walkText.SetActive(false);
        attackText.SetActive(true);
        specialText01.SetActive(false);

        walkShade.SetActive(true);
        attackShade.SetActive(false);
        specialShade.SetActive(true);
    }

    //function to public click on the special attack button.
    public void ClickOnSpecialButton()
    {
        PlayClick();

        if (canSpecialAttack == true)
        { 
            pc.walkMode = false;
            pc.attackMode = false;
            pc.specialMode = true;

            walkText.SetActive(false);
            attackText.SetActive(false);
            specialText01.SetActive(true);
            specialText02.SetActive(false);

            walkShade.SetActive(true);
            attackShade.SetActive(true);
            specialShade.SetActive(false);
        }
        else
        {
            //pop up text saying need to fill special bar.
            specialText01.SetActive(false);
            specialText02.SetActive(true);
            specialShade.SetActive(true);
        }
    }

    //function for a click on the camera switch button.
    public void ClickOnCameraSwitchButton()
    {
        PlayClick();

        cc.SwitchCamera();
    }

    //function to bring up the stats panel from the menu pop-up. Uses timesUp to see if game is over and which panel needs activating.
    public void ClickOnStatsButton()
    {
        PlayClick();

        //fill stats panel (only when needed rather than in update).
        StatisticsPanelFill();

        //set panel to active.
        statsPanel.SetActive(true);

        //check which previous panel needs turning off.
        if (timesUp == true)
        {
            gameOverPanel.SetActive(false);
        }
        else
        { 
            menuPopUpPanel.SetActive(false);
        }
    }


    //function for exiting stats panel, checks whether time is up (game has ended) on which panel to bring up on exit.
    public void ClickOnExitStatButton()
    {
        PlayClick();

        statsPanel.SetActive(false);

        if (timesUp == true)
        {
            
            gameOverPanel.SetActive(true);
        }
        else
        { 
            menuPopUpPanel.SetActive(true);
        }
    }

    //function to bring up the setting panel from the menu pop-up.
    public void ClickOnSettingsButton()
    {
        PlayClick();

        settingsPanel.SetActive(true);
        menuPopUpPanel.SetActive(false);
    }

    //function to exit the settings panel.
    public void ClickOnExitSettingsButton()
    {
        PlayClick();

        settingsPanel.SetActive(false);
        menuPopUpPanel.SetActive(true);
    }

    //function for toggling the mute/unmute option in the menu pop-up panel.
    public void ClickOnMuteButton()
    {
        PlayClick();

        if (isMuted == true)
        {
            //unmute the game 
            MuteSound();
            //change icon so the unmute button is active and current button is deactivated.
            notMutedButton.SetActive(true);
            mutedButton.SetActive(false);
            isMuted = false;
        }
        else if (isMuted == false)
        {
            //mute the game.
            MuteSound();
            //change icon so mute button is active and current button is deactivated.
            notMutedButton.SetActive(false);
            mutedButton.SetActive(true);
            isMuted = true;
        }
    }

    //function to great a target marker around a target building.
    void AcquiredTarget()
    {
        if(pc != null)
        { 
            if(markerCreated == false)
            { 
                if(pc.target != null)
                {
                    selectedObject = pc.target;
                    Instantiate(targetMarker, selectedObject.transform.position, Quaternion.identity);
                    markerCreated = true;
                }
            }
        }
    }

    //function to mute and unmute all sound.
    void MuteSound()
    {
        PlayClick();
        if (isMuted) { 
            AudioListener.volume = 1.0f;
        }  else {
            AudioListener.volume = 0.0f;
        }
    }

    //function to resume game after pausing.
    void Resume()
    {
        if (isPaused == true)
        { 
            Time.timeScale = 1.0f;
            isPaused = false;
        }
    }

    //function to pause game. 
    void Pause()
    {
        if(isPaused != true)
        { 
            Time.timeScale = 0.0f;
            isPaused = true;
        }
    }

}
