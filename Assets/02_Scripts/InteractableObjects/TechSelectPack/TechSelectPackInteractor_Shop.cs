using UnityEngine;
public class TechSelectPackInteractor_Shop : TechSelectPackInteractor, IShopItem
{
    [SerializeField] int _price = 150;
    public int Price => _price;
    public override bool TryInteract(Transform subject)
    {
        if(Purchase())
        {
            return base.TryInteract(subject);
        }
        Debug.Log("Tech Select Pack을 구매할 수 있는 크레딧이 부족합니다.");
        return false;
    }
    public bool Purchase()
    {
        Debug.Log("Attempting to purchase Tech Select Pack for " + _price + " credits.");
        if (GameManager.Instance.CurrencyManager.TrySpendCredit(_price))
        {
            return true;
        }
        return false;
    }
}