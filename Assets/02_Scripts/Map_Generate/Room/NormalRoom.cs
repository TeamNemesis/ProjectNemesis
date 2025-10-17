using UnityEngine;

public enum NormalRoomType
{
    Credit, 
    Heal,
    TechSelect, 
    TechUpgrade,
    Chrome,
}

/// <summary>
/// 
/// </summary>
public class NormalRoom : Room
{
    [SerializeField] string _roomName = "normalDoor";
    [SerializeField] float _roomChance = 0.7f; // 노말룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Normal;

    public override string RoomName => _roomName;
    public override float RoomChance => _roomChance;
    public override RoomType RoomType => _roomType;

}