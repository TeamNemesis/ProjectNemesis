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

    [SerializeField] float door_CreditChance; // 크레딧 방 등장 확률
    [SerializeField] float door_HealChance; // 회복 방 등장 확률
    [SerializeField] float door_UpgradeChance; // 업그레이드 방 등장 확률
    [SerializeField] float door_ChromeChance; // 크롬 방 등장 확률
    [SerializeField] float door_SkillPackChance; // 스킬팩 방 등장 확률

    public override string RoomName => door_Name;
    public override float RoomChance => door_Chance;
    
}