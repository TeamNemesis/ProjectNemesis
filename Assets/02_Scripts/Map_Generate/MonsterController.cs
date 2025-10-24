using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    //[SerializeField] MonsterSpanwer _monsterSpawner;    // 몬스터 소환기

    [SerializeField] int _roomCost;     // 이번 방에 소환 가능한 몬스터 코스트
    [SerializeField] int _waveCost;     // 이번 웨이브에 소환 가능한 몬스터 코스트
    [SerializeField] Transform[] _spawnPoints;    // 몬스터 소환 위치들
    [SerializeField] Transform _bossSpawnPoint; // 보스 소환 위치

    List<MonsterBase> _allNormalMonsters;      // 소환 가능한 모든 몬스터들 (A B C D E)
    List<MonsterBase> _allBossMonster;  // 
    [SerializeField] List<MonsterBase> _monsterToSpawn;   // 이번 웨이브에 소환해야할 몬스터(AAABC), (ABBCD)
    // 

    //public void Initialize(List<MonsterBase> allMonsterList)
    //{
    //    // 소환가능한 모든 몬스터 리스트 받아오기
    //    _allNormalMonsters = allMonsterList;
    //}

    /// <summary>
    /// 보스 소환
    /// </summary>
    public void SpawnBoss()
    {
        // 보스 몬스터 소환
        //_monsterSpawner.SpawnBoss( ,_bossSpawnPoint);
        // 연출 추가
    }

    public void UpdateMonsterSpawnPoints(Transform[] newSpawnPoints)
    {
        if(newSpawnPoints == null || newSpawnPoints.Length == 0)
        {
            Debug.LogWarning("New spawn points are null or empty.");
            return;
        }
        _spawnPoints = newSpawnPoints;
    }

    /// <summary>
    /// 일반 잡몹 소환
    /// </summary>
    public void SpawnMonster()
    {
        // 이번 웨이브에 소환할 몬스터 결정
        //_monsterToSpawn = DecideMonstersToSpawn();

        //// 소환해야할 위치가 모자라면 경고 출력
        //if (_monsterToSpawn.Count > _spawnPoints.Length)
        //{
        //    Debug.LogWarning("Not enough spawn points for the monsters to spawn!");
        //    return;
        //}

        // _spawnPoints 중 _monsterToSpawn.Count 개 만큼 랜덤으로 골라서 몬스터 소환
        List<int> usedIndices = new List<int>();
        if(_monsterToSpawn == null || _monsterToSpawn.Count == 0)
        {
            Debug.LogWarning("No monsters to spawn for this wave.");
            return;
        }
        for (int i = 0; i < _monsterToSpawn.Count; i++)
        {
            int randIndex;
            do
            {
              randIndex = Random.Range(0, _spawnPoints.Length);
            } while (usedIndices.Contains(randIndex));
            usedIndices.Add(randIndex);
            //_monsterSpawner.SpawnMonster(_monsterToSpawn[i], _spawnPoints[randIndex].position);
        }
    }

    /// <summary>
    /// 이번 웨이브에 소환할 몬스터들을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    List<MonsterBase> DecideMonstersToSpawn()
    {
        List<MonsterBase> monsters = new List<MonsterBase>();
        int remainingCost = _waveCost;
        // 남은 코스트가 0보다 클 때까지 반복
        while (remainingCost > 0)
        {
            // 소환 가능한 몬스터 리스트에서 랜덤으로 하나 선택
            MonsterBase candidate = GetRandomMonsterWithinCost(remainingCost);
            if (candidate == null)
                break; // 더 이상 소환할 수 있는 몬스터가 없으면 종료
            monsters.Add(candidate);
            //remainingCost -= candidate.Cost; // 선택한 몬스터의 코스트만큼 차감
        }
        return monsters;
    }

    /// <summary>
    /// 남은 코스트보다 작거나 같은 코스트를 가진 몬스터들 중 랜덤으로 하나 반환하는 함수
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    MonsterBase GetRandomMonsterWithinCost(int cost)
    {
        // 소환 가능한 몬스터 리스트에서 코스트 이하인 몬스터들만 필터링
        List<MonsterBase> affordableMonsters = new List<MonsterBase>();
        foreach (var monster in _allNormalMonsters)
        {
            //if (monster.Cost <= cost)
            //{
            //    affordableMonsters.Add(monster);
            //}
        }
        if (affordableMonsters.Count == 0)
            return null; // 소환할 수 있는 몬스터가 없으면 null 반환
        // 필터링된 몬스터들 중에서 랜덤으로 하나 선택하여 반환
        int randIndex = Random.Range(0, affordableMonsters.Count);
        return affordableMonsters[randIndex];
    }
}