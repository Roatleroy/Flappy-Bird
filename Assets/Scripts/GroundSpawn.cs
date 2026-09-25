using UnityEngine;


// Link two prefab objects and spawn initial objects. Then after the initial objects reach a certain X coordinate
// Spawn another set of ground and background objects. 
public class GroundSpawn : MonoBehaviour
{
    private GameObject Ground;
    private GameObject Background;

    public GameObject GroundPre;
    public GameObject BackPre;
  
    // Start objects at menu level X=0
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

    // When previous object spawned reaches X=0, then spawn new spawn Object
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
