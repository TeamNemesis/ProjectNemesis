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
    [SerializeField] float door_Chance = 0.7f; // 노말룸의 등장 확률 설정
    [SerializeField] RoomType door_Type = RoomType.Normal;

    public override string RoomName => _roomName;
    public override float RoomChance => door_Chance;
    
}