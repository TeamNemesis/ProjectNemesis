using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 리팩터링된 MapController
/// - 룸 생성/소멸과 문 생성 책임을 더 명확히 분리
/// - 방어적 검사 및 로그 강화
/// - 작은 헬퍼 메서드로 가독성 향상
/// </summary>
public class MapController : MonoBehaviour
{
    [SerializeField] MonsterController _monsterController;
    [SerializeField] RoomSpawner _roomSpawner;
    [SerializeField] DoorSpawner _doorSpawner;
    [SerializeField] DoorDecider _doorDecider;

    [SerializeField] Room _currentRoom;
    [SerializeField] Door[] _currentDoors;
    [SerializeField] int _currentRoomCount = -1;
    [SerializeField] bool _hasLabRoomAppeared = false;

    public MonsterController MonsterController => _monsterController;
    public RoomSpawner RoomSpawner => _roomSpawner;
    public DoorSpawner DoorSpawner => _doorSpawner;
    public DoorDecider DoorDecider => _doorDecider;

    public Room CurrentRoom => _currentRoom;
    public Door[] CurrentDoors => _currentDoors;
    public int CurrentRoomCount => _currentRoomCount;
    public bool HasLabRoomAppeared => _hasLabRoomAppeared;

    /// <summary>
    /// 룸 로딩이 끝나고 전투 시작시 발행하는 이벤트
    /// </summary>
    public event Action OnRoomStart;

    public void Initialize()
    {
        if (_roomSpawner == null) Debug.LogError("MapController.Initialize: _roomSpawner is null");
        if (_doorSpawner == null) Debug.LogError("MapController.Initialize: _doorSpawner is null");
        if (_doorDecider == null) Debug.LogError("MapController.Initialize: _doorDecider is null");

        _roomSpawner.OnRoomSpawned += OnRoomSpawned;
        _doorSpawner.DoorInteracted += OnDoorInteracted;
        _monsterController.OnAllMonsterDefeated += StartReward;

        _roomSpawner.Initialize();
        _doorDecider.Initialize();
        _monsterController.Initialize();
    }

    void OnDoorInteracted(IInteractable interactable)
    {
        if (interactable is DoorInteractor doorInteractor && doorInteractor.RoomInfo != null)
        {
            DestroyCurrentRoomObjects();
            Room room = _roomSpawner.SpawnRoom(doorInteractor.RoomInfo);
        }
        else
        {
            Debug.LogError("OnDoorInteracted: interactable is not a DoorInteractor or RoomInfo is null");
        }
    }

    void OnRoomSpawned(Room room)
    {
        if (room == null)
        {
            Debug.LogError("OnRoomSpawned called with null room");
            return;
        }

        // 상태 갱신 (작은 메서드로 분리)
        UpdateCurrentRoomState(room);

        // 문 생성 처리
        CreateDoorsForCurrentRoom();

        // 여기서 방 타입별로 스폰할 몬스터 정해주고
        // 스폰위치 업데이트 해주고 몬스터 스폰해야함.
        // 지금은 테스트용으로 노말 방에서만 생성

        int roomCost = _currentRoomCount * 10; // 예시: 방 번호에 비례하는 비용
        Transform[] spawnPoints = room.MonsterSpawnPoints;
        _monsterController.SpawnMonster(roomCost, spawnPoints);

        OnRoomStart?.Invoke();
    }

    void UpdateCurrentRoomState(Room room)
    {
        _currentRoom = room;
        _currentRoomCount++;
        Debug.Log($"[MapController] Entered room #{_currentRoomCount} type={room.RoomInfo?.RoomType.ToString() ?? "null"}");

        if (room.RoomInfo?.RoomType == RoomType.Lab)
            _hasLabRoomAppeared = true;
        if (room.RoomInfo?.RoomType == RoomType.Colosseum)
        {

        }
    }

    void CreateDoorsForCurrentRoom()
    {
        if (_currentRoom == null)
        {
            Debug.LogError("CreateDoorsForCurrentRoom: _currentRoom is null");
            return;
        }

        if (_currentRoomCount == 1)
        {
            // TechSelectPackType을 랜덤으로 선택하여 TechSelect 방 생성
            int rand = UnityEngine.Random.Range(0, (int)TechSelectPackType.Count);
            TechSelectPackType randomTechPackType = (TechSelectPackType)rand;
            Door door = TrySpawnDoor(_currentRoom.GetNextDoorPositions(1)[0], new RoomInfo(RoomType.Normal, NormalRoomType.TechSelect, randomTechPackType), _currentRoom?.transform);
            return;
        }

        // 다음 문 개수(nextDoorCount) 결정 (DoorDecider에 current index 전달)
        int nextDoorCount = _doorDecider.GetNextDoorCount(_currentRoomCount);

        if (nextDoorCount <= 0)
        {
            Debug.Log("[MapController] nextDoorCount <= 0, skipping door spawn");
            _currentDoors = Array.Empty<Door>();
            return;
        }

        // 다음 문 개수(nextDoorCount)에 따라 문 위치 배열(doorPoisitions) 얻기
        var doorPositions = _currentRoom.GetNextDoorPositions(nextDoorCount) ?? Array.Empty<Transform>();
        if (doorPositions.Length < nextDoorCount)
        {
            Debug.LogWarning($"CreateDoorsForCurrentRoom: requested {nextDoorCount} positions but got {doorPositions.Length}. Clamping.");
            nextDoorCount = doorPositions.Length;
        }

        // 문 타입 / 일반방 타입 / 기술팩 타입 결정 (분리해서 얻기)
        int normalRoomCount;
        RoomType[] doorTypes = _doorDecider.GetNextRoomTypes(
            nextDoorCount,
            _currentRoom.RoomInfo?.RoomType ?? RoomType.Normal,
            _currentRoomCount,
            _hasLabRoomAppeared,
            out normalRoomCount) ?? Array.Empty<RoomType>();

        // 안전: doorTypes 길이가 nextDoorCount보다 작으면 남는 슬롯을 마지막 선택 타입으로 채움
        if (doorTypes.Length != nextDoorCount)
        {
            Debug.LogWarning($"GetNextRoomTypes returned {doorTypes.Length} items but expected {nextDoorCount}. Filling remaining slots.");
            doorTypes = FillToCount(doorTypes, nextDoorCount);
        }

        NormalRoomType[] normalRoomTypes = Array.Empty<NormalRoomType>();
        int techSelectPackCount = 0;
        if (normalRoomCount > 0)
            normalRoomTypes = _doorDecider.GetNormalRoomTypes(normalRoomCount, out techSelectPackCount) ?? Array.Empty<NormalRoomType>();

        TechSelectPackType[] techPackTypes = Array.Empty<TechSelectPackType>();
        if (techSelectPackCount > 0)
            techPackTypes = GameManager.Instance?.skillManager?.GetSkillPackTypes(techSelectPackCount) ?? Array.Empty<TechSelectPackType>();

        // 소비 인덱스
        int normalIdx = 0;
        int techIdx = 0;

        List<Door> created = new List<Door>(nextDoorCount);

        for (int i = 0; i < nextDoorCount; i++)
        {
            RoomType rt = doorTypes.Length > i ? doorTypes[i] : RoomType.Normal;

            NormalRoomType? nrt = null;
            TechSelectPackType? tpt = null;

            if (rt == RoomType.Normal)
            {
                if (normalIdx < normalRoomTypes.Length)
                {
                    nrt = normalRoomTypes[normalIdx++];
                    if (nrt == NormalRoomType.TechSelect)
                    {
                        if (techIdx < techPackTypes.Length)
                            tpt = techPackTypes[techIdx++];
                        else
                            Debug.LogWarning("CreateDoorsForCurrentRoom: techPackTypes 부족");
                    }
                }
                else
                {
                    Debug.LogWarning("CreateDoorsForCurrentRoom: normalRoomTypes 부족, defaulting to Normal");
                }
            }

            var nextRoomInfo = new RoomInfo(rt, nrt, tpt);

            // DoorSpawner가 parent 파라미터를 지원하면 전달하고, 아니면 SetParent로 처리
            Door door = TrySpawnDoor(doorPositions[i], nextRoomInfo, _currentRoom?.transform);
            if (door != null)
            {
                created.Add(door);
                Debug.Log($"[MapController] Spawned door #{i} type={nextRoomInfo.RoomType} at {door.transform.position}");
            }
            else
            {
                Debug.LogWarning($"[MapController] Failed to spawn door at index {i}");
            }
        }

        _currentDoors = created.ToArray();

        // Lab 포함 여부 갱신(선택지에 lab 포함돼있으면 표시)
        if (Array.Exists(doorTypes, t => t == RoomType.Lab))
            _hasLabRoomAppeared = true;
    }

    // DoorSpawner가 parent 파라미터를 지원하는 경우를 고려해 안전하게 호출
    Door TrySpawnDoor(Transform position, RoomInfo info, Transform parent)
    {
        if (_doorSpawner == null)
        {
            Debug.LogError("TrySpawnDoor: _doorSpawner is null");
            return null;
        }

        // 기본 SpawnDoor(Transform, RoomInfo)
        Door door = _doorSpawner.SpawnDoor(position, info);
        _currentRoom.OnRewardSelectionFinished += door.OnRewardSelectionCompleted;
        Debug.Log("OnRewardSelectionFinished event subscribed to door.OnRewardSelectionCompleted");
        if (_currentRoom.RoomInfo.RoomType == RoomType.Start || _currentRoom.RoomInfo.RoomType == RoomType.Shop)
        {
            door.OnRewardSelectionCompleted();
        }
        if (door != null && parent != null)
            door.transform.SetParent(parent, worldPositionStays: true);

        return door;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"TrySpawnDoor exception: {ex}");
        //    return null;
        //}
    }

    // doorTypes 배열을 요청 개수(count)만큼 채움: 마지막 값 또는 Normal로 채움
    RoomType[] FillToCount(RoomType[] original, int count)
    {
        if (original == null) return Array.Empty<RoomType>();
        if (original.Length >= count) return original;

        var result = new RoomType[count];
        for (int i = 0; i < original.Length; i++) result[i] = original[i];

        RoomType fill = original.Length > 0 ? original[original.Length - 1] : RoomType.Normal;
        for (int i = original.Length; i < count; i++) result[i] = fill;
        return result;
    }

    void DestroyCurrentRoomObjects()
    {

        // PoolableObject 찾아서 반환 처리
        var poolables = _currentRoom.PoolableObjectsInRoom;
        foreach (var poolableObj in poolables)
        {
            GameManager.Instance.PoolManager.ReleaseToPool(poolableObj);
        }

        // 먼저 문들 파괴
        if (_currentDoors != null)
        {
            foreach (var door in _currentDoors)
            {
                if (door == null) continue;
                if (door.gameObject.scene.IsValid())
                {
                    Debug.Log($"[MapController] Destroying door instance: {door.name}");
                    Destroy(door.gameObject);
                }
                else
                {
                    Debug.LogWarning($"[MapController] Door appears to be an asset: {door.name}, skipping Destroy");
                }
            }
            _currentDoors = null;
        }

        // 그 다음 방 파괴
        if (_currentRoom != null)
        {
            if (_currentRoom.gameObject.scene.IsValid())
            {
                Debug.Log($"[MapController] Destroying room instance: {_currentRoom.name}");
                Destroy(_currentRoom.gameObject);
            }
            else
            {
                Debug.LogWarning($"[MapController] CurrentRoom appears to be an asset: {_currentRoom.name}, skipping Destroy");
            }
            _currentRoom = null;
        }
    }

    void StartReward()
    {
        _currentRoom.SpawnReward();
    }

#if UNITY_EDITOR
    public void KillAllMonsters()
    {
        _monsterController.MonsterSpawner?.KillAllActiveMonsters();
    }

#endif

}