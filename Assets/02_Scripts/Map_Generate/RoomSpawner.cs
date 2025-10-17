using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방을 스폰하는 역할을 하는 클래스
/// 다음 방으로 넘어갈 때 RoomType에 따라 적절한 방을 생성
/// </summary>
public class RoomSpawner : MonoBehaviour
{
    public event Action<Room> OnRoomSpawned; // 방이 생성될 때 발생하는 이벤트

    public void Initialize()
    {
        // 시작 시 StartRoom 생성
        SpawnRoom(RoomType.Start, null, null);
    }

    
    public void SpawnRoom(RoomType roomType, NormalRoomType? normalRoomType, TechSelectPackType? techSelectPackType)
    {
        GameObject roomPrefab = GameManager.Instance.ResourceManager.RoomPrefabMap[roomType];
        if (roomPrefab != null)
        {
            // Instantiate로 새 GameObject 생성
            GameObject roomObj = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

            // 생성된 오브젝트에서 Room 컴포넌트 가져오기
            Room room = roomObj.GetComponent<Room>();
            if (room != null)
            {
                room.Initialize(roomType, normalRoomType, techSelectPackType);
                OnRoomSpawned?.Invoke(room); // 이벤트는 Initialize 이후로 옮기는게 더 안전
            }
            else
            {
                Debug.LogError($"RoomType {roomType}에 해당하는 프리팹에 Room 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"RoomType {roomType}에 해당하는 프리팹이 없습니다.");
        }
    }
}
