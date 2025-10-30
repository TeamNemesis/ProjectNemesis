using System;
using UnityEngine;

public class ShopItem : InteractableObject
{
    [SerializeField] int _price;
    [SerializeField] ShopItemType _itemType;
    [SerializeField] RewardInteractableObject _reward;
    
    public override InteractableType InteractableType => InteractableType.ShopItem;
    public ShopItemType ItemType => _itemType;

    public override event Action<IInteractable> OnInteracted;

    public void Initialize()
    {
        _reward.Initialize();
    }

    public override void GetInteractionMessage(out string title, out string instruction)
    {
        title = $"({_price} 크레딧)";
        instruction = "구매하려면 E키를 누르세요.";
    }

    public override bool TryInteract(Transform subject)
    {
        if (!GameManager.Instance.CurrencyManager.TrySpendCredit(_price))
        {
            return false; // 잔액 부족으로 상호작용 거부
        }
        OnInteracted?.Invoke(this);
        // 구매 성공 처리 (예: 아이템 지급 등)
        _reward.TryInteract(subject);
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        return true;
    }
}