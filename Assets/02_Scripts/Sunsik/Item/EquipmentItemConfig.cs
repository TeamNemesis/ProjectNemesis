using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 아이템의 설정 데이터
/// </summary>
[CreateAssetMenu(fileName = "EquipmentItemConfig", menuName = "GameSettings/EquipmentItemConfig")]
public class EquipmentItemConfig : ItemConfig
{
    [Header("----- 장비 설정 데이터 -----")]
    [SerializeField] Equipment _equipmentPrefab;

    public Equipment EquipmentPrefab => _equipmentPrefab;
}
