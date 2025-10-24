using UnityEngine;

public class rotateTest : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject cube;

    void Start()    //생성후, 각도 변경
    {
        Transform objectPoint = cube.transform.Find("Point");
        Instantiate(cube, new Vector3(spawnPoint.position.x - objectPoint.position.x, spawnPoint.position.y - objectPoint.position.y, spawnPoint.position.z - objectPoint.position.z), 
            spawnPoint.rotation, spawnPoint);
        spawnPoint.Rotate(0, -45, 0);
    }

    
}
