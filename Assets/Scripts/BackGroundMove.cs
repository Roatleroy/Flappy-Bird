using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private float deadzone = -100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * ((PipeMove.moveSpeed / 6) * Time.deltaTime);

        if (transform.position.x < deadzone)
        {
            Debug.Log("Ground Cleared");
            Destroy(gameObject);
        }
    }
}
