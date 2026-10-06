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
    private RigidbodyConstraints2D flightConstraints;
    private bool hasDied;
    private bool hasLanded;

    private void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        // Collisions must not compete with the scripted tilt.
        flightConstraints = myRigidbody.constraints | RigidbodyConstraints2D.FreezeRotation;
        myRigidbody.constraints = flightConstraints;
        myRigidbody.angularVelocity = 0f;
    }

    public void ResetBird(Vector3 startPosition)
    {
        myRigidbody.linearVelocity = Vector2.zero;
        myRigidbody.angularVelocity = 0f;
        myRigidbody.position = startPosition;
        myRigidbody.rotation = 0f;
        myRigidbody.constraints = flightConstraints;
        tiltUpTimer = 0f;
        targetRotation = 0f;
        hasDied = false;
        hasLanded = false;
        enabled = true;
        myRigidbody.WakeUp();
    }

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
        if (hasDied) return;
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


    }

    private void FixedUpdate()
    {
        if (hasLanded) return;

        tiltUpTimer -= Time.fixedDeltaTime;

        targetRotation = tiltUpTimer > 0f ? 10f : -85f;

        //Roatate Bird Object UP to Z = 10
        if (targetRotation == 10f)
        {
            myRigidbody.rotation = Mathf.MoveTowardsAngle(
                myRigidbody.rotation,
                targetRotation,
                rotationSpeedUp * Time.fixedDeltaTime
            );
        }
        else //Roatate Bird Object DOWN Z=-85
        {
            myRigidbody.rotation = Mathf.MoveTowardsAngle(
                myRigidbody.rotation,
                targetRotation,
                rotationSpeedDown * Time.fixedDeltaTime
            );
        }
    }

    //When BirdObject Detects collision on Rigid Body. Find all Objects and stop them from spawning and moving. 
    // Send code to LOGIC BOARD to start menu. 
    private void OnCollisionEnter2D(Collision2D Collision)
    {
        // Unity can deliver collision callbacks to disabled behaviours too.
        if (hasLanded) return;

        bool hitFloor = Collision.gameObject.CompareTag("Floor");
        if (!hasDied && (Collision.gameObject.CompareTag("Obstacle") || hitFloor))
        {
            hasDied = true;
            tiltUpTimer = 0f;
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
        if (hitFloor)
        {
            hasLanded = true;
            myRigidbody.linearVelocity = Vector2.zero;
            myRigidbody.angularVelocity = 0f;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            logicBoard.endGameMenu();
            menu.Play();
        }
    }

}
