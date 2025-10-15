using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    /// <summary>
    /// 입력받은 위치에 문을 생성하고, 해당 문에 방 타입 정보를 설정
    /// </summary>
    /// <param name="position"></param>
    /// <param name="roomtype"></param>
    public void SpawnDoor(Transform position, RoomType roomtype)
    {
        // 문 생성
        GameObject doorObj = GameManager.Instance.ResourceManager.DoorPrefab;
        Instantiate(doorObj, position.position, position.rotation, transform);
        Door door = doorObj.GetComponent<Door>();
        if (door != null)
        {
            door.Initialize(roomtype);
        }
        else
        {
            Debug.LogError("DoorPrefab에 Door 컴포넌트가 없습니다.");
        }
    }
}