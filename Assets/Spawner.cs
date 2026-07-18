using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject pipe;
    public float spawnRate = 4f;
    public float minimumSpawnRate = 1f;
    public float spawnRateDecrease = 0.1f;

    private float timer = 0f;
    public float heightOffset = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnRate -= 1f;
        Instantiate(pipe, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {                                                                                             
        if (timer < spawnRate) {
            timer += Time.deltaTime;
        }
        else
        {
            spawnPipe();
            timer = 0;
        }
        
    }

    void spawnPipe()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;

        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), transform.position.z), transform.rotation);

        spawnRate = Mathf.Max(
            minimumSpawnRate,
            spawnRate - spawnRateDecrease
        );

    }
}
