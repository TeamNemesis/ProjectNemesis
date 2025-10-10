using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Shop,
    Lab,
    Colosseum,
    Boss
}

public abstract class Room : MonoBehaviour
{
    [SerializeField] string _roomName; // 방의 이름
    [SerializeField] protected float _roomChance; // 방이 선택될 확률
    [SerializeField] protected RoomType _roomType; // 방의 타입

    public string RoomName => _roomName;
    public float RoomChance => _roomChance;
    public RoomType RoomType => _roomType;

    public void GenerateRoom()
    {

    }
}