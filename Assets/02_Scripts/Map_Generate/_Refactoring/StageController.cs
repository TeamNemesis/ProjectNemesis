using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// StageController
/// - 책임: 룸 전환 흐름(언로드 → 스폰 → 입장 후 몬스터 스폰 → 보상 처리 요청 -> 문 스폰) 오케스트레이션
/// - 비즈니스 로직(몬스터/문/풀/보상 세부 처리)은 각 스포너(Spawner)에게 위임합니다.
/// 
/// 사용 전제(인터페이스/이벤트 계약)
/// - IRoom: RoomInfo, event Action<IRoom> OnEntered, void Enter(), void Exit(),
///          List<GameObject> PoolableObjectsInRoom { get; }
/// - RoomSpawner: IRoom SpawnRoom(RoomInfo info)
/// - MonsterSpawner: void SpawnForRoom(IRoom room), event Action OnAllMonsterDefeated
/// - RewardSpawner: IInteractable[] SpawnRewards() (Room이 호출)
/// - DoorSpawner: event Action<RoomInfo> DoorInteracted (혹은 적절한 시그니처)
/// - PoolManager 등은 Room / spawned objects가 자체적으로 관리하거나 StageController가 Release 처리
/// 
/// 리팩터링 포인트:
/// - StageController는 "어떻게"가 아니라 "언제"만 알고, 실제 작업은 각 컴포넌트에 위임합니다.
/// - 모든 외부 참조(null) 체크와 이벤트 구독·해제에 신경썼습니다.
/// </summary>
public class StageController : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] RoomSpawner _roomSpawner;
    [SerializeField] MonsterSpawner _monsterSpawner;
    [SerializeField] RewardSpawner _rewardSpawner;
    [SerializeField] DoorSpawner _doorSpawner;

    [Header("Start room options")]
    [SerializeField] TechSelectPackType _startTechPackType;       // 시작 룸에서 다음 방으로 갈 때 줄 TechPack 타입 (임시/디폴트)

    [Header("----- 런타임 정보 -----")]
    [SerializeField] int _currentRoomIndex = 0;

    // 현재 활성화된 룸
    IRoom _currentRoom;
    List<Door> _currentDoors = new List<Door>();

    public RoomSpawner RoomSpawner => _roomSpawner;
    public MonsterSpawner MonsterSpawner => _monsterSpawner;
    public DoorSpawner DoorSpawner => _doorSpawner;
    public IRoom CurrentRoom => _currentRoom;
    public int CurrentRoomIndex => _currentRoomIndex;

    private void Awake()
    {
        if (_roomSpawner == null) _roomSpawner = GetComponent<RoomSpawner>();
        if (_monsterSpawner == null) _monsterSpawner = GetComponent<MonsterSpawner>();
        if (_rewardSpawner == null) _rewardSpawner = GetComponent<RewardSpawner>();
        if (_doorSpawner == null) _doorSpawner = GetComponent<DoorSpawner>();
    }

    public void Initialize()
    {
        if (_roomSpawner == null) Debug.LogError("StageController.Initialize: RoomSpawner is null");
        if (_monsterSpawner == null) Debug.LogError("StageController.Initialize: MonsterSpawner is null");
        if (_rewardSpawner == null) Debug.LogError("StageController.Initialize: RewardSpawner is null");
        if (_doorSpawner == null) Debug.LogError("StageController.Initialize: DoorSpawner is null");

        // 시작 룸 스폰 (Stage 시작 시 바로 호출)
        OnDoorInteracted(new RoomInfo(RoomType.Start, NormalRoomType.TechSelect, _startTechPackType));
    }

    /// <summary>
    /// 문 상호작용으로 다음 RoomInfo가 전달됐을 때 처리.
    /// 수정: 초기 방이 없을 때 바로 spawn 허용 (기존 early-return 버그 수정).
    /// </summary>
    void OnDoorInteracted(RoomInfo nextRoomInfo)
    {
        if (nextRoomInfo == null)
        {
            Debug.LogError("StageController.OnDoorInteracted: nextRoomInfo is null");
            return;
        }

        // 이전 룸이 있으면 언로드(없으면 첫 방)
        if (_currentRoom != null)
        {
            UnloadRoom(_currentRoom);
        }

        if (_roomSpawner == null)
        {
            Debug.LogError("StageController.OnDoorInteracted: RoomSpawner is null, cannot spawn room");
            return;
        }

        IRoom newRoom = null;
        try { newRoom = _roomSpawner.SpawnRoom(nextRoomInfo); }
        catch (Exception ex) { Debug.LogError($"StageController.OnDoorInteracted: SpawnRoom threw exception: {ex}"); return; }

        if (newRoom == null) { Debug.LogError("StageController.OnDoorInteracted: SpawnRoom returned null"); return; }

        _currentRoom = newRoom;
        _currentRoomIndex++;
        _currentRoom.OnEntered += HandleRoomEntered;
        _currentRoom.Enter();
        if(_currentRoom is IRewardRoom rewardRoom)
        {
            rewardRoom.OnRewardsRequested += (r) =>
            {
                SpawnRewardsForRoom(r);
            };
        }
    }

    /// <summary>
    /// 룸 입장 시 처리
    /// - 문 생성(프리뷰 표시) -> 몬스터 스폰(있다면) -> 몬스터 종료 시 보상 스폰 -> 보상 완료 시 문 언락
    /// </summary>
    void HandleRoomEntered(IRoom room)
    {
        if (room == null) return;

        // 한 번만 처리하도록 구독 해제
        room.OnEntered -= HandleRoomEntered;

        // 1) 문 생성 및 초기 설정 (프리뷰 제공)
        try
        {
            _doorSpawner.SpawnDoorsForCurrentRoom(room);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"StageController.HandleRoomEntered: SpawnDoorsForCurrentRoom threw: {ex}");
        }

        // 2) 몬스터 스폰 흐름
        if (_monsterSpawner == null)
        {
            Debug.LogError("StageController.HandleRoomEntered: MonsterSpawner is null");
            return;
        }

        if (room is IMonsterRoom monsterRoom)
        {
            // 몬스터 스폰
            SpawnMonster(monsterRoom);
        }
        else
        {
            // 몬스터 없는 방: 바로 보상 스폰 처리
            SpawnRewardsForRoom(room);
        }
    }

    /// <summary>
    /// 몬스터를 소환하는 방인지 확인하고 방의 타입에 따라 몬스터를 소환합니다.
    /// </summary>
    /// <param name="room"></param>
    void SpawnMonster(IMonsterRoom monsterRoom)
    {
        switch (monsterRoom.RoomInfo.RoomType)
        {
            case RoomType.Normal:
                _monsterSpawner.InitializeAndSpawn(_currentRoomIndex * 10, monsterRoom.MonsterSpawnPoints);
                break;
            case RoomType.Colosseum:
                Debug.Log("콜로세움 몬스터 스폰 로직 필요");
                // TODO : 콜로세움 몬스터 스폰 로직 구현
                break;
            case RoomType.Boss:
                Debug.Log("보스 몬스터 스폰 로직 필요");
                // TODO : 보스 몬스터 스폰 로직 구현
                break;
            default:
                Debug.Log("이 방 타입은 몬스터 스폰이 필요 없습니다.");
                break;
        }
    }
    
    void SpawnRewardsForRoom(IRoom room)
    {
        if (room == null)
        {
            Debug.LogWarning("room이 null입니다. 보상을 스폰할 수 없습니다.");
            return;
        }

        IInteractable[] rewards = null;
        if(room is IRewardRoom rewardRoom)
        {
            // 보상 요청 이벤트 구독
            Action<IRoom> onRewardsRequestedHandler = null;
            onRewardsRequestedHandler = (r) =>
            {
                try
                {
                    rewardRoom.OnRewardsRequested -= onRewardsRequestedHandler;
                }
                catch { }
                try
                {
                    rewards = _rewardSpawner.SpawnForRoom(room, rewardRoom.RewardSpawnPoints.Length);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"StageController.SpawnRewardsForRoom: RewardSpawner.SpawnForRoom threw: {ex}");
                }
                if (rewards == null || rewards.Length == 0)
                {
                    Debug.LogWarning("StageController.SpawnRewardsForRoom: No rewards spawned for room");
                }
            };
            rewardRoom.OnRewardsRequested += onRewardsRequestedHandler;
            return;

        }
        try
        {
            rewards = _rewardSpawner.SpawnForRoom(room, 1);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"StageController.SpawnRewardsForRoom: RewardSpawner.SpawnForRoom threw: {ex}");
        }
        if (rewards == null || rewards.Length == 0)
        {
            Debug.LogWarning("StageController.SpawnRewardsForRoom: No rewards spawned for room");
        }

    }

    /// <summary>
    /// 방에 있는 동안 생성된 모든 풀 오브젝트를 반환하고 방을 파괴합니다.
    /// </summary>
    /// <param name="room"></param>
    void UnloadRoom(IRoom room)
    {
        if (room == null) return;

        try
        {
            room.Exit();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"StageController.UnloadRoom: room.Exit threw: {ex}");
        }

        ReleasePoolables(room);
        DestroyRoomGameObject(room);

        if (_currentRoom == room)
        {
            _currentRoom = null;
        }
    }

    /// <summary>
    /// 방에 있는 풀 오브젝트들을 PoolManager에 반환합니다.
    /// </summary>
    /// <param name="room"></param>
    void ReleasePoolables(IRoom room)
    {
        if (room == null) return;

        var poolables = room.PoolableObjectsInRoom;

        if (poolables == null || poolables.Count == 0) return;

        var poolMgr = GameManager.Instance?.PoolManager;
        foreach (var go in poolables)
        {
            if (go == null) continue;

            if (poolMgr != null)
            {
                poolMgr.ReleaseToPool(go);
            }

            else
            {
                Destroy(go);
            }
        }
    }
        void DestroyRoomGameObject(IRoom room)
    {
        if (room is MonoBehaviour mb)
        {
            if (mb.gameObject.scene.IsValid())
            {
                Destroy(mb.gameObject);
            }
            else
            {
                Debug.LogWarning($"DestroyRoomGameObject: room appears to be an asset, skipping destroy: {mb.name}");
            }
        }
    }
}