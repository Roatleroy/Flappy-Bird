using UnityEngine;


// Move Pipe object until the point in which we despawn the object
public class PipeMove : MonoBehaviour
{
    public static float moveSpeed = 15f;
    public float speedIncrease = 0.02f;
    public float deadzone = -65f;

    //Increase the speed of the next pipe to make dodging pipes harder
    void Start()
    {
        moveSpeed += speedIncrease;
       // Debug.Log(moveSpeed);
    }

    //Move pipe by movement speed moved across real time units
    void Update()
    {
        transform.position += Vector3.left * (moveSpeed * Time.deltaTime);

        if (transform.position.x < deadzone)
        {
            //Debug.Log("Pipe Cleared"); 
            Destroy(gameObject);
        }
    }
}
