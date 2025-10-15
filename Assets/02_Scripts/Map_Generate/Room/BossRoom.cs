using UnityEngine;

public class BossRoom : Room
{
    // 보스룸에 문이 있나?([serializeField]를 통해 직렬화 해놨으므로 배열이 비어도 null은 아님)
    [SerializeField] string _roomName = "bossDoor";
    [SerializeField] float _roomChance = 0.05f; // 보스룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Boss;

    public override string RoomName => _roomName;

    public override float RoomChance => _roomChance;

    public override RoomType RoomType => _roomType;

    public override void Initialize()
    {
        
    }
}