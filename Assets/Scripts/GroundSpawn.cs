using UnityEngine;

public class GroundSpawn : MonoBehaviour
{
    private GameObject GroundSpawned;
    public GameObject Cloud;

    void Start()
    {
        GroundSpawned = Instantiate(
            Cloud,
            new Vector3(0f, -38f, transform.position.z),
            transform.rotation
        );
    }

    void Update()
    {
        if (GroundSpawned != null && GroundSpawned.transform.position.x <= 0f)
        {
            SpawnGround();
        }
    }

    void SpawnGround()
    {
        GroundSpawned = Instantiate(
            Cloud,
            new Vector3(106f, -38f, transform.position.z),
            transform.rotation
        );
    }
}
