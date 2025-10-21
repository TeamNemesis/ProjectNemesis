using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SpawnObj : MonoBehaviour
{
    [SerializeField] private Transform[] points; // 배치할 포인트들
    [SerializeField] private GameObject[] prefabs; // 랜덤 프리팹들
    [SerializeField] private float overlapRadius = 0.5f; // 겹침 판단 반경

    private List<Transform> availablePoints = new List<Transform>();

    void Start()
    {
        Spawn();
        //SpawnPrefabs();
    }

    void Spawn()
    {
        for (int i = 0;  i < points.Length; i++)
        {
            Instantiate(prefabs[i], points[i].position, points[i].rotation);
        }
    }
    
}
