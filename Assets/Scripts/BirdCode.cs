using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    public Spawner spawn;
    public GroundSpawn groundSpawn;
    public LogicBoard logicBoard;

    //--------------------------- Audio
    public AudioSource FLAP;
    public AudioSource DIE;
    public AudioSource menu;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && logicBoard.TRUF)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            tiltUpTimer = tiltUpDuration;
            FLAP.Play();
        }
        if (logicBoard.TRUF == false && transform.position.y <= 0f)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            tiltUpTimer = tiltUpDuration;
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
            groundSpawn = GameObject.FindGameObjectWithTag("GroundSpawn").GetComponent<GroundSpawn>();
            DIE.Play();

            foreach (PipeMove pipe in FindObjectsByType<PipeMove>())
            {
                pipe.enabled = false;

                Transform bottomPipe = pipe.transform.Find("Bottom Pipe");
                bottomPipe.GetComponent<BoxCollider2D>().enabled = false;
            }

            foreach(GroundMove Ground in FindObjectsByType<GroundMove>())
            {
                Ground.enabled = false;
            }

            groundSpawn.enabled = false;
            spawn.enabled = false;

        }
        else
        {
            logicBoard.endGameMenu();
            menu.Play();
        }
    }

}