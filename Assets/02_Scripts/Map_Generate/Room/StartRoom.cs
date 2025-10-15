using UnityEngine;

public class StartRoom : Room
{
    [SerializeField] string _roomName = "startDoor";
    [SerializeField] float _roomChance = 1.0f; // 시작룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Start;
    [SerializeField] Transform _doorPosForStartRoom; // 시작룸에서 다음 방으로 나가는 문 위치(고정일 경우 사용)

    public override string RoomName => _roomName;
    public override float RoomChance => _roomChance;
    public override RoomType RoomType => _roomType;
    public Transform DoorPosForStartRoom => _doorPosForStartRoom;

    public override void Initialize()
    {
        //Debug.Log("시작룸 생성됨");
    }
}