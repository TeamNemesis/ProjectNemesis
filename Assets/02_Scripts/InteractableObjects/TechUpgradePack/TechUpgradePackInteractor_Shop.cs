using UnityEngine;

public class TechUpgradePackInteractor_Shop : TechUpgradePackInteractor, IShopItem
{
    [SerializeField] int _price = 100;
    public int Price => _price;
    public override bool TryInteract(Transform subject)
    {
        if(Purchase())
        {
            return base.TryInteract(subject);
        }
        Debug.Log("Tech Upgrade Pack을 구매할 수 있는 크레딧이 부족합니다.");
        return false;
    }
    public bool Purchase()
    {
        if(GameManager.Instance.CurrencyManager.TrySpendCredit(_price))
        {
            return true;
        }
        return false;
    }
}