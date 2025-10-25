using UnityEngine;

/// <summary>
/// NormalRoom: 보상 생성 로직을 단순화하고 안전성 검사를 추가한 버전
/// </summary>
public class NormalRoom : Room
{
    [SerializeField] Transform[] _monsterSpawnPoints;

    public Transform[] MonsterSpawnPoints => _monsterSpawnPoints;

    public override RewardInteractableObject[] SpawnReward()
    {
        // 안전 검사: 스폰 포인트가 없으면 빈 배열 반환
        if (_rewardSpawnPoints == null || _rewardSpawnPoints.Length == 0)
        {
            Debug.LogWarning("NormalRoom.SpawnReward: reward spawn points are not set.");
            return System.Array.Empty<RewardInteractableObject>();
        }

        // DataManager 캐시
        var dataManager = GameManager.Instance?.DataManager;
        if (dataManager == null)
        {
            Debug.LogError("NormalRoom.SpawnReward: DataManager is null.");
            return System.Array.Empty<RewardInteractableObject>();
        }

        var prefab = GetPrefabForRoomType(dataManager);
        Debug.Log($"Spawning prefab: {(prefab != null ? prefab.name : "null")} for type {_roomInfo.NormalType}");

        var instantiated = Instantiate(prefab, _rewardSpawnPoints[0].position, Quaternion.identity, transform);
        Debug.Log($"Instantiated root name: {instantiated.name}");

        // 시도 1: 루트에서 찾기
        var rewardRoot = instantiated.GetComponent<RewardInteractableObject>();
        Debug.Log($"GetComponent<RewardInteractableObject>() => {(rewardRoot == null ? "null" : rewardRoot.GetType().FullName)}");

        // 시도 2: 자식까지 검색
        var rewardChild = instantiated.GetComponentInChildren<RewardInteractableObject>(true);
        Debug.Log($"GetComponentInChildren<RewardInteractableObject>(true) => {(rewardChild == null ? "null" : rewardChild.GetType().FullName)}");

        // 전체 MonoBehaviour 목록 출력(무엇이 붙어있는지 확인)
        var monos = instantiated.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var m in monos)
            Debug.Log($"Component on instantiated: {m.GetType().FullName}");

        //GameObject prefab = GetPrefabForRoomType(dataManager);
        //if (prefab == null)
        //{
        //    Debug.LogWarning($"NormalRoom.SpawnReward: No prefab mapped for NormalType {_roomInfo.NormalType}.");
        //    return System.Array.Empty<RewardInteractableObject>();
        //}

        //var instantiated = Instantiate(prefab, _rewardSpawnPoints[0].position, Quaternion.identity, transform);
        var reward = instantiated.GetComponent<RewardInteractableObject>();
        if (reward == null)
        {
            Debug.LogError("NormalRoom.SpawnReward: Prefab does not contain RewardInteractableObject component.");
            Destroy(instantiated);
            return System.Array.Empty<RewardInteractableObject>();
        }
        reward.OnRewardGiven += RewardSelectionFinished;

        // Tech 초기화가 필요한 경우 인터페이스로 안전하게 초기화
        if (reward is TechSelectPackInteractor techSelectPackInteractor &&
            _roomInfo.TechType.HasValue)
        {
            techSelectPackInteractor.Initialize(_roomInfo.TechType.Value);
        }

        return new RewardInteractableObject[] { reward };
    }

    // NormalRoomType -> prefab 선택을 중앙화
    GameObject GetPrefabForRoomType(DataManager dataManager)
    {
        switch (_roomInfo.NormalType)
        {
            case NormalRoomType.TechSelect:
                if (_roomInfo.TechType.HasValue)
                    return dataManager.TechPackDataMap[_roomInfo.TechType.Value].RewardPrefab;
                return null;
            case NormalRoomType.Credit:
                return dataManager.RewardDataMap[RewardType.Credit].RewardPrefab;
            case NormalRoomType.Heal:
                return dataManager.RewardDataMap[RewardType.HealPack].RewardPrefab;
            case NormalRoomType.Chrome:
                return dataManager.RewardDataMap[RewardType.Chrome].RewardPrefab;
            case NormalRoomType.TechUpgrade:
                return dataManager.RewardDataMap[RewardType.TechUpgradePack].RewardPrefab;
            default:
                Debug.LogWarning("NormalRoom.GetPrefabForRoomType: Invalid NormalType.");
                return null;
        }
    }
}

//using UnityEngine;

//public class NormalRoom : Room
//{
//    [SerializeField] Transform[] _monsterSpawnPoints;

//    public Transform[] MonsterSpawnPoints => _monsterSpawnPoints;
//    public override RewardInteractableObject[] SpawnReward()
//    {
//        RewardInteractableObject[] rewardInteractableObjects = new RewardInteractableObject[1];
//        switch (_roomInfo.NormalType)
//        {
//            case NormalRoomType.TechSelect:
//                {
//                    GameObject obj = GameManager.Instance.DataManager.TechPackDataMap[_roomInfo.TechType.Value].RewardPrefab;
//                    RewardInteractableObject reward = Instantiate(obj, _rewardSpawnPoints[0].position, Quaternion.identity, transform).GetComponent<RewardInteractableObject>();
//                    rewardInteractableObjects[0] = reward;
//                    TechSelectPackInteractor techSelectPackInteractor = reward as TechSelectPackInteractor;
//                    techSelectPackInteractor.Initialize(_roomInfo.TechType.Value);
//                    return rewardInteractableObjects;
//                }
//            case NormalRoomType.Credit:
//                {
//                    GameObject obj = GameManager.Instance.DataManager.RewardDataMap[RewardType.Credit].RewardPrefab;
//                    RewardInteractableObject reward = Instantiate(obj, _rewardSpawnPoints[0].position, Quaternion.identity, transform).GetComponent<RewardInteractableObject>();
//                    rewardInteractableObjects[0] = reward;
//                    TechSelectPackInteractor techSelectPackInteractor = reward as TechSelectPackInteractor;
//                    techSelectPackInteractor.Initialize(_roomInfo.TechType.Value);
//                    return rewardInteractableObjects;
//                }
//            case NormalRoomType.Heal:
//                {
//                    GameObject obj = GameManager.Instance.DataManager.RewardDataMap[RewardType.HealPack].RewardPrefab;
//                    RewardInteractableObject reward = Instantiate(obj, _rewardSpawnPoints[0].position, Quaternion.identity, transform).GetComponent<RewardInteractableObject>();
//                    rewardInteractableObjects[0] = reward;
//                    TechSelectPackInteractor techSelectPackInteractor = reward as TechSelectPackInteractor;
//                    techSelectPackInteractor.Initialize(_roomInfo.TechType.Value);
//                    return rewardInteractableObjects;
//                }
//            case NormalRoomType.Chrome:
//                {
//                    GameObject obj = GameManager.Instance.DataManager.RewardDataMap[RewardType.Chrome].RewardPrefab;
//                    RewardInteractableObject reward = Instantiate(obj, _rewardSpawnPoints[0].position, Quaternion.identity, transform).GetComponent<RewardInteractableObject>();
//                    rewardInteractableObjects[0] = reward;
//                    TechSelectPackInteractor techSelectPackInteractor = reward as TechSelectPackInteractor;
//                    techSelectPackInteractor.Initialize(_roomInfo.TechType.Value);
//                    return rewardInteractableObjects;
//                }
//            case NormalRoomType.TechUpgrade:
//                {
//                    GameObject obj = GameManager.Instance.DataManager.RewardDataMap[RewardType.TechUpgradePack].RewardPrefab;
//                    RewardInteractableObject reward = Instantiate(obj, _rewardSpawnPoints[0].position, Quaternion.identity, transform).GetComponent<RewardInteractableObject>();
//                    rewardInteractableObjects[0] = reward;
//                    TechSelectPackInteractor techSelectPackInteractor = reward as TechSelectPackInteractor;
//                    techSelectPackInteractor.Initialize(_roomInfo.TechType.Value);
//                    return rewardInteractableObjects;
//                }
//            default:
//                {
//                    Debug.Log("NormalRoom SpawnReward: NormalType이 유효하지 않습니다.");
//                    return null;
//                }
//        }
//    }
//}