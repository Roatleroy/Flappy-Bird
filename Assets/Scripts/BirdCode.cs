using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


//Manages the interactions between the bird object and the rigid bodies in the scene
public class BirdCode : MonoBehaviour
{
    //----------------------------  Bird Physics
    public Rigidbody2D myRigidbody;
    public float flapStrength;
    float targetRotation;
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

    //Spawn Ground Object so there is no null reference
    private void Start()
    {
        groundSpawn = GameObject.FindGameObjectWithTag("GroundSpawn").GetComponent<GroundSpawn>();
    }

    // For when menu starts and player doesn't the bird flaps above X=0 and when user initiates start it controls bird object.
    // Also sets behavior for bird object interacting with gravity and when flapping
    void Update()
    {
        // if either the space bar or left moust button is clicked and TRUF is true. TRUF is an true false indicator to indicate menu mode or game mode.
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && logicBoard.TRUF)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            tiltUpTimer = tiltUpDuration;
            FLAP.Play();
        }
        // if TRUF is in menu mode and position of X is less that 0 and user had indicated that they are in the menu either by starting the game or wrapping
        // back around from the score menu then manually flap the bird until user makes a menu decision to start the game
        if (logicBoard.TRUF == false && transform.position.y <= 0f && groundSpawn.enabled == true)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            tiltUpTimer = tiltUpDuration;
        }


        tiltUpTimer -= Time.deltaTime;

        targetRotation = tiltUpTimer > 0f ? 10f : -85f;

        //Roatate Bird Object UP to Z = 10
        if (targetRotation == 10f)
        {
            myRigidbody.rotation = Mathf.MoveTowards(
                myRigidbody.rotation,
                targetRotation,
                rotationSpeedUp * Time.deltaTime
            );
        }
        else //Roatate Bird Object DOWN Z=-85
        {
            myRigidbody.rotation = Mathf.MoveTowards(
                myRigidbody.rotation,
                targetRotation,
                rotationSpeedDown * Time.deltaTime
            );
        }
    }

    //When BirdObject Detects collision on Rigid Body. Find all Objects and stop them from spawning and moving. 
    // Send code to LOGIC BOARD to start menu. 
    private void OnCollisionEnter2D(Collision2D Collision)
    {
        if (Collision.gameObject.CompareTag("Obstacle") || Collision.gameObject.CompareTag("Floor"))
        {
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

            foreach(BackGroundMove BackMove in FindObjectsByType<BackGroundMove>())
            {
                BackMove.enabled = false;
            }

            groundSpawn.enabled = false;
            logicBoard.TRUF = false;

        }
        if (Collision.gameObject.CompareTag("Floor"))
        {
            logicBoard.endGameMenu();
            menu.Play();
        }
    }

}