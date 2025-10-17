using System;
using System.Collections.Generic;
using UnityEngine;

// 방 생성 플로우를 생각해보자
// 1. 시작 시 StartRoom 생성
// 2. StartRoom에서 다음 방으로 넘어갈 때, RoomSpawner가 NormalRoom 생성 -> 이건 고정
// 3. 방에 들어갔어. 이 시점에 MapController가 DoorDecider에게 다음 방 선택지 개수를 물어봄
// 4. DoorDecider는 현재 방의 타입과 인덱스를 기반으로 다음 방 후보군을 구성하고, 확률적으로 개수를 결정하여 반환
// 5. MapController는 개수만큼 DoorSpawner에게 문 생성 요청, DoorSpawner는 개수를 받아서 정해진 위치에 문 생성
// 그럼 이시점에서 문 생성 위치를 어떻게 정하지?
// 아, 애초에 DoorSpanwer에게 요청할 때 위치 정보와 방의 타입에 따라 문을 생성하게 하면 되겠네
// 
// 6. 
public class MapController : MonoBehaviour
{
    // GetNextRoomCount()
    // -> 다음 방 선택지 개수 결정을 int로 받아옴

    // GetNextRoomTypes(int count, RoomType currentRoomType, int currentRoomIndex, bool hasLabRoomAppeared, out int normalRoomCount)
    // -> 다음 방 선택지 개수, 현재 방 타입, 현재 방 인덱스, 실험실 등장 여부를 인자로 넘기고, 다음 방 선택지로 등장할 방들의 타입을 RoomType배열로 받아옴
    // 이때 normalRoomCount는 out으로 몇개의 일반방이 선택되었는지 받아옴

    // GetNextNormalRoomTypes(int normalRoomCount, out techSelectPackCount)
    // -> normalRoomCount만큼 일반방 타입을 결정하여 NormalRoomType배열로 받아오고, techSelectPackCount는 out으로 몇개의 기술선택팩 방이 선택되었는지 받아옴

    // GetNextTechSelectPackTypes(int techSelectPackCount)
    // -> techSelectPackCount만큼 기술선택팩 방 타입을 결정하여 TechSelectPackType배열로 받아옴

    [SerializeField] RoomSpawner _roomSpawner; // 방 생성 컴포넌트
    [SerializeField] DoorSpawner _doorSpawner; // 문 생성 컴포넌트
    [SerializeField] DoorDecider _doorDecider; // 다음 방 선택지 결정 컴포넌트

    [SerializeField] Room _currentRoom; // 현재 방
    [SerializeField] Door[] _currentDoors; // 현재 방의 문들
    [SerializeField] int _currentRoomCount = -1; // 현재 몇번째 방인지(시작 방은 0, 다음 방은 1, ...)
    [SerializeField] bool _hasLabRoomAppeared = false; // 실험실 방이 이미 등장했는지 여부

    public void Initialize()
    {
        _roomSpawner.OnRoomSpawned += OnRoomSpawned;
        _doorSpawner.DoorInteracted += OnDoorInteracted;

        _roomSpawner.Initialize();
        //_doorSpawner.Initialize();
        _doorDecider.Initialize();
    }

    void OnDoorInteracted(IInteractable interactable)
    {
        DoorInteractor doorInteractor = interactable as DoorInteractor;
        if (doorInteractor != null)
        {
            SpawnRoom(doorInteractor.RoomInfo);
        }
        else
        {
            Debug.LogError("OnDoorInteracted 호출 시 interactable이 Door가 아닙니다!");
        }
    }

    public void SpawnRoom(RoomInfo roomInfo)
    {
        if (roomInfo == null)
        {
            Debug.LogError("SpawnRoom 호출 시 roomInfo가 null입니다! 호출자 스택을 확인하세요.");
            return;
        }

        if (_roomSpawner == null)
        {
            Debug.LogError("MapController._roomSpawner가 null입니다! Inspector에서 할당되었는지 확인하세요.");
            return;
        }

        // 정상 호출이면 진행
        // 현재 방을 파괴하고 새로운 방 생성 요청
        if (_currentRoom != null)
        {
            DestroyCurrentRoomObjects();
        }
        _roomSpawner.SpawnRoom(roomInfo.RoomType, roomInfo.NormalRoomType, roomInfo.TechSelectPackType);
    }

    /// <summary>
    /// Room이 생성되었을 때 호출되는 함수
    /// </summary>
    /// <param name="room"></param>
    void OnRoomSpawned(Room room)
    {
        // 현재 방 갱신
        _currentRoom = room;
        _currentRoomCount++;
        if (room.RoomInfo.RoomType == RoomType.Lab)
        {
            _hasLabRoomAppeared = true;
        }

        int nextDoorCount = _doorDecider.GetNextDoorCount();
        int normalRoomCount;
        int techSelectPackCount = 0;

        Transform[] doorPositions = _currentRoom.GetNextDoorPositions(nextDoorCount);

        RoomType[] doorTypes = _doorDecider.GetNextRoomTypes(
            nextDoorCount,
            _currentRoom.RoomInfo.RoomType,
            _currentRoomCount,
            _hasLabRoomAppeared,
            out normalRoomCount);

        NormalRoomType[] normalRoomTypes = new NormalRoomType[0];
        if (normalRoomCount > 0)
        {
            normalRoomTypes = _doorDecider.GetNormalRoomTypes(normalRoomCount, out techSelectPackCount);
        }

        TechSelectPackType[] techPackTypes = new TechSelectPackType[0];
        if (techSelectPackCount > 0)
        {
            techPackTypes = GameManager.Instance.skillManager.GetSkillPackTypes(techSelectPackCount);
        }

        // 생성된 문을 모을 리스트
        List<Door> createdDoors = new List<Door>();

        int normalIdx = 0;
        int techIdx = 0;

        for (int i = 0; i < nextDoorCount; i++)
        {
            RoomType rt = (i < doorTypes.Length) ? doorTypes[i] : RoomType.Normal;

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
                        {
                            tpt = techPackTypes[techIdx++];
                        }
                        else
                        {
                            Debug.LogWarning("techPackTypes가 부족합니다. 기본값으로 처리합니다.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("normalRoomTypes가 부족합니다. 기본 Normal으로 처리합니다.");
                }
            }

            var nextRoomInfo = new RoomInfo(rt, nrt, tpt);

            Door door = _doorSpawner.SpawnDoor(doorPositions[i], nextRoomInfo);
            if (door != null)
            {
                // 부모를 현재 방으로 설정하면 방 파괴 시 문도 같이 파괴됨
                door.transform.SetParent(_currentRoom.transform, worldPositionStays: true);

                createdDoors.Add(door);

                Debug.Log($"Spawned door instance: {door.name} (roomType={nextRoomInfo.RoomType})");
            }
            else
            {
                Debug.LogWarning($"Door spawn failed at index {i}");
            }
        }

        // createdDoors를 배열로 보관
        _currentDoors = createdDoors.ToArray();

        if (Array.Exists(doorTypes, t => t == RoomType.Lab))
        {
            _hasLabRoomAppeared = true;
        }
    }

    void DestroyCurrentRoomObjects()
    {
        // 파괴할 방
        if (_currentRoom != null)
        {
            // 안전 체크: 이 객체가 씬에 있는 인스턴스인가?
            if (_currentRoom.gameObject.scene.IsValid())
            {
                Debug.Log($"Destroying current room instance: {_currentRoom.name}");
                Destroy(_currentRoom.gameObject);
            }
            else
            {
                Debug.LogWarning($"CurrentRoom appears to be an asset, not a scene instance: {_currentRoom.name}. Skipping Destroy.");
            }
            _currentRoom = null;
        }

        // 문들 파괴
        if (_currentDoors != null)
        {
            foreach (var door in _currentDoors)
            {
                if (door == null) continue;

                if (door.gameObject.scene.IsValid())
                {
                    Debug.Log($"Destroying door instance: {door.name}");
                    Destroy(door.gameObject);
                }
                else
                {
                    Debug.LogWarning($"Door appears to be an asset, not a scene instance: {door.name}. Skipping Destroy.");
                }
            }
            _currentDoors = null;
        }
    }
}