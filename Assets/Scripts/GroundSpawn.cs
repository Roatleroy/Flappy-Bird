using UnityEngine;

public class GroundSpawn : MonoBehaviour
{
    private GameObject Ground;
    private GameObject Background;

    public GameObject GroundPre;
    public GameObject BackPre;
  
    void Start()
    { 
        Ground = Instantiate(
           GroundPre,
           new Vector3(23.847f, -47.602f, transform.position.z),
           transform.rotation
       );
        Background = Instantiate(
            BackPre,
            new Vector3(43.93f, 10.31f, transform.position.z),
            transform.rotation
        );
    }

    void Update()
    {
        if (Ground != null && Ground.transform.position.x <= 23.847f)
        {
            SpawnGround();
        }
        if (Background != null && Background.transform.position.x <= 43.93f)
        {
            SpawnBackGround();
        }
    }

    void SpawnGround()
    {
        Ground = Instantiate(
           GroundPre,
           new Vector3(125.414f, -47.602f, transform.position.z),
           transform.rotation
       );
    }

    void SpawnBackGround()
    {
        Background = Instantiate(
            BackPre,
            new Vector3(187.7f, 10.309f, transform.position.z),
            transform.rotation
        );
    }
}
