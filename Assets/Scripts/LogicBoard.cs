using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
    public int score;
    public int gold;
    public int silver;
    public int bronze;
}

public class LogicBoard : MonoBehaviour
{
    public EndData end = new EndData();
    public int score;
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
/*
    //-------------------- Json Variable Key:Value(Struct) Object
 
    [System.Serializable]
    public class PlayerStats
    {
        public int score;
        public int gold;
        public int silver;
        public int bronze;
    }
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public PlayerStats stats;
    }

    public List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

    //----- Game Object for LeaderBoard Username Spawning
    public GameObject textMeshPrefab;
    public Transform uiContentContainer;

    public TextMeshProUGUI First_Place;
    public TextMeshProUGUI Second_Place;
    public TextMeshProUGUI Third_Place;

    private string GetserverUrl = "https://localhost:7130/api/game/scores";
    */

    public PlayerCreate newPlayer = new PlayerCreate();
   
    // USER CREATION VARIABLES
    public GameObject CreateUser;
    public PlayerAPI playerCheck;
    public TMP_InputField usernameText;
    public Image myImageMany;
    public Image myImageTaken;

    private Vector3 birdStartPosition;

    public Spawner value;

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

        //Debug.Log("Player ID: " + playerId);

        end.unity_id = playerId;
        
        var response = await playerCheck.CheckPlayer(playerId);

        end.score = response.score;

        if (response != null)
        {
            //Debug.Log(response.username);
            //Debug.Log(response.score);

            startMenu();
        }
        else
        {
            newPlayer.Unity_ID = playerId;
            Debug.Log("Player Doesn't Exist Yet");
        }
        
    }

    void Start()
    {
        
    }

    public void Addition(int add)
    {
        score += add;
        scoreText.text = score.ToString();
    }

    // When the Game starts up for the first time and user hits play it starts the game
    // Disables Menu Screen
    public void startGame() {

        spawn.SetActive(true);
        playButton.SetActive(false);
        LeaderBoardObject.SetActive(false);
        logo.SetActive(false);
        Score.SetActive(true);
        TRUF = true;
        value.canSpawn = true;
    }


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
    public void startMenu()
    {
        CreateUser.SetActive(false);
        playButton.SetActive(true);
        logo.SetActive(true);
    }

    public void startLeaderBoard()
    {
        LeaderBoardObject.SetActive(true);
    }

    // This takes the input from BirdCode, when bird collides with collider it puts the end game menu on
    public void endGameMenu()
    {

        Score.SetActive(false);
        ScoreBoard.SetActive(true);
        Bird.enabled = false;
        highestScore = Mathf.Max(highestScore, score);

        //
        end.score = Mathf.Max(highestScore, end.score);
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

    public void ResetScene()
    { 
        Debug.Log("RESET START");

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
        }

        Debug.Log("Before ground spawn");

        Bird.groundSpawn.enabled = true;

        Debug.Log("After ground spawn");

        score = 0;
        scoreText.text = score.ToString();

        Debug.Log("After score");

        Bird.enabled = true;

        Debug.Log("After Bird");

        BirdObject.transform.position = birdStartPosition;

        Debug.Log("After Bird position");

        PipeMove.moveSpeed = 14.5f;

        Debug.Log("RESET COMPLETE");
    }


    /*
    IEnumerator FetchScoresFromServer()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetserverUrl))
        {
            // Crucial for testing: allows Unity to trust Visual Studio's local SSL certificate
            webRequest.certificateHandler = new BypassCertificateHandler();

            Debug.Log("Pinging the game server...");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = webRequest.downloadHandler.text;
                Debug.Log("Successfully fetched data from server: " + jsonResult);

                // Next, you will parse this jsonResult to update your UI!
                var dict = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(jsonResult);

                leaderboard.Clear();
                foreach (Transform child in uiContentContainer)
                {
                    Destroy(child.gameObject);
                }

                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        leaderboard.Add(new LeaderboardEntry { playerName = kvp.Key, stats = kvp.Value });
                    }
                }

                switch(leaderboard.Count)
                {
                    case 0:
                        break;
                    case 1:
                        First_Place.text = leaderboard[0].playerName;
                        break;
                    case 2:
                        First_Place.text = leaderboard[0].playerName;
                        Second_Place.text = leaderboard[1].playerName;
                        break;
                    case 3:
                        First_Place.text = leaderboard[0].playerName;
                        Second_Place.text = leaderboard[1].playerName;
                        Third_Place.text = leaderboard[2].playerName;
                        break;
                    default:
                        First_Place.text = leaderboard[0].playerName;
                        Second_Place.text = leaderboard[1].playerName;
                        Third_Place.text = leaderboard[2].playerName;
                        for (int i = 3; i < leaderboard.Count; i++)
                        {
                            LeaderboardEntry entry = leaderboard[i];

                            GameObject TextRow = Instantiate(textMeshPrefab, uiContentContainer);

                            TextMeshProUGUI nameText = TextRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI scoreText = TextRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI medalText = TextRow.transform.GetChild(2).GetComponent<TextMeshProUGUI>();


                            nameText.text = entry.playerName;
                            scoreText.text = entry.stats.score.ToString();

                            // Optional: Combine gold, silver, and bronze into the third medal column
                            medalText.text = $"{entry.stats.gold}   {entry.stats.silver}   {entry.stats.bronze}";
                        }
                        break;

                }
            }
            else
            {
                Debug.LogError("Server Connection Failed: " + webRequest.error);
            }
        }
    }

    // Helper class so Unity doesn't block local development certificates
    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
    */
}
