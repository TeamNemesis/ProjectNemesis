using System;
using UnityEngine;

/// <summary>
/// Door의 상호작용을 담당하는 클래스
/// </summary>
public class DoorInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;

    public override Vector3 GuidePoint => _guidePoint.position;

    public override InteractableType InteractableType => InteractableType.Door;

    public override event Action<IInteractable> OnInteracted;

    public override void Interact(Transform subject)
    {
        Debug.Log("문과 상호작용 함");
    }
}
