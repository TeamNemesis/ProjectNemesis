using System;
using UnityEngine;

/// <summary>
/// 방을 스폰하는 역할을 하는 클래스
/// RoomInfo(타입 + 옵션)를 받아서 SO 기반으로 인스턴스화 및 초기화를 수행한다.
/// </summary>
public class RoomSpawner : MonoBehaviour
{
    public event Action<Room> OnRoomSpawned; // 방이 생성될 때 발생하는 이벤트
    public event Action OnRewardSelectionFinished;  // 보상 선택이 완료되었을 때 발생하는 이벤트

    /// <summary>
    /// RoomInfo를 받아 해당 Room을 생성한다. position/parent를 지정하지 않으면 (0,0,0)과 씬 루트에 생성된다.
    /// </summary>
    public Room SpawnRoom(RoomInfo roomInfo)
    {
        #region null 체크
        if (roomInfo == null)
        {
            Debug.LogError("RoomSpawner.SpawnRoom: roomInfo is null.");
            return null;
        }

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("RoomSpawner: GameManager.Instance is null.");
            return null;
        }

        var dataManager = gm.DataManager;
        if (dataManager == null)
        {
            Debug.LogError("RoomSpawner: DataManager is null on GameManager.");
            return null;
        }
        #endregion

        // RoomDataSO 조회
        if (!dataManager.TryGetRoomData(roomInfo.RoomType, out var roomData) || roomData == null)
        {
            Debug.LogError($"RoomSpawner: RoomData not found for RoomType {roomInfo.RoomType}.");
            return null;
        }

        // RoomDataSo에서 프리팹 가져오기
        var prefab = roomData.RoomPrefab;
        if (prefab == null)
        {
            Debug.LogError($"RoomSpawner: RoomData '{roomData.name}' has no assigned RoomPrefab.");
            return null;
        }

        // 기본 위치에 인스턴스화
        GameObject roomObj = null;
        try
        {
            roomObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        catch (Exception ex)
        {
            Debug.LogError($"RoomSpawner: Failed to instantiate prefab '{prefab.name}': {ex}");
            return null;
        }

        // Room 컴포넌트 검사 및 초기화
        var room = roomObj.GetComponent<Room>();
        // Room 컴포넌트가 없으면 오류 로그 출력 후 인스턴스 파괴
        if (room == null)
        {
            Debug.LogError($"RoomSpawner: Instantiated prefab '{prefab.name}' has no Room component. Destroying instance.");
            Destroy(roomObj);
            return null;
        }

        // SO 기반 초기화(정책: Room 내부에서 필요한 값들을 SO로부터 적용)
        try
        {
            room.OnRewardSelectionFinished += RaiseRewardSelectionFinishedEvent;
            room.Initialize(roomInfo);
        }
        catch (Exception ex)
        {
            Debug.LogError($"RoomSpawner: Exception during InitializeFromRoomData: {ex}");
            Destroy(roomObj);
            return null;
        }

        // 이벤트는 초기화 이후에 발생
        OnRoomSpawned?.Invoke(room);
        return room;
    }

    public void RaiseRewardSelectionFinishedEvent()
    {
        OnRewardSelectionFinished?.Invoke();
    }
}