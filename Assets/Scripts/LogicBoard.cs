using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class PlayerStats
{
    public int score;
    public int gold;
    public int silver;
    public int bronze;
}
public class LogicBoard : MonoBehaviour
{
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

    //-------------------- Json Variable Key:Value(Struct) Object
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

    private string serverUrl = "https://localhost:7130/api/game/scores";

    void Start()
    {
        spawn.SetActive(false);
        Score.SetActive(false);
        ScoreBoard.SetActive(false);

        StartCoroutine(FetchScoresFromServer());
    }

    [ContextMenu("Increase Scores")]
    public void Addition(int add)
    {
        score += add;
        scoreText.text = score.ToString();
    }

    // When the Game starts up for the first time and user hits play it starts the game
    // Disables Menu Screen
    public void startGameMenu() {

        spawn.SetActive(true);
        playButton.SetActive(false);
        LeaderBoardObject.SetActive(false);
        logo.SetActive(false);
        Score.SetActive(true);
        TRUF = true;
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
        BestScore.text = highestScore.ToString();
        EndScore.text = scoreText.text;

        if (score > 9 && score < 15)
        {
            Bronze.SetActive(true);
        }
        if(score > 14 && score < 20)
        {
            Silver.SetActive(true);
        }
        if(score > 19)
        {
            Gold.SetActive(true);
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PipeMove.moveSpeed = 14.5f;
    }

    IEnumerator FetchScoresFromServer()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl))
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

                if (leaderboard.Count > 3)
                {
                    First_Place.text = leaderboard[0].playerName;
                    Second_Place.text = leaderboard[1].playerName;
                    Third_Place.text = leaderboard[2].playerName;
                }

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
}
