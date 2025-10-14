using System;
using UnityEngine;

public class WeaponInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] WeaponType _weaponType;

    public Vector3 GuidePoint => _guidePoint.position;

    public InteractableType InteractableType => InteractableType.Weapon;
    public WeaponType WeaponType => _weaponType;

    public event Action<WeaponType> OnWeaponInteracted;

    public void Interact(Transform subject)
    {
        Debug.Log("무기와 상호작용 함");
        OnWeaponInteracted?.Invoke(_weaponType);
    }
}