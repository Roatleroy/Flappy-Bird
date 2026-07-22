using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdCode : MonoBehaviour
{
    //----------------------------  Bird Physics
    public Rigidbody2D myRigidbody;
    public float flapStrength;
    float targetRotation = -85f;
    public float rotationSpeedDown;
    public float rotationSpeedUp;
  
    float tiltUpTimer;
    public float tiltUpDuration;

    // --------------------------  END GAME
    private bool TRUF = true;
    public Spawner spawn;
    public AddScore upgrade;

    //--------------------------- Audio
    public AudioSource FLAP;
    public AudioSource DIE;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn = GameObject.FindGameObjectWithTag("PipeObject").GetComponent<Spawner>();
        upgrade = GameObject.FindGameObjectWithTag("Logic").GetComponent<AddScore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && TRUF)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            tiltUpTimer = tiltUpDuration;
            FLAP.Play();
        }
       
        tiltUpTimer -= Time.deltaTime;

        targetRotation = tiltUpTimer > 0f ? 10f : -85f;

        if (targetRotation == 10f)
        {
            myRigidbody.rotation = Mathf.MoveTowards(
                myRigidbody.rotation,
                targetRotation,
                rotationSpeedUp * Time.deltaTime
            );
        }
        else
        {
            myRigidbody.rotation = Mathf.MoveTowards(
                myRigidbody.rotation,
                targetRotation,
                rotationSpeedDown * Time.deltaTime
            );
        }

    }

    private void OnCollisionEnter2D(Collision2D Collision)
    {
        if (Collision.gameObject.CompareTag("Obstacle"))
        {
            DIE.Play();
            TRUF = false;

            foreach (PipeMove pipe in FindObjectsByType<PipeMove>())
            {
                pipe.enabled = false;
            }

            spawn.enabled = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.GameObject().layer == 3)
        {
            upgrade.Addition(1);
        }

    }

}