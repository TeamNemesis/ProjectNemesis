using System;
using UnityEngine;

public class WeaponInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] WeaponType _weaponType;

    public override Vector3 GuidePoint => _guidePoint.position;

    public override InteractableType InteractableType => InteractableType.Weapon;
    public WeaponType WeaponType => _weaponType;

    public override event Action<IInteractable> OnInteracted;

    public override void Interact(Transform subject)
    {
        Debug.Log("무기와 상호작용 함");
        OnInteracted?.Invoke(this);
    }
}