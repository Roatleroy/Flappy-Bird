using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//
[System.Serializable]
public class PlayerCreate
{
    public string Username;
    public string Unity_ID;
}
[System.Serializable]
public class PlayerData
{
    public string username;
    public int score;
}

[System.Serializable]
public class EndData
{
    public string unity_id;
    public int score = 0;
    public int gold = 0;
    public int silver = 0;
    public int bronze = 0;
}


// Functions other scripts use and beginning functions to get values to dynamically change with user input
public class LogicBoard : MonoBehaviour
{
    public EndData end = new EndData();
    public int score;
    public int prevScore = 0;

    public static int highestScore;
    public bool start = false;

    // Text on the scoreboard of the running game
    public Text scoreText;
    // Score on endgame screen
    public Text EndScore;
    // Highest Score of all runs on endGame screen
    public Text BestScore;


    public GameObject Score;
    public bool TRUF = false;

    public BirdCode Bird;
    public GameObject BirdObject;
    //-------------------- This a reference to the PipeSpawner game object
    public GameObject spawn;
    //----------------------- Play Menu
    public GameObject playButton;
    public GameObject LeaderBoardObject;
    public GameObject logo;
    //----------------------- End Menu
    public GameObject restartButton;
    public GameObject ScoreBoard;
    public GameObject highScore;
    public GameObject regularScore;
    // Rating Objects
    
    public GameObject Gold;
    public GameObject Silver;
    public GameObject Bronze;

    public GameObject BackGround;

    public PlayerCreate newPlayer = new PlayerCreate();
   
    // USER CREATION VARIABLES
    public GameObject CreateUser;
    public PlayerAPI playerCheck;
    public TMP_InputField usernameText;
    public Image myImageMany;
    public Image myImageTaken;


    public GameObject ENDimage;
    public GameObject EndButton;

    private Vector3 birdStartPosition;

    public Spawner value;


    // Gather initial bird position for reset function. 
    // Get 100 rows of user data in POSTGRES database
    // Use Unities Authentication to Grab unique code for that specific users version of the Game
    // If user already exists when unity checks the cloud continue with game ELSE create new user.
    private async void Awake()
    {
        birdStartPosition = BirdObject.transform.position;

        StartCoroutine(playerCheck.FetchScoresFromServer());

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        string playerId = AuthenticationService.Instance.PlayerId;

        Debug.Log("Player ID: " + playerId);

        end.unity_id = playerId;
        
        var response = await playerCheck.CheckPlayer(playerId);

        
        if (response != null)
        {
            highestScore = response.score;
            prevScore = response.score;
            Debug.Log("Welcome: " + response.username);
            Debug.Log("Score: " + response.score);

            startMenu();
        }
        else
        {
            CreateUser.SetActive(true);
            newPlayer.Unity_ID = playerId;
            Debug.Log("Player Doesn't Exist Yet");
        }
        
    }

    // When user pressed the EXIT BUTTON turns off every object that could possibly be on. 
    public void EndAssistant()
    {
        spawn.SetActive(false);
        playButton.SetActive(false);
        LeaderBoardObject.SetActive(false);
        logo.SetActive(false);
        Score.SetActive(false);
        ENDimage.SetActive(true); 
        ScoreBoard.SetActive(false);
        EndButton.SetActive(false);
        
        Bronze.SetActive(false);

        Silver.SetActive(false);

        Gold.SetActive(false); 
        
    }


    // When user presses Yes button post score and close game. Linked to Game Object
    public async void YesEnd()
    {
        if (end.score < prevScore && end.gold == 0 && end.silver == 0 && end.bronze == 0)
        {
            Application.Quit();
        }
        else
        {
            if (await playerCheck.PostEnd(end))
            {
                Application.Quit();
            }
            else
            {
                Debug.Log("EndGame Post Failed");
            }
        }
    }

    // When user presses NO button it resets the scene back to the main menu
    public void NoEnd()
    {
        ENDimage.SetActive(false);
        ResetScene();
    }


    // Every time the bird object goes through the score object add one to the score
    // If Score is 50 change background to nightime background
    public void Addition(int add)
    {
        score += add;
        scoreText.text = score.ToString();

        if (score == 25)
        {
            foreach (BackGroundMove BackMove in FindObjectsByType<BackGroundMove>())
            {
                
                Transform childTransform = BackMove.transform.Find("Daytime");
                Transform childTransform2 = BackMove.transform.Find("Nightime");

                if (childTransform != null && childTransform2 != null)
                {
                 
                    childTransform.gameObject.SetActive(false);
                    childTransform2.gameObject.SetActive(true);
                }
            }
        }
        if (score == 50)
        {
            foreach (BackGroundMove BackMove in FindObjectsByType<BackGroundMove>())
            {

                Transform childTransform = BackMove.transform.Find("Daytime");
                Transform childTransform2 = BackMove.transform.Find("Nightime");

                if (childTransform != null && childTransform2 != null)
                {

                    childTransform.gameObject.SetActive(true);
                    childTransform2.gameObject.SetActive(false);
                }
            }
        }
    }

    // When the Game starts up for the first time and user hits play it starts the game
    // Disables Menu Screen
    public void startGame() {

        spawn.SetActive(true);
        playButton.SetActive(false);
        LeaderBoardObject.SetActive(false);
        logo.SetActive(false);
        Score.SetActive(true);
        EndButton.SetActive(false);
        TRUF = true;
        value.canSpawn = true;
    }

    // Function called by branch when Unity cache can't find user session
    // Call create user function is PlayerAPI script to post UNITY_ID
    // User name has to be under 16 characters and over 1 character long and must not already exist
    // If those conditions are met display error messages. 
    public async void createUser()
    {
        string username = usernameText.text;

        if (username.Length > 15 || username.Length < 2)
        {
            myImageMany.gameObject.SetActive(true);
            myImageTaken.gameObject.SetActive(false);
            usernameText.text = "";
        }
        else if (!(await playerCheck.CheckUsername(username)))
        {
            newPlayer.Username = username;
           
            if (await playerCheck.PostPlayer(newPlayer))
            { 
                Debug.Log(usernameText.text + " Created");
                startMenu();
            }
           
        }
        else
        {
            myImageMany.gameObject.SetActive(false);
            myImageTaken.gameObject.SetActive(true);
            usernameText.text = "";
        }
    }

    // When user successfully creates a new use -> start game menu.
    // Reason not using reset scene is because those call background
    // objects that are not yet created and will cause null error
    public void startMenu()
    {
        CreateUser.SetActive(false);
        playButton.SetActive(true);
        logo.SetActive(true);
        EndButton.SetActive(true);
    }


    // Grab user data to get current leadboard stats just in case user got a better score
    // turn on leaderboard game object and display leaderboard. 
    public void startLeaderBoard()
    {
        StartCoroutine(playerCheck.FetchScoresFromServer());
        LeaderBoardObject.SetActive(true);
    }

    // This takes the input from BirdCode, when bird collides with collider it turns on end game menu
    // Depending on score displays Bronze, Silver, or Gold Sprite. 
    public void endGameMenu()
    {

        Score.SetActive(false);
        ScoreBoard.SetActive(true);
        EndButton.SetActive(true);
        Bird.enabled = false;
        highestScore = Mathf.Max(highestScore, score);

        //
        end.score = Mathf.Max(highestScore, score);
        //

        BestScore.text = highestScore.ToString();
        EndScore.text = scoreText.text;

        if (score > 9 && score < 15)
        {
            Bronze.SetActive(true);
            end.bronze += 1;
        }
        if(score > 14 && score < 20)
        {
            Silver.SetActive(true);
            end.silver += 1;
        }
        if(score > 19)
        {
            Gold.SetActive(true);
            end.gold += 1;
        }
    }

    // Turns menu back on and resets scene by turning off all other possible game objects
    // and turning on Main MENU objects and restarting the background objects to start moving and spawning again. 
    // Kill Pipes and to start spawning from beginning position again. 
    // Returns bird object back to X=0, reset pipe move speed to beginning speed. 
    public void ResetScene()
    {
        //Debug.Log("RESET START");

        Bronze.SetActive(false);
        Silver.SetActive(false);
        Gold.SetActive(false);
        playButton.SetActive(true);
        LeaderBoardObject.SetActive(false);
        Score.SetActive(false);
        TRUF = false;
        ScoreBoard.SetActive(false);

        foreach (GameObject pipe in GameObject.FindGameObjectsWithTag("PipeObject"))
        {
            Destroy(pipe);
        }
        value.pipeSpawned = null;

        foreach (GroundMove Ground in FindObjectsByType<GroundMove>())
        {
            Ground.enabled = true;
        }

        foreach (BackGroundMove BackMove in FindObjectsByType<BackGroundMove>())
        {
            BackMove.enabled = true;
            Transform childTransform = BackMove.transform.Find("Daytime");
            Transform childTransform2 = BackMove.transform.Find("Nightime");

            if (childTransform != null && childTransform2 != null)
            {

                childTransform.gameObject.SetActive(true);
                childTransform2.gameObject.SetActive(false);
            }
        }

        // Debug.Log("Before ground spawn");

        Bird.groundSpawn.enabled = true;

        //Debug.Log("After ground spawn");

        score = 0;
        scoreText.text = score.ToString();

       // Debug.Log("After score");

        Bird.enabled = true;

        //Debug.Log("After Bird");

        BirdObject.transform.position = birdStartPosition;

        Debug.Log(transform.position);
        // Output in Console: (1.2, 3.4, 0.0)


        //Debug.Log("After Bird position");

        PipeMove.moveSpeed = 15f;

        //Debug.Log("RESET COMPLETE");
    }
}
