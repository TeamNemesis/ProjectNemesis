using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : Room
{
    Dictionary<ShopItemType, string> _shopItemPathMap = new()
    {
        { ShopItemType.MutantPack , "Prefabs/ShopItems/MutantPack" },
        {ShopItemType.HealPack , "Prefabs/ShopItems/HealPack" },
        {ShopItemType.TechSelectPack , "Prefabs/ShopItems/TechSelectPack" },
        {ShopItemType.TechUpgradePack , "Prefabs/ShopItems/TechUpgradePack" },
    };

    Dictionary<TechSelectPackType, string> _techSelectPackPathMap = new()
    {
        {TechSelectPackType.Company1, "Prefabs/ShopItems/TechSelectPack_Company1" },
        {TechSelectPackType.Company2, "Prefabs/ShopItems/TechSelectPack_Company2" },
        {TechSelectPackType.Company3, "Prefabs/ShopItems/TechSelectPack_Company3" },
        {TechSelectPackType.Company4, "Prefabs/ShopItems/TechSelectPack_Company4" },
        {TechSelectPackType.Company5, "Prefabs/ShopItems/TechSelectPack_Company5" },
    };

    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        SpawnReward();
        RewardSelectionFinished();
    }

    public override IInteractable[] SpawnReward()
    {
        ShopItemType[] selectedTypes = DecideShopItems(4);
        List<IInteractable> spawnedItems = new List<IInteractable>();
        for (int i = 0; i < selectedTypes.Length; i++)
        {
            ShopItemType itemType = selectedTypes[i];
            if (!_shopItemPathMap.TryGetValue(itemType, out string prefabPath))
            {
                Debug.LogWarning($"ShopRoom.SpawnReward: 아이템 타입 {itemType}에 대한 프리팹 경로를 찾을 수 없습니다.");
                continue;
            }
            GameObject itemPrefab = Resources.Load<GameObject>(prefabPath);
            if (itemPrefab == null)
            {
                Debug.LogWarning($"ShopRoom.SpawnReward: 경로 '{prefabPath}'에서 아이템 프리팹을 로드할 수 없습니다.");
                continue;
            }
            Transform spawnPoint = _rewardSpawnPoints.Length > i ? _rewardSpawnPoints[i] : _rewardSpawnPoints[0];
            GameObject itemInstance = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation, this.transform);
            ShopItem shopItem = itemInstance.GetComponent<ShopItem>();
            if (shopItem == null)
            {
                Debug.LogWarning($"ShopRoom.SpawnReward: 인스턴스화된 오브젝트에 ShopItem 컴포넌트가 없습니다.");
                continue;
            }
            // TechSelectPack인 경우, 랜덤 회사 팩으로 초기화
            if (itemType == ShopItemType.TechSelectPack)
            {
                TechSelectPackType randomPackType = (TechSelectPackType)Random.Range(0, System.Enum.GetValues(typeof(TechSelectPackType)).Length);
                if (_techSelectPackPathMap.TryGetValue(randomPackType, out string techPackPath))
                {
                    GameObject techPackPrefab = Resources.Load<GameObject>(techPackPath);
                    if (techPackPrefab != null)
                    {
                        Destroy(itemInstance);
                        itemInstance = Instantiate(techPackPrefab, spawnPoint.position, spawnPoint.rotation, this.transform);
                        shopItem = itemInstance.GetComponent<ShopItem>();
                    }
                }
            }
            shopItem.Initialize();
            spawnedItems.Add(shopItem);


        }
        return spawnedItems.ToArray();
    }

    ShopItemType[] DecideShopItems(int count = 4)
    {
        List<ShopItemType> selectedTypes = new List<ShopItemType>();
        // 4개 중 한개는 무조건 체력 회복
        selectedTypes.Add(ShopItemType.HealPack);

        // 4개 중 한개는 무조건 기술 선택 팩
        selectedTypes.Add(ShopItemType.TechSelectPack);

        // 나머지 2개는 체력 회복팩을 제외한 나머지 아이템들 중에서 랜덤 선택
        ShopItemType[] possibleTypes = new ShopItemType[]
        {
            ShopItemType.TechUpgradePack,
            ShopItemType.MutantPack,
            ShopItemType.TechSelectPack,
        };
        while (selectedTypes.Count < count)
        {
            ShopItemType randomType = possibleTypes[Random.Range(0, possibleTypes.Length)];
            if (!selectedTypes.Contains(randomType))
            {
                selectedTypes.Add(randomType);
            }
        }
        return selectedTypes.ToArray();
    }
}
