using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable,     // 소모성 아이템
    NonConsumable,  // 비소모성 아이템
    Equipment,      // 장비 아이템
}

/// <summary>
/// 아이템의 설정 데이터 클래스
/// </summary>
[CreateAssetMenu(fileName = "ItemConfig", menuName = "GameSettings/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    [Header("----- 아이템 설정 데이터 -----")]
    [SerializeField] string _id;                            // 아이템 ID
    [SerializeField] ItemType _itemType;                    // 아이템 타입
    [SerializeField] string _itemName;                      // 아이템 이름
    [TextArea(3,5)][SerializeField] string _description;    // 아이템 설명
    [SerializeField] int _price;                            // 아이템 가격
    [SerializeField] Sprite _iconSprite;                    // 아이템 아이콘
    [SerializeField] ItemEffect _acquiredEffect;            // 획득 시 효과
    [SerializeField] ItemEffect _usedEffect;                // 사용 시 효과

    public string Id => _id;                                // 아이템 ID
    public ItemType ItemType => _itemType;                  // 아이템 타입
    public string ItemName => _itemName;                    // 아이템 이름
    public string Description => _description;              // 아이템 설명
    public int Price => _price;                              // 아이템 가격
    public Sprite IconSprite => _iconSprite;                // 아이템 아이콘
    public ItemEffect AcquiredEffect => _acquiredEffect;
    public ItemEffect UsedEffect => _usedEffect;
}