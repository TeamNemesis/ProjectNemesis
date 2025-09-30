using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 장비 클래스
/// </summary>
public class Weapon : Equipment
{
    [Header("----- 무기 -----")]
    [SerializeField] Transform _hitPoint;

    public override EquipSlotType EquipSlotType => EquipSlotType.Weapon;
    public Transform HitPoint => _hitPoint;
}
