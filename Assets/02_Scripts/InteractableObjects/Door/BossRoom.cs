using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] string _roomName = "bossDoor";
    [SerializeField] float _roomChance = 0.05f; // 보스룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Boss;

    public override string RoomName => _roomName;

    public override float RoomChance => _roomChance;

    public override RoomType RoomType => _roomType;

    public override void Initialize()
    {
        //Debug.Log("보스룸 생성됨");
    }
}