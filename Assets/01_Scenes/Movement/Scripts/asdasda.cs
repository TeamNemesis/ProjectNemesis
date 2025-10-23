using UnityEngine;

public class asdasda : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject cube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        Transform objectPoint = cube.transform.Find("Point");
        Instantiate(cube,new Vector3(spawnPoint.position.x-objectPoint.position.x, spawnPoint.position.y-objectPoint.position.y, spawnPoint.position.z- objectPoint.position.z), spawnPoint.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
