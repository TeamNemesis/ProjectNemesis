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
        if (room.RoomInfo.RoomType == RoomType.Lab)
        {
            _hasLabRoomAppeared = true;
        }

        if (room.RoomInfo.RoomType == RoomType.Start)
        {
            // RoomType이 StartRoom일 때는 문 생성 로직을 타지 않고
            // 다음 방은 무조건 NormalRoom이어야 하므로
            StartRoom startRoom = room as StartRoom;

            return;
        }

        // 문의 개수 결정
        int nextDoorCount = _doorDecider.GetNextDoorCount();
        int normalRoomCount;
        int techSelectPackCount = 0;

        // 문 위치 얻기
        Transform[] doorPositions = _currentRoom.GetNextDoorPositions(nextDoorCount);

        // RoomType 배열 얻기
        RoomType[] doorTypes = _doorDecider.GetNextRoomTypes(
            nextDoorCount,
            _currentRoom.RoomInfo.RoomType,
            _currentRoomCount,
            _hasLabRoomAppeared,
            out normalRoomCount);

        // 일반방 타입들 얻기 (normalRoomCount가 0이면 GetNormalRoomTypes는 빈 배열 반환해야 함)
        NormalRoomType[] normalRoomTypes = new NormalRoomType[0];
        if (normalRoomCount > 0)
        {
            normalRoomTypes = _doorDecider.GetNormalRoomTypes(normalRoomCount, out techSelectPackCount);
        }

        // 기술팩 타입들 얻기
        TechSelectPackType[] techPackTypes = new TechSelectPackType[0];
        if (techSelectPackCount > 0)
        {
            techPackTypes = GameManager.Instance.skillManager.GetSkillPackTypes(techSelectPackCount);
        }

        // 유효성 검사: 배열 길이 체크
        if (doorTypes.Length != nextDoorCount)
            Debug.LogWarning("doorTypes length != nextDoorCount");

        if (normalRoomTypes.Length < normalRoomCount)
            Debug.LogError("normalRoomTypes 부족");

        if (techPackTypes.Length < techSelectPackCount)
            Debug.LogError("techPackTypes 부족");

        // 소비 인덱스
        int normalIdx = 0;
        int techIdx = 0;

        // 각 문 생성
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
                    // 만약 NormalRoomType이 기술팩을 의미하면 techPack에서 꺼내 연결
                    if (nrt == NormalRoomType.TechSelect) // (예시 enum 값 이름)
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

            // RoomInfo 생성
            var nextRoomInfo = new RoomInfo(rt, nrt, tpt);

            // DoorSpawner에 문 생성 요청 (아래 예시 SpawnDoor 시그니처 참고)
            Door door = _doorSpawner.SpawnDoor(doorPositions[i], nextRoomInfo);
        }

        // 선택지에 실험실(Lab)이 포함되어 있으면 플래그 처리할지 결정
        // (주의: 보통은 '실제로 방에 들어갔을 때' 플래그를 true로 하는 것이 맞음)
        // 예: if (doorTypes.Any(t => t == RoomType.Lab)) _hasLabRoomAppeared = true;
        if (Array.Exists(doorTypes, t => t == RoomType.Lab))
        {
            _hasLabRoomAppeared = true;
        }
    }


}