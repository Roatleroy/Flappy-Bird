using Unity.VisualScripting;
using UnityEngine;


// When pipe object's score collider is interacted with update score by 1
public class ScoreCollider : MonoBehaviour
{
    public LogicBoard upgrade;
    public AudioSource audioSource;
    

    // assign upgrade variable with pipe object find with unity tag "Logic"
    void Start()
    {
       upgrade = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicBoard>();
    }

    //Unity Built in Function to detect when Score Collider and Bird object interact
    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.GameObject().layer == 3)
        {
            upgrade.Addition(1);
        }

        audioSource.Play();

    }
}
