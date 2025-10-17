using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] DoorInteractor _doorInteractor;    // 문이 상호작용 가능하도록 하는 컴포넌트
    RoomInfo _roomInfo;

    public void Initialize(RoomInfo info)
    {
        _roomInfo = info;
        SetReward(_roomInfo); // 보상/아이콘 표시 등
    }

    void SetReward(RoomInfo info)
    {
        // info.RoomType / info.NormalRoomType / info.TechSelectPackType에 따라 UI 세팅
    }
}