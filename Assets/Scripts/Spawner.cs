using UnityEngine;


// Spawns(Instantiate) pipe object when a certain distance has been met
public class Spawner : MonoBehaviour
{
    public GameObject pipeSpawned;
    public GameObject pipe;
    public bool canSpawn = true;
    public float heightOffset = 40;
    public float Gapsize;
    

    //if pipes spawned script determines if the distance is right. canSpawn is set to true. Pipe spawns. 
    void Update()
    {
        if (canSpawn)
        {
            spawnPipe();
            canSpawn = false;
        }
        if (pipeSpawned == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, pipeSpawned.transform.position) > Gapsize)
        {
            canSpawn = true;
        }
    }

    // set random y axis value and spawn pipe object
    void spawnPipe()
    {
        float lowestPoint = -heightOffset;
        float highestPoint = heightOffset;
        pipeSpawned = Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), transform.position.z), transform.rotation);
    }
}
