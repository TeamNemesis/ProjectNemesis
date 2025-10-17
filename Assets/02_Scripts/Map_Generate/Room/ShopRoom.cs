using UnityEngine;

public class ShopRoom : Room
{
    [SerializeField] string _roomName = "shopDoor";
    [SerializeField] float _roomChance = 0.1f; // 상점룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Shop;

    public override string RoomName => _roomName;
    public override float RoomChance => _roomChance;
    public override RoomType RoomType => _roomType;
}