using System;
using UnityEngine;

/// <summary>
/// 상호작용 가능한 무기(훈련장의 무기)가 상호작용 하기 위해 구현하는 클래스
/// </summary>
public class WeaponInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] WeaponType _weaponType;

    public override Vector3 GuidePoint => _guidePoint.position;

    public override InteractableType InteractableType => InteractableType.Weapon;
    public WeaponType WeaponType => _weaponType;

    public override event Action<IInteractable> OnInteracted;

    public override void StartInteract(Transform subject)
    {
        Debug.Log("무기와 상호작용 함");
        OnInteracted?.Invoke(this);
    }
}