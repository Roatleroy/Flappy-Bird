using UnityEngine;

public class Spawner : MonoBehaviour
{
    private GameObject pipeSpawned;
    public GameObject pipe;
    private bool canSpawn;
    public float heightOffset = 10;
    public float Gapsize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pipeSpawned = Instantiate(pipe, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, pipeSpawned.transform.position) > Gapsize)
        {
            canSpawn = true;
        }
        if (canSpawn)
        {
            spawnPipe();
            canSpawn = false;
        }
    }

    void spawnPipe()
    {
        float lowestPoint = -heightOffset;
        float highestPoint = heightOffset;
        pipeSpawned = Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), transform.position.z), transform.rotation);
        Gapsize -= 0.1f;
    }
}
