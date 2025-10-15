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
    //[SerializeField] DoorInteractor _doorInteractor; // DoorInteractor 컴포넌트

    public abstract string RoomName { get; }    // 방의 이름(자식에서 반드시 정의)
    public abstract float RoomChance { get; }   // 방의 등장 확률(자식에서 반드시 정의)
    public abstract RoomType RoomType { get; }  // 방의 타입(자식에서 반드시 정의)

    public abstract void Initialize();
}