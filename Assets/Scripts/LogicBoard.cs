using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LogicBoard : MonoBehaviour
{
    public int score;
    public Text scoreText;
    public bool start = false;

    //--------------------
    public GameObject spawn;
    public GameObject playButton;
    public GameObject tierButton;
    public GameObject logo;
    public GameObject Score;
    public bool TRUF = false;

    void Start()
    {
        spawn.SetActive(false);
        Score.SetActive(false);
    }

    [ContextMenu("Increase Scores")]
    public void Addition(int add)
    {
        score += add;
        scoreText.text = score.ToString();
    }

    public void startGameMenu() {

        spawn.SetActive(true);
        playButton.SetActive(false);
        tierButton.SetActive(false);
        logo.SetActive(false);
        Score.SetActive(true);
        TRUF = true;
    }
}
