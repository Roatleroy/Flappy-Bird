using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;

public class AddScore : MonoBehaviour
{
    public int score;
    public Text scoreText;

    [ContextMenu("Increase Scores")]
    public void Addition(int add)
    {
        score += add;
        scoreText.text = score.ToString();
    }
}
