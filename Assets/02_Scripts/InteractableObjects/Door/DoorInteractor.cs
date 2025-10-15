using System;
using UnityEngine;

/// <summary>
/// Door의 상호작용을 담당하는 클래스
/// </summary>
public class DoorInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] RoomType _roomType; // 이 문이 연결하는 방의 타입

    public override Vector3 GuidePoint => _guidePoint.position;

    public override InteractableType InteractableType => InteractableType.Door;
    public RoomType RoomType => _roomType;

    public override event Action<IInteractable> OnInteracted;

    public override void Interact(Transform subject)
    {
        Debug.Log("문과 상호작용 함");
        OnInteracted?.Invoke(this);
    }

    /// <summary>
    /// 이 문이 연결되는 방의 타입을 설정하는 함수
    /// 방이 생성됨과 동시에 문이 생성될 때 각 문에 달려있는 이 DoorInteractor의 RoomType을 설정해줘야 합니다.
    /// </summary>
    /// <param name="roomType"></param>
    public void SetRoomType(RoomType roomType)
    {
        _roomType = roomType;
    }
}
