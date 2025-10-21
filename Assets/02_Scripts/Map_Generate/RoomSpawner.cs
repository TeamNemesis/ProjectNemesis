using System;
using UnityEngine;

/// <summary>
/// 방을 스폰하는 역할을 하는 클래스
/// RoomInfo(타입 + 옵션)를 받아서 SO 기반으로 인스턴스화 및 초기화를 수행한다.
/// </summary>
public class RoomSpawner : MonoBehaviour
{
    public event Action<Room> OnRoomSpawned; // 방이 생성될 때 발생하는 이벤트

    public void Initialize()
    {
        // 시작 시 StartRoom 생성 (예시)
        var startInfo = new RoomInfo(RoomType.Start, null, null);
        SpawnRoom(startInfo);
    }

    /// <summary>
    /// RoomInfo를 받아 해당 Room을 생성한다. position/parent를 지정하지 않으면 (0,0,0)과 씬 루트에 생성된다.
    /// </summary>
    public void SpawnRoom(RoomInfo roomInfo, Vector3 position = default, Transform parent = null)
    {
        if (roomInfo == null)
        {
            Debug.LogError("RoomSpawner.SpawnRoom: roomInfo is null.");
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("RoomSpawner: GameManager.Instance is null.");
            return;
        }

        var dataManager = gm.DataManager;
        if (dataManager == null)
        {
            Debug.LogError("RoomSpawner: DataManager is null on GameManager.");
            return;
        }

        if (!dataManager.TryGetRoomData(roomInfo.RoomType, out var roomData) || roomData == null)
        {
            Debug.LogError($"RoomSpawner: RoomData not found for RoomType {roomInfo.RoomType}.");
            return;
        }

        var prefab = roomData.RoomPrefab;
        if (prefab == null)
        {
            Debug.LogError($"RoomSpawner: RoomData '{roomData.name}' has no assigned RoomPrefab.");
            return;
        }

        // Instantiate
        GameObject roomObj = null;
        try
        {
            roomObj = Instantiate(prefab, position, Quaternion.identity, parent);
        }
        catch (Exception ex)
        {
            Debug.LogError($"RoomSpawner: Failed to instantiate prefab '{prefab.name}': {ex}");
            return;
        }

        // Room 컴포넌트 검사 및 초기화
        var room = roomObj.GetComponent<Room>();
        if (room == null)
        {
            Debug.LogError($"RoomSpawner: Instantiated prefab '{prefab.name}' has no Room component. Destroying instance.");
            Destroy(roomObj);
            return;
        }

        // SO 기반 초기화(정책: Room 내부에서 필요한 값들을 SO로부터 적용)
        try
        {
            room.Initialize(roomInfo);
        }
        catch (Exception ex)
        {
            Debug.LogError($"RoomSpawner: Exception during InitializeFromRoomData: {ex}");
            Destroy(roomObj);
            return;
        }

        // 이벤트는 초기화 이후에 발생
        OnRoomSpawned?.Invoke(room);
    }
}