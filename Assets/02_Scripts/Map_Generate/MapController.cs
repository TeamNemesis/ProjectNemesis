using System;
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
    

    [SerializeField] RoomSpawner _roomSpawner; // 방 생성 컴포넌트
    [SerializeField] DoorSpawner _doorSpawner; // 문 생성 컴포넌트
    [SerializeField] DoorDecider _doorDecider; // 다음 방 선택지 결정 컴포넌트

    [SerializeField] Room _currentRoom; // 현재 방
    [SerializeField] int _currentRoomCount; // 현재 몇번째 방인지(시작 방은 0, 다음 방은 1, ...)
    [SerializeField] bool _hasLabRoomAppeared = false; // 실험실 방이 이미 등장했는지 여부

    public void Initialize()
    {
        _roomSpawner.OnRoomSpawned += OnRoomSpawned;

        _roomSpawner.Initialize();
        //_doorSpawner.Initialize();
        _doorDecider.Initialize();
    }

    /// <summary>
    /// Room이 생성되었을 때 호출되는 함수
    /// </summary>
    /// <param name="room"></param>
    void OnRoomSpawned(Room room)
    {
        // 현재 방 갱신
        _currentRoom = room;
        // 현재 방 카운트 증가
        _currentRoomCount++;
        // 실험실 방 등장 여부 갱신
        if (room.RoomType == RoomType.Lab)
        {
            _hasLabRoomAppeared = true;
        }

        if (room.RoomType == RoomType.Start)
        {
            // RoomType이 StartRoom일 때는 문 생성 로직을 타지 않고
            // 다음 방은 무조건 NormalRoom이어야 하므로
            StartRoom startRoom = room as StartRoom;
            _doorSpawner.SpawnDoor(startRoom.DoorPosForStartRoom, RoomType.Normal);

            return;
        }

        // 생성할 문의 개수 결정(=다음 방 선택지 개수)
        int nextDoorCount = _doorDecider.GetNextDoorCount();

        // 문의 개수만큼 문이 생성될 위치 정보 받아오기
        Transform[] doorPositions = _currentRoom.GetNextDoorPositions(nextDoorCount);

        // 문의 개수만큼 로직에 따라 문의 타입 결정하기
        RoomType[] doorTypes = _doorDecider.GetNextDoorTypes(
            nextDoorCount,
            _currentRoom.RoomType,
            _currentRoomCount,
            _hasLabRoomAppeared);

        for (int i = 0; i < nextDoorCount; i++)
        {
            // 다음 방 선택지 개수만큼 문 생성
            _doorSpawner.SpawnDoor(doorPositions[i], doorTypes[i]);
        }
    }

    
}