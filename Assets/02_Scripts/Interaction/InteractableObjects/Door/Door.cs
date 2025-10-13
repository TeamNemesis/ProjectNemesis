using UnityEngine;

public enum DoorType
{
    Start,
    Normal,
    Shop,
    Lab,
    Colosseum,
    Boss
}

public abstract class Door : MonoBehaviour
{
    //[SerializeField] DoorInteractor _doorInteractor; // DoorInteractor 컴포넌트

    public abstract string DoorName { get; }    // 문 이름(자식에서 반드시 정의)
    public abstract float DoorChance { get; }   // 문 등장 확률(자식에서 반드시 정의)
    public abstract DoorType DoorType { get; }  // 문 타입(자식에서 반드시 정의)

    public abstract void Initialize();
}