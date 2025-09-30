using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipSlotType
{
    None = -1,
    Helmet,
    Armor,
    Weapon,
    Shield,
    Boots,
    Acc
}

/// <summary>
/// 장비 클래스
/// </summary>
public class Equipment : MonoBehaviour
{
    [Header("----- 장비 슬롯 -----")]
    [SerializeField] EquipSlotType _equipSlotType;

    [Header("----- 장비 스탯 -----")]
    [SerializeField] float _bonusMaxHp;
    [SerializeField] float _bonusArmor;
    [SerializeField] float _bonusDamage;

    // 이 장비가 어떤 아이템에서 비롯된 것인지
    EquipmentItemModel _itemModel;

    public virtual EquipSlotType EquipSlotType => _equipSlotType;

    public float BonusMaxHp => _bonusMaxHp;
    public float BonusArmor => _bonusArmor;
    public float BonusDamage => _bonusDamage;

    public EquipmentItemModel ItemModel => _itemModel;

    public void SetItemModel(EquipmentItemModel itemModel)
    {
        _itemModel = itemModel;
    }
}
