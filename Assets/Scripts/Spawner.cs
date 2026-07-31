using UnityEngine;

public class Spawner : MonoBehaviour
{
    private GameObject pipeSpawned;
    public GameObject pipe;
    private bool canSpawn = true;
    public float heightOffset = 25;
    public float Gapsize;
    

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            spawnPipe();
            canSpawn = false;
        }
        if (Vector3.Distance(transform.position, pipeSpawned.transform.position) > Gapsize)
        {
            canSpawn = true;
        }
    }

    void spawnPipe()
    {
        float lowestPoint = -heightOffset;
        float highestPoint = heightOffset;
        pipeSpawned = Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), transform.position.z), transform.rotation);
    }
}
