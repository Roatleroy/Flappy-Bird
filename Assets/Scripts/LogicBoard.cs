using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject tierButton;
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


    void Start()
    {
        spawn.SetActive(false);
        Score.SetActive(false);
        ScoreBoard.SetActive(false);
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
        tierButton.SetActive(false);
        logo.SetActive(false);
        Score.SetActive(true);
        TRUF = true;
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
}
