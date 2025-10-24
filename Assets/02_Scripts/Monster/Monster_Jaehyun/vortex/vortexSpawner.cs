using UnityEngine;

public class vortexSpawner : MonoBehaviour
{
    [SerializeField] private GameObject vortexPrefab;
    void Start()
    {
        SpawnVortex(5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void SpawnVortex(int cool)  // 소용돌이 생성(지속시간)
    {
        GameObject prefab= Instantiate(vortexPrefab, new Vector3(0,0,0), Quaternion.identity);
        Destroy(prefab, cool);
    }
}
