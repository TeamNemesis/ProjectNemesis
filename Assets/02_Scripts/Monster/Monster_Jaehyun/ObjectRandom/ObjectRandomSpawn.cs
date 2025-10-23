using UnityEngine;

public class ObjectRandomSpawn : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] objects;
    void Start()
    {
        SpawnObjects2();
    }

    private void SpawnObjects()
    {

        foreach (Transform point in spawnPoints)
        {
            int rand = Random.Range(0, objects.Length);
            Instantiate(objects[rand], point.position, point.rotation);
        }
    }
    private void SpawnObjects2()
    {
        int[] array = new int[objects.Length];
        for (int i = 0; i < array.Length; i++)
            array[i] = i;

        
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Instantiate(objects[array[i]], spawnPoints[i].position, spawnPoints[i].rotation);
            //Transform spawnPoint = spawnPoints[i];
            //GameObject prefab = objects[array[i]];

            //// 1️⃣ 프리팹 임시 생성
            //GameObject instance = Instantiate(prefab);

            //// 2️⃣ 프리팹 안에서 "ObjectPoint"라는 이름의 자식 Transform 찾기
            //Transform objectPoint = instance.transform.Find("Point");


            //// 3️⃣ ObjectPoint를 spawnPoint에 정렬시키기
            //Vector3 offset = instance.transform.position - objectPoint.position;

            //instance.transform.position = spawnPoint.position + offset;
            //instance.transform.rotation = spawnPoint.rotation;
        }

        
    }


}
