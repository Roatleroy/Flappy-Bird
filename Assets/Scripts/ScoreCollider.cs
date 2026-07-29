using Unity.VisualScripting;
using UnityEngine;

public class ScoreCollider : MonoBehaviour
{
    public LogicBoard upgrade;
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       upgrade = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicBoard>();
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.GameObject().layer == 3)
        {
            upgrade.Addition(1);
        }

        audioSource.Play();

    }
}
