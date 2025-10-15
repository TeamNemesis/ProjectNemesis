using UnityEngine;

public class StartRoom : Room
{
    [SerializeField] string _roomName = "startDoor";
    [SerializeField] float _roomChance = 1.0f; // 시작룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Start;

    public override string RoomName => _roomName;
    public override float RoomChance => _roomChance;
    public override RoomType RoomType => _roomType;

    public override void Initialize()
    {
        //Debug.Log("시작룸 생성됨");
    }
}