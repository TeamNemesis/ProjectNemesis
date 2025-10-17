using UnityEngine;

public class LabRoom : Room
{
    [SerializeField] string _roomName = "labDoor";
    [SerializeField] float _roomChance = 0.1f; // 실험실룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Lab;

    public override string RoomName => _roomName;
    public override float RoomChance => _roomChance;
    public override RoomType RoomType => _roomType;
}