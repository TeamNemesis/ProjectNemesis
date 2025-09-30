using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 모델(런타임 데이터 + 비즈니스 로직)
/// 인벤토리에 포함되기 전에는 설정 데이터만 갖는다.
/// </summary>
public class ItemModel
{
    ItemConfig _config;  // 아이템 설정 데이터
    protected Inventory _inventory;
    int _slotIndex = -1;

    public ItemConfig Config => _config;  // 아이템 설정 데이터 접근자
    public ItemType ItemType => _config.ItemType;
    public int SlotIndex => _slotIndex;
    public virtual EquipSlotType EquipSlotType => EquipSlotType.None;
    public virtual bool IsEquipped => false;
        
    public ItemModel(ItemConfig config)
    {
        _config = config;
    }

    public void SetSlotIndex(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    /// <summary>
    /// 아이템이 인벤토리에 추가될 때 자동으로 호출되어야 하는 함수
    /// 비소모성 아이템의 경우 패시브 효과를 적용한다.
    /// </summary>
    /// <param name="inventory"></param>
    public void Acquire(Inventory inventory, int slotIndex)
    {
        _inventory = inventory;
        SetSlotIndex(slotIndex);

        if (_config.AcquiredEffect == null) return;
        _config.AcquiredEffect.Apply(inventory);
    }

    /// <summary>
    /// 아이템을 사용하는 함수
    /// </summary>
    public virtual void Use()
    {
        if (_inventory == null) return;
        
        if(_config.UsedEffect == null) return;
        _config.UsedEffect.Apply(_inventory);
    }

    /// <summary>
    /// 아이템이 인벤토리에서 제거될 때 자동으로 호출되어야 하는 함수
    /// 비소모성 아이템의 경우 패시브 효과를 해제한다.
    /// </summary>
    public void Remove()
    {
        _slotIndex = -1;
        if(_inventory == null) return;

        if (_config.AcquiredEffect == null) return;
        _config.AcquiredEffect.Disapply(_inventory);
    }

    /// <summary>
    /// 이 아이템을 slotType에 장착할 수 있는지 여부를 반환하는 함수
    /// </summary>
    /// <param name="slotType"></param>
    /// <returns></returns>
    public virtual bool GetIsEquippable(EquipSlotType slotType)
    {
        return false;
    }
}

public class EquipmentItemModel : ItemModel
{
    Equipment _equipmentPrefab;
    bool _isEquipped = false;

    public Equipment EquipmentPrefab => _equipmentPrefab;
    public override EquipSlotType EquipSlotType => _equipmentPrefab.EquipSlotType;
    public override bool IsEquipped => _isEquipped;

    public EquipmentItemModel(EquipmentItemConfig config) : base(config)
    {
        _equipmentPrefab = config.EquipmentPrefab;
    }

    public override void Use()
    {
        if (_inventory == null) return;

        // 장비 장착
        _inventory.EquipController.Equip(this);
    }

    public void SetIsEquipped(bool isEquipped)
    {
        _isEquipped = isEquipped;
    }
    public override bool GetIsEquippable(EquipSlotType slotType)
    {
        return EquipSlotType == slotType;
    }
}