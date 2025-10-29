using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DoorSpawner: 문 생성 전담 컴포넌트
/// </summary>
public class DoorSpawner : MonoBehaviour
{
    [SerializeField] DoorDecider _doorDecider;

    private void Awake()
    {
        if (_doorDecider == null)
        {
            _doorDecider = GetComponent<DoorDecider>();
            if (_doorDecider == null)
            {
                Debug.LogWarning("DoorSpawner: DoorDecider 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    public List<Door> SpawnDoorsForCurrentRoom(IRoom room)
    {
        // null 처리
        if (room == null) return new List<Door>();
        if (_doorSpawner == null)
        {
            Debug.LogWarning("StageController에 DoorSpawner가 없습니다.");
            return new List<Door>();
        }

        // 생성할 문의 개수 정하기
        int count = 1;
        try
        {
            if (_doorDecider != null)
                count = _doorDecider.GetNextDoorCount(_currentRoomIndex);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"StageController.SpawnDoorsForCurrentRoom: DoorDecider.GetNextDoorCount threw: {ex}");
            return new List<Door>();
        }

        // 문의 개수만큼 다음 방 타입 결정
        int normalCount = 0;
        RoomType[] roomTypes = Array.Empty<RoomType>();
        try
        {
            if (_doorDecider != null)
                roomTypes = _doorDecider.GetNextRoomTypes(count, room.RoomInfo.RoomType, _currentRoomIndex, hasLabRoomAppeared: false, out normalCount);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"StageController.SpawnDoorsForCurrentRoom: DoorDecider.GetNextRoomTypes threw: {ex}");
            return new List<Door>();
        }

        // normal room 내부 타입 결정
        int techSelectCount = 0;
        NormalRoomType[] normalTypes = Array.Empty<NormalRoomType>();
        if (normalCount > 0)
        {
            try
            {
                normalTypes = _doorDecider != null ? _doorDecider.GetNormalRoomTypes(normalCount, out techSelectCount) : new NormalRoomType[normalCount];
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"StageController.SpawnDoorsForCurrentRoom: DoorDecider.GetNormalRoomTypes threw: {ex}");
                return new List<Door>();
            }
        }

        // tech select room 내부 팩 타입 결정
        TechSelectPackType[] techSelectPackTypes = Array.Empty<TechSelectPackType>();
        if (techSelectCount > 0)
        {
            try
            {
                techSelectPackTypes = _doorDecider.GetTechSelectPackTypes(techSelectCount);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"StageController.SpawnDoorsForCurrentRoom: DoorDecider.GetTechSelectPackTypes threw: {ex}");
                techSelectPackTypes = new TechSelectPackType[techSelectCount];
            }
        }

        // 이번에 생성할 문 위치 가져오기
        var doorPositions = room.GetNextDoorPositions(count);
        if (doorPositions == null || doorPositions.Length == 0)
        {
            Debug.LogWarning("StageController.SpawnDoorsForCurrentRoom: No door spawn points found in room. Doors will not be spawned.");
            return new List<Door>();
        }

        // 기존 문 정리
        _currentDoors.Clear();
        var spawnedDoors = new List<Door>();

        // for문 카운트용 인덱스
        int normalIndex = 0;
        int techSelectPackIndex = 0;

        for (int i = 0; i < count; i++)
        {
            // 각 타입이 있는지 검사하기 위해 변수 할당
            NormalRoomType? normalType = null;
            TechSelectPackType? techPack = null;

            // 만약 이번 루프에서 생성할 방이 Normal 타입이라면 내부 타입도 할당
            if (roomTypes[i] == RoomType.Normal)
            {
                // 당연하지만 체크용 카운트가 이번에 결정된 개수보다 많으면 안됨
                if (normalIndex < normalTypes.Length)
                {
                    normalType = normalTypes[normalIndex++];
                }

                // 만약 이번 루프에서 생성할 Normal 방이 TechSelect 타입이라면 내부 팩 타입도 할당
                if (normalType == NormalRoomType.TechSelect)
                {
                    // 당연하지만 체크용 카운트가 이번에 결정된 개수보다 많으면 안됨
                    if (techSelectPackIndex < techSelectPackTypes.Length)
                    {
                        techPack = techSelectPackTypes[techSelectPackIndex++];
                    }
                }
            }

            // RoomInfo에 필요한 정보 세팅
            var roomInfoForThisDoor = new RoomInfo(roomTypes[i], normalType, techPack);

            // 보상 결정하기
            try
            {
                if (_rewardDecider != null)
                {
                    var previewSpecs = _rewardDecider.DecideForRoom(destInfo, desiredCount: 1, preview: true);
                    if (previewSpecs != null && previewSpecs.Length > 0)
                    {
                        destInfo.DecidedRewards = previewSpecs;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"StageController.SpawnDoorsForCurrentRoom: RewardDecider.DecideForRoom threw: {ex}");
            }

            // Spawn the Door via DoorSpawner
            Door door = null;
            try
            {
                door = _doorSpawner.SpawnDoor(doorPositions[i], destInfo, parent: GetRoomDoorParentTransform(room));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"StageController.SpawnDoorsForCurrentRoom: DoorSpawner.SpawnDoor threw: {ex}");
                door = null;
            }

            if (door == null) continue;

            // Default lock policy: Shop & Start unlocked, others locked
            if (destType == RoomType.Shop || destType == RoomType.Start)
                door.Unlock();
            else
                door.Lock();

            _currentDoors.Add(door);
            spawnedDoors.Add(door);
        }

        return spawnedDoors;
    }

}