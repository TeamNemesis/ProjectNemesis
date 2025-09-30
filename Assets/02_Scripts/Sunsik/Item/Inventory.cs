using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유저가 보유한 아이템들을 관리하는 클래스
/// </summary>
public class Inventory : MonoBehaviour
{
    const int _slotCount = 18;      // 인벤토리 슬롯 개수

    [Header("----- 아이템 설정 데이터 -----")]
    [SerializeField] ItemConfig[] _itemConfigs;      // 아이템 설정 데이터 배열
    Dictionary<string, ItemConfig> _itemConfigMap = new();  // 아이템 설정 데이터 맵

    [Header("----- 컴포넌트 참조 -----")]
    //[SerializeField] ItemView[] _itemViews;         // 아이템 뷰 배열
    //[SerializeField] ItemView _selectedItemView;    // 내가 선택한 아이템 뷰
    //[SerializeField] ItemDescView _itemDescView;          // 아이템 툴팁 창
    //[SerializeField] ItemDragController _dragController;

    [SerializeField] EquipController _equipController;
    // 영웅 모델 (아이템을 장착할 영웅)
    [SerializeField] HeroModel _heroModel;

    public HeroModel HeroModel => _heroModel; // 영웅 모델 프로퍼티
    public EquipController EquipController => _equipController;

    // 슬롯 변경 이벤트
    public event Action<int, ItemModel> OnSlotChanged;

    // 유저가 보유하고 있는 아이템 배열
    ItemModel[] _itemModels = new ItemModel[_slotCount];

    public void Initialize()
    {
        // 아이템 설정 데이터를 맵에 저장
        foreach (var itemConfig in _itemConfigs)
        {
            _itemConfigMap[itemConfig.Id] = itemConfig;
        }
    }

    /// <summary>
    /// 아이템 설정 데이터로 아이템 모델을 만들어 반환해 주는 함수
    /// </summary>
    /// <param name="itemConfig"></param>
    /// <returns></returns>
    public ItemModel CreateItemModel(ItemConfig itemConfig)
    {
        switch (itemConfig)
        {
            case EquipmentItemConfig equipmentItemConfig:
                return new EquipmentItemModel(equipmentItemConfig);
            default:
                return new ItemModel(itemConfig);
        }
    }

    /// <summary>
    /// 슬롯 번호로 아이템 모델 반환을 시도하는 함수
    /// </summary>
    /// <param name="slotIndex">슬롯 번호</param>
    /// <param name="itemModel">찾은 아이템 모델</param>
    /// <returns>아이템 모델 존재 여부</returns>
    public bool TryGetItemModel(int slotIndex, out ItemModel itemModel)
    {
        itemModel = null;

        // 인덱스 범위 검사
        if (slotIndex < 0 || slotIndex >= _itemModels.Length) return false;

        itemModel = _itemModels[slotIndex];
        return itemModel != null;
    }

    /// <summary>
    /// 아이템 슬롯이 비어 있는지 여부를 반환하는 함수
    /// </summary>
    /// <param name="slotIndex">슬롯 번호</param>
    /// <returns></returns>
    public bool GetIsEmptySlot(int slotIndex)
    {
        if(slotIndex <0 || slotIndex >= _itemModels.Length) return false;

        ItemModel itemModel = _itemModels[slotIndex];
        return itemModel == null;
    }

    /// <summary>
    /// 아이템 Id로 아이콘을 획득하는 함수
    /// </summary>
    /// <param name="id">획득할 아이템의 Id</param>
    public void AddItem(string id)
    {
        if (_itemConfigMap.ContainsKey(id) == false)
        {
            Debug.LogWarning($"존재하지 않는 아이템입니다. (id: {id})");
            return;
        }

        // 아이템 설정 데이터 검색
        ItemConfig itemConfig = _itemConfigMap[id];

        for (int i = 0; i < _slotCount; i++)
        {
            if (_itemModels[i] == null)
            {
                // 아이템 설정 데이터로 아이템 모델 생성
                _itemModels[i] = CreateItemModel(itemConfig);

                // 아이템 획득 시 실행되어야 하는 함수 호출
                _itemModels[i].Acquire(this, i);

                // 슬롯 변경 이벤트 발행
                OnSlotChanged?.Invoke(i, _itemModels[i]);
                return;
            }
        }

        Debug.Log($"아이템 슬롯이 가득 찼습니다.");
    }

    /// <summary>
    /// 이미 있는 아이템을 인벤토리에 추가 시도하는 함수
    /// </summary>
    /// <param name="itemModel"></param>
    /// <returns></returns>
    public bool TryAddItem(ItemModel itemModel)
    {
        for (int i = 0; i < _itemModels.Length; i++)
        {
            if (_itemModels[i] == null)
            {
                _itemModels[i] = itemModel;
                itemModel.SetSlotIndex(i);
                OnSlotChanged?.Invoke(i, itemModel);
                return true;
            }
        }

        Debug.Log("아이템 슬롯이 가득 찼습니다.");
        return false;
    }

    /// <summary>
    /// 아이템을 제거하는 함수
    /// </summary>
    /// <param name="slotIndex">제거할 아이템 슬롯 번호</param>
    public void RemoveItem(int slotIndex)
    {
        if (TryGetItemModel(slotIndex, out ItemModel itemModel) == true)
        {
            itemModel.Remove();
            _itemModels[slotIndex] = null;
            OnSlotChanged?.Invoke(slotIndex, null);
        }
    }

    /// <summary>
    /// 아이템을 사용하는 함수
    /// </summary>
    /// <param name="slotIndex">사용할 아이템 슬롯 번호</param>
    public void UseItem(int slotIndex)
    {
        if (TryGetItemModel(slotIndex, out ItemModel itemModel) == true)
        {
            if (itemModel.ItemType == ItemType.Consumable || itemModel.ItemType == ItemType.Equipment)
            {
                RemoveItem(slotIndex);
            }
            itemModel.Use();
        }
    }

    /// <summary>
    /// 두 아이템의 자리를 바꾸는 함수
    /// </summary>
    /// <param name="a">첫 번째 아이템의 슬롯 번호</param>
    /// <param name="b">두 번째 아이템의 슬롯 번호</param>
    public void SwapItems(int a, int b)
    {
        Debug.Log($"{a}번 아이템과 {b}번 아이템의 자리를 바꿉니다.");

        // a번 ItemModel과 b번 ItemModel의 자리 교환
        ItemModel temp = _itemModels[a];
        _itemModels[a] = _itemModels[b];
        _itemModels[b] = temp;

        _itemModels[a]?.SetSlotIndex(a);
        _itemModels[b]?.SetSlotIndex(b);

        OnSlotChanged?.Invoke(a, _itemModels[a]);
        OnSlotChanged?.Invoke(b, _itemModels[b]);
    }
    
    // 테스트
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem("Mace");
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            AddItem("Apple");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            AddItem("RedRing");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            AddItem("Sword");
        }
    }
}
