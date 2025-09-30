using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 장착/해제를 총괄하는 클래스
/// </summary>
public class EquipController : MonoBehaviour
{
    Dictionary<EquipSlotType, Equipment> _equipmentMap = new();

    [SerializeField] HeroModel _heroModel;
    [SerializeField] Inventory _inventory;

    [SerializeField] Transform[] _slotTransforms;       // 장비 슬롯 부모 트랜스폼

    /// <summary>
    /// 무기 장착 이벤트. HitPoint 전달
    /// </summary>
    public event Action<Transform> OnWeaponEquipped;

    /// <summary>
    /// 슬롯 변경 이벤트
    /// </summary>
    public event Action<EquipSlotType, Equipment> OnSlotChanged;

    /// <summary>
    /// 슬롯 타입으로 장비 반환을 시도하는 함수
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="equipment"></param>
    /// <returns></returns>
    public bool TryGetEquipment(EquipSlotType slotType, out Equipment equipment)
    {
        equipment = null;
        return _equipmentMap.TryGetValue(slotType, out equipment);
    }

    /// <summary>
    /// 장비를 장착하는 함수
    /// 기존 장비가 있으면 자동으로 해제 후 새 장비를 장착한다.
    /// </summary>
    /// <param name="itemModel"></param>
    public void Equip(EquipmentItemModel itemModel)
    {
        EquipSlotType slotType = itemModel.EquipmentPrefab.EquipSlotType;
        int slotIndex = (int)slotType;
        if (slotIndex < 0 || slotIndex >= _slotTransforms.Length) return;

        // 기존 장비가 있었다면 해제
        Unequip(slotType);

        // 장비 프리펩 생성
        Transform slotTransform = _slotTransforms[slotIndex];
        Equipment equipment = Instantiate(itemModel.EquipmentPrefab, slotTransform);
        equipment.SetItemModel(itemModel);

        // 딕셔너리 등록
        _equipmentMap[slotType] = equipment;

        // 장비 스탯 적용
        _heroModel.AddMaxHp(equipment.BonusMaxHp);
        _heroModel.AddArmor(equipment.BonusArmor);
        _heroModel.AddDamage(equipment.BonusDamage);

        // is 키워드 사용(cf. as 키워드)
        // 어떤 객체가 해당 클래스가 맞는지 검사할 수 있다.
        if (equipment is Weapon weapon)
        {
            OnWeaponEquipped?.Invoke(weapon.HitPoint);
        }

        itemModel.SetIsEquipped(true);

        // 슬롯 변경 이벤트 발행
        OnSlotChanged?.Invoke(slotType, equipment);
    }

    /// <summary>
    /// 장비를 해제하는 함수
    /// </summary>
    /// <param name="slotType"></param>
    public void Unequip(EquipSlotType slotType)
    {
        if (_equipmentMap.ContainsKey(slotType))
        {
            Equipment equipment = _equipmentMap[slotType];
            EquipmentItemModel itemModel = equipment.ItemModel;

            // 1. 해당 장비와 연결된 아이템 모델을 인벤토리에 추가
            // -> 인벤토리에 아이템 추가 실패 시 장비 해제 불가
            if (_inventory.TryAddItem(equipment.ItemModel) == false) return;

            // 2. 장비로 인한 능력치 변화 해제
            _heroModel.AddMaxHp(-equipment.BonusMaxHp);
            _heroModel.AddArmor(-equipment.BonusArmor);
            _heroModel.AddDamage(-equipment.BonusDamage);

            // 3. 장비 제거
            Destroy(equipment.gameObject);

            // 4. 장비 맵에서 키 제거
            _equipmentMap.Remove(slotType);

            // 5. 무기의 경우 무기 제거 이벤트 알림
            if (slotType == EquipSlotType.Weapon)
            {
                OnWeaponEquipped?.Invoke(null);
            }

            itemModel.SetIsEquipped(false);

            // 6. 슬롯 변경 이벤트 발행
            OnSlotChanged?.Invoke(slotType, null);
        }
    }

    public void Unequip(ItemModel itemModel)
    {
        if (_equipmentMap.ContainsKey(itemModel.EquipSlotType) == true)
        {
            if (_equipmentMap[itemModel.EquipSlotType].ItemModel == itemModel)
            {
                Unequip(itemModel.EquipSlotType);
            }
        }
    }
}
