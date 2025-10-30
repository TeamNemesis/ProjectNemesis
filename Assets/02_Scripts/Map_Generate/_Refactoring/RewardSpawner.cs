using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RewardSpawner: DataManager의 매핑을 사용하여 보상(RewardType)을 실제 인스턴스로 생성/풀에서 가져오는 구현.
/// - RewardDecider는 RewardType(또는 타입+메타)을 반환하도록 구성되어 있다고 가정.
/// - RewardDataSO는 프리팹(GameObject Prefab 또는 PrefabKey)과 RequiresSelection 같은 메타를 갖고 있어야 함.
/// </summary>
public class RewardSpawner : MonoBehaviour
{
    // 기존 RewardDecider 타입을 그대로 사용(DecideForRoom은 RewardType[] 또는 구조체를 반환하도록 조정 필요)
    [SerializeField] RewardDecider _decider; // 에디터에서 주입하거나 GameManager에서 가져오세요

    DataManager _dataManager => GameManager.Instance?.DataManager;
    PoolManager _poolMgr => GameManager.Instance?.PoolManager;
    ResourceManager _resMgr => GameManager.Instance?.ResourceManager;

    void Reset()
    {
        // 편의: 에디터에서 자동으로 연결(있다면)
        if (_decider == null) _decider = GetComponent<RewardDecider>();
    }

    /// <summary>
    /// 룸 기준으로 보상 결정을 요청하고 실제 인스턴스화하여 반환.
    /// - Decider는 보상 "타입/메타" 배열을 반환해야 함 (예: RewardType[] 또는 (RewardType, TechPack) 튜플 등)
    /// - 여기서는 간단히 RewardType[]을 반환한다고 가정
    /// </summary>
    public IInteractable[] SpawnForRoom(IRoom room, int desiredCount)
    {
        if (room == null) return Array.Empty<IInteractable>();

        // 1) Decider로부터 보상 타입/메타 결정
        RewardType[] decidedTypes = null;
        TechSelectPackType[] decidedTechPacks = null;

        try
        {
            if (_decider != null)
            {
                // NOTE: RewardDecider.DecideForRoom를 실제로는 RewardType[] 반환 시그니처로 맞추어야 합니다.
                // 예: (RewardType[] rewardTypes, TechSelectPackType[] techPacks) = _decider.DecideForRoomTypes(room.RoomInfo, desiredCount);
                var tuple = _decider.DecideForRoomAsTypes(room.RoomInfo, desiredCount);
                decidedTypes = tuple.types;
                decidedTechPacks = tuple.techPacks;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"RewardSpawner: Decider.DecideForRoomAsTypes threw: {ex}");
        }

        if (decidedTypes == null || decidedTypes.Length == 0)
            return Array.Empty<IInteractable>();

        // 2) 실제 생성: mapping된 RewardDataSO를 통해 인스턴스화/풀에서 획득
        var spawned = new List<IInteractable>();
        var concreteRoom = room as Room; // Room에 풀 오브젝트 등록 API가 있다고 가정

        for (int i = 0; i < decidedTypes.Length; i++)
        {
            var rtype = decidedTypes[i];
            RewardDataSO rdata = null;
            if (_dataManager != null)
            {
                if (!_dataManager.TryGetRewardData(rtype, out rdata))
                {
                    Debug.LogWarning($"RewardSpawner: DataManager has no RewardData for {rtype}");
                    continue;
                }
            }

            GameObject instance = null;

            // Prefer PoolManager
            if (_poolMgr != null && rdata != null)
            {
                string poolKey = rdata.PrefabKey ?? rdata.name; // RewardDataSO는 PrefabKey 또는 Prefab reference 를 갖고 있어야 함
                try
                {
                    instance = _poolMgr.GetFromPool(poolKey, GetRewardSpawnPosition(concreteRoom), Quaternion.identity, concreteRoom?.transform);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"RewardSpawner: PoolManager.GetFromPool threw for key '{rdata?.PrefabKey}': {ex}");
                    instance = null;
                }
            }

            // Fallback: ResourceManager 또는 direct Prefab Instantiate
            if (instance == null && rdata != null)
            {
                if (rdata.Prefab != null)
                {
                    instance = Instantiate(rdata.Prefab, GetRewardSpawnPosition(concreteRoom), Quaternion.identity, concreteRoom?.transform);
                }
                else if (!string.IsNullOrEmpty(rdata.PrefabKey) && _resMgr != null)
                {
                    var prefab = _resMgr.LoadResource<GameObject>(rdata.PrefabKey);
                    if (prefab != null)
                        instance = Instantiate(prefab, GetRewardSpawnPosition(concreteRoom), Quaternion.identity, concreteRoom?.transform);
                }
            }

            if (instance == null)
            {
                Debug.LogWarning($"RewardSpawner: Could not spawn reward for type {rtype}");
                continue;
            }

            // Register poolable to room so StageController can release later
            if (concreteRoom != null)
            {
                try { concreteRoom.GetPoolableObjectsInRoom().Add(instance); } catch { }
            }

            // find IInteractable and RewardInteractableObject
            var interactable = instance.GetComponent<IInteractable>() ?? instance.GetComponentInChildren<IInteractable>(true);
            var rewardObj = instance.GetComponent<RewardInteractableObject>() ?? instance.GetComponentInChildren<RewardInteractableObject>(true);

            if (rewardObj != null)
            {
                // If RewardDataSO contains metadata like RequiresSelection, assign it
                rewardObj.SpecRequiresSelection = rdata?.RequiresSelection ?? false;

                // subscribe to completion
                Action onGiven = null;
                onGiven = () =>
                {
                    try { rewardObj.OnRewardGiven -= onGiven; } catch { }
                    try
                    {
                        // room is responsible for handling selection finished (StageController listens)
                        room.OnRewardsRequested?.Invoke(room); // optional hook
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"RewardSpawner: room reward callback threw: {ex}");
                    }
                };
                rewardObj.OnRewardGiven += onGiven;
            }

            if (interactable != null)
                spawned.Add(interactable);
            else
                Debug.LogWarning($"RewardSpawner: Spawned object '{instance.name}' does not implement IInteractable.");
        }

        return spawned.ToArray();
    }

    // helper: find spawn position for rewards in the room
    Vector3 GetRewardSpawnPosition(Room room)
    {
        if (room == null) return Vector3.zero;
        if (room.RewardSpawnPoints != null && room.RewardSpawnPoints.Length > 0)
            return room.RewardSpawnPoints[0].position;
        return room.transform.position;
    }
}