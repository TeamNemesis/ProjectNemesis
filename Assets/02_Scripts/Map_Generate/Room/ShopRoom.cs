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

    List<ShopItem> _decidedShopItems = new();

    Dictionary<ShopItemType, string> _shopItemMap = new Dictionary<ShopItemType, string>()
    {
        {ShopItemType.HealPack, Constants.RESOURCES_PATH_SHOPITEMS + "/HealPack" },
        {ShopItemType.TechSelectPack, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack" },
        {ShopItemType.TechUpgradePack, Constants.RESOURCES_PATH_SHOPITEMS + "/UpgradePack" },
        {ShopItemType.MutantPack, Constants.RESOURCES_PATH_SHOPITEMS + "/Mutant" },
    };

    Dictionary<TechSelectPackType, string> _techSelectPackMap = new Dictionary<TechSelectPackType, string>()
    {
        {TechSelectPackType.Company1, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company1" },
        {TechSelectPackType.Company2, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company2" },
        {TechSelectPackType.Company3, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company3" },
        {TechSelectPackType.Company4, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company4" },
        {TechSelectPackType.Company5, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company5" },
    };

    public override IInteractable[] SpawnReward()
    {
        DecideRewards(4);
        IInteractable[] interactables = new IInteractable[_decidedShopItems.Count];

        for(int i = 0; i < _decidedShopItems.Count; i++)
        {
            interactables[i] = _decidedShopItems[i];
        }
        return interactables;
    }

    List<ShopItem> DecideRewards(int count)
    {
        if(count < 4)
        {
            Debug.LogWarning("ShopRoom.DecideRewards: 보상 개수는 최소 4개여야 합니다.");
            return null;
        }
        // 초기화
        _decidedShopItems.Clear();
        // 체력회복 키트는 무조건 1개 포함
        GameObject healObj = GameManager.Instance.PoolManager.GetFromPool(_shopItemMap[ShopItemType.HealPack], _rewardSpawnPoints[0].position, Quaternion.identity, transform);
        ShopItem healItem = healObj.GetComponent<ShopItem>();
        healItem.Initialize();
        _decidedShopItems.Add(healItem);

        // 기술 선택 팩 1개 반드시 포함
        float randomTechPackIndex = Random.Range(0, _techSelectPackMap.Count);
        GameObject techObj = GameManager.Instance.PoolManager.GetFromPool(_techSelectPackMap[(TechSelectPackType)randomTechPackIndex], _rewardSpawnPoints[1].position, Quaternion.identity, transform);
        ShopItem techItem = techObj.GetComponent<ShopItem>();
        techItem.Initialize();
        _decidedShopItems.Add(techItem);

        float total = _techSelectChance + _upgradePackChance + _mutantChance;

        // 확률에 따라 나머지 보상 결정
        for (int i = 0; i < count - 2; i++)
        {
            float random = Random.Range(0f, total);
            if (random < _techSelectChance)
            {
                GameObject techPack = GameManager.Instance.PoolManager.GetFromPool(_techSelectPackMap[(TechSelectPackType)randomTechPackIndex], _rewardSpawnPoints[i + 2].position, Quaternion.identity, transform);
            }
            else if(random < _techSelectChance + _upgradePackChance)
            {
                GameObject upgradeObj = GameManager.Instance.PoolManager.GetFromPool(_shopItemMap[ShopItemType.TechUpgradePack], _rewardSpawnPoints[i + 2].position, Quaternion.identity, transform);
                ShopItem upgradeItem = upgradeObj.GetComponent<ShopItem>();
                upgradeItem.Initialize();
                _decidedShopItems.Add(upgradeItem);
            }
            else
            {
                GameObject mutantObj = GameManager.Instance.PoolManager.GetFromPool(_shopItemMap[ShopItemType.MutantPack], _rewardSpawnPoints[i + 2].position, Quaternion.identity, transform);
                ShopItem mutantItem = mutantObj.GetComponent<ShopItem>();
                mutantItem.Initialize();
                _decidedShopItems.Add(mutantItem);
            }
        }
        return _decidedShopItems;
    }
}