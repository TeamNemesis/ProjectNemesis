using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Shop,
    Lab,
    Colosseum,
    Boss
}

public abstract class Room : MonoBehaviour
{
    [SerializeField] protected Transform[] _doorSpawnPointsLeft;  // 왼쪽 문 생성 위치
    [SerializeField] protected Transform[] _doorSpawnPointsRight; // 오른쪽 문 생성 위치

    RoomInfo _roomInfo; // 방의 정보

    public Transform[] DoorSpawnPointsLeft => _doorSpawnPointsLeft;
    public Transform[] DoorSpawnPointsRight => _doorSpawnPointsRight;

    public abstract string RoomName { get; }    // 방의 이름(자식에서 반드시 정의)
    public abstract float RoomChance { get; }   // 방의 등장 확률(자식에서 반드시 정의)
    public RoomInfo RoomInfo => _roomInfo; // 방의 정보


    public virtual void Initialize(RoomType roomType, NormalRoomType? normalRoomType, TechSelectPackType? techSelectPackType)
    {
        _roomInfo = new RoomInfo(roomType, normalRoomType, techSelectPackType);
    }

    /// <summary>
    /// 입력받은 count 수만큼 다음 문들이 생성될 위치들을 Transform배열로 반환
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public Transform[] GetNextDoorPositions(int count)
    {
        if (count < 1 || count > 3)
        {
            Debug.Log("생성될 문의 개수는 1~3개 사이여야 합니다.");
            return null;
        }

        switch (count)
        {
            case 1:
                // 1개일 때는 왼쪽과 오른쪽 중 랜덤으로 하나 선택하여 각 Positions[0]을 반환
                Transform chosen = Random.value < 0.5f
                    ? _doorSpawnPointsLeft[0]
                    : _doorSpawnPointsRight[0];
                return new Transform[] { chosen };
            case 2:
                // 2개일 때는 Left [0], Right[0] 이거나 Left[1], Left[2] 또는 Right[1], Right[2] 중 랜덤으로 선택하여 반환
                // 근데 확률은 다 같음
                float rand2 = Random.value;
                if (rand2 < 1 / 3f)
                {
                    return new Transform[] { _doorSpawnPointsLeft[0], _doorSpawnPointsRight[0] };
                }
                else if (rand2 < 2 / 3f)
                {
                    return new Transform[] { _doorSpawnPointsLeft[1], _doorSpawnPointsLeft[2] };
                }
                else
                {
                    return new Transform[] { _doorSpawnPointsRight[1], _doorSpawnPointsRight[2] };
                }
            case 3:
                // 3개일 때는 Left[1], Left[2], Right[1] 이거나 Left[1], Left[2], Right[2] 이거나
                // Right[1], Right[2], Left[1] 이거나 Right[1], Right[2], Left[2] 중 랜덤으로 선택하여 반환
                float rand3 = Random.value;
                if (rand3 < 0.25f)
                {
                    return new Transform[] { _doorSpawnPointsLeft[1], _doorSpawnPointsLeft[2], _doorSpawnPointsRight[1] };
                }
                else if (rand3 < 0.5f)
                {
                    return new Transform[] { _doorSpawnPointsLeft[1], _doorSpawnPointsLeft[2], _doorSpawnPointsRight[2] };
                }
                else if (rand3 < 0.75f)
                {
                    return new Transform[] { _doorSpawnPointsRight[1], _doorSpawnPointsRight[2], _doorSpawnPointsLeft[1] };
                }
                else
                {
                    return new Transform[] { _doorSpawnPointsRight[1], _doorSpawnPointsRight[2], _doorSpawnPointsLeft[2] };
                }
            default:
                return null;
        }
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
    }
}