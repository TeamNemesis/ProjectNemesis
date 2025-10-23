using System;
using UnityEngine;

public class MonsterSpanwer : MonoBehaviour
{
    public event Action<MonsterBase> OnMonsterSpawned;
    public event Action OnMonsterDied;

    public void SpawnMonster(MonsterBase monster, Vector3 spawnPos)
    {
        // 오브젝트 풀링
        Instantiate(monster, spawnPos, Quaternion.identity);
        monster.Initialize();
        // 확인용 이벤트 해제 한번
        // monster.OndDieEvent가 있는지 확인 후 해제
        monster.OnDieEvent -= RaiseMonsterDie;
        monster.OnDieEvent += RaiseMonsterDie;
        OnMonsterSpawned?.Invoke(monster);
    }

    public void SpawnBoss(MonsterBase bossMonster, Vector3 spawnPos)
    {
        // Instantiate
        Instantiate(bossMonster, spawnPos, Quaternion.identity);
        OnMonsterSpawned?.Invoke(bossMonster);
    }

    void RaiseMonsterDie()
    {
        OnMonsterDied?.Invoke();
    }
}