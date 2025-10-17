using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    /// <summary>
    /// 입력받은 위치와 RoomInfo를 기반으로 문을 생성하여 반환
    /// </summary>
    /// <param name="position"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public Door SpawnDoor(Transform position, RoomInfo info)
    {
        // 문 생성
        GameObject go = GameManager.Instance.ResourceManager.DoorPrefab;
        Door door = go.GetComponent<Door>();
        if (door == null) { Debug.LogError("DoorPrefab에 Door 컴포넌트가 없습니다."); return null; }
        door.Initialize(info); // Door.Initialize(RoomInfo) 구현
        return door;
    }
}