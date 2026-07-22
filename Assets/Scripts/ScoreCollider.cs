using Unity.VisualScripting;
using UnityEngine;

public class ScoreCollider : MonoBehaviour
{
    public AddScore upgrade;
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       upgrade = GameObject.FindGameObjectWithTag("Logic").GetComponent<AddScore>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
