using System;
using UnityEngine;

public class MonsterSpanwer : MonoBehaviour
{
    public event Action<MonsterBase> OnMonsterSpawned;

    public void SpawnMonster(MonsterBase monster, Vector3 spawnPos)
    {
        // 오브젝트 풀링
        Instantiate(monster, spawnPos, Quaternion.identity);
        monster.Initialize();
        OnMonsterSpawned?.Invoke(monster);
    }

    public void SpawnBoss(MonsterBase bossMonster, Vector3 spawnPos)
    {
        // Instantiate
        Instantiate(bossMonster, spawnPos, Quaternion.identity);
        OnMonsterSpawned?.Invoke(bossMonster);
    }
}