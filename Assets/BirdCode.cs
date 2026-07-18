using UnityEngine;
using UnityEngine.InputSystem;

public class BirdCode : MonoBehaviour
{

    public Rigidbody2D myRigidbody;
    public float flapStrength;
    float targetRotation = -85f;
    public float rotationSpeedDown;
    public float rotationSpeedUp;

    float tiltUpTimer;
    public float tiltUpDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            tiltUpTimer = tiltUpDuration;
        }
       
        tiltUpTimer -= Time.deltaTime;

        targetRotation = tiltUpTimer > 0f ? 20f : -85f;

        if (targetRotation == 20f)
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
}