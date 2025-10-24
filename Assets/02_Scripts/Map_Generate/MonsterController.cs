using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] MonsterSpawner _monsterSpawner;    // 몬스터 소환기

    [SerializeField] int _spawnPointsCount = 15;      // 소환 위치 개수

    /// <summary>
    /// 일반 잡몹 소환
    /// </summary>
    public void SpawnMonster(int roomCost, Transform[] monsterSpawnPoints)
    {
        List<Transform> tempList = new List<Transform>(monsterSpawnPoints);
        _monsterSpawner.InitializeAndSpawn(roomCost, tempList);
    }
}