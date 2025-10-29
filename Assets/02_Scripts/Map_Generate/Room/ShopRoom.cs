using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 구매할 수 있는 아이템은 수리키트, 기술, 업그레이드 키트, 돌연변이 등이다.
/// 구매를 원하는 아이템과 상호작용 하면 아이템을 습득한다.
/// 매 상점에 입점되는 항목은 4개가 주어지며
/// 반드시 수리키트와 랜덤한 회사의 기술을 하나 이상 포함한다.
/// 중복된 항목은 나오지 않는다. (수리키트가 2개 나오지는 않음. 기술은 서로 다른 기술로 2개 가능)
/// 상점에서 아이템을 구매하면 해당 아이템은 매진되며, 해당 방에서는 다시 구매할 수 없다. 
/// </summary>
public class ShopRoom : Room
{
    [SerializeField] float _techSelectChance = 0.4f; // 기술 선택 보상이 나올 확률
    [SerializeField] float _upgradePackChance = 0.4f; // 업그레이드 팩 보상이 나올 확률
    [SerializeField] float _mutantChance = 0.1f; // 돌연변이 보상이 나올 확률

    //Dictionary<ShopItemType, string> _shopItemMap = new Dictionary<ShopItemType, string>()
    //{
    //    {ShopItemType.HealPack, Constants.RESOURCES_PATH_REWARDS + "/HealPack" },
    //    {ShopItemType.TechSelectPack_Company1, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company1" },
    //    {ShopItemType.TechSelectPack_Company2, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company2" },
    //    {ShopItemType.TechSelectPack_Company3, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company3" },
    //    {ShopItemType.TechSelectPack_Company4, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company4" },
    //    {ShopItemType.TechSelectPack_Company5, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company5" },
    //    {ShopItemType.UpgradePack, Constants.RESOURCES_PATH_REWARDS + "/UpgradePack" },
    //    {ShopItemType.Mutant, Constants.RESOURCES_PATH_REWARDS + "/Mutant" },
    //};

    public override IInteractable[] SpawnReward()
    {
        throw new System.NotImplementedException();
    }

    RewardInteractableObject[] DecideRewards(int count)
    {
        //// 보상 count개를 담을 배열 생성
        //RewardInteractableObject[] rewards = new RewardInteractableObject[count];

        throw new System.NotImplementedException();

    }
}