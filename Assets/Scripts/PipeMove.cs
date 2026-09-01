using UnityEngine;

public class PipeMove : MonoBehaviour
{
    public static float moveSpeed = 14.5f;
    public float speedIncrease = 0.02f;
    public float deadzone = -65f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed += speedIncrease;
    }

    // Update is called once per frame
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
