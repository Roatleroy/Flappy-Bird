using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    
   // This value tells when background object is supposed to be despawned
    private float deadzone = -100f;

    //When a background object is spawned every frame move it X amount
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
