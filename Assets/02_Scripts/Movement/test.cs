using Unity.VisualScripting;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class test : MonoBehaviour
{
    public Transform point;
    public GameObject prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(prefab, new Vector3(point.position.x, prefab.transform.position.y, point.position.z), point.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }



}
