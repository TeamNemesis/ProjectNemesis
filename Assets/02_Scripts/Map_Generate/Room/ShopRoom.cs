using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 상점 방: 풀링을 사용해서 ShopItem을 결정/스폰한다.
/// 수정 포인트:
/// - 모든 null 가능성 방어(SpawnPoints, GameManager/PoolManager, 맵 키 등)
/// - 랜덤 색출에서 float→int 버그 수정
/// - 중복 방지 (같은 프리팹/키가 중복으로 나오지 않음)
/// - TechSelect 패키지는 반드시 1개 포함, HealPack는 반드시 1개 포함
/// - 풀에서 실패하면 ResourceManager/Resources.Load 폴백
/// - 실패 시 안전하게 빈 리스트 반환 (NullReferenceException 방지)
/// - ShopItem 초기화는 IInitializePoolable 인터페이스가 있으면 호출 (직접 Initialize 호출 제거, 존재 여부 확인)
/// </summary>
public class ShopRoom : Room
{
    [Header("확률")]
    [SerializeField] float _techSelectChance = 0.4f;
    [SerializeField] float _upgradePackChance = 0.4f;
    [SerializeField] float _mutantChance = 0.1f;

    // 내부 상태
    List<ShopItem> _decidedShopItems = new List<ShopItem>();

    // 매핑: ShopItemType/TechSelectPackType -> 리소스/풀 키 (프로젝트 규칙에 맞게 값 설정)
    Dictionary<ShopItemType, string> _shopItemMap = new Dictionary<ShopItemType, string>()
    {
        {ShopItemType.HealPack,    Constants.RESOURCES_PATH_SHOPITEMS + "/HealPack" },
        {ShopItemType.TechSelectPack, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack" },
        {ShopItemType.TechUpgradePack, Constants.RESOURCES_PATH_SHOPITEMS + "/TechUpgradePack" },
        {ShopItemType.MutantPack,  Constants.RESOURCES_PATH_SHOPITEMS + "/MutantPack" },
    };

    Dictionary<TechSelectPackType, string> _techSelectPackMap = new Dictionary<TechSelectPackType, string>()
    {
        {TechSelectPackType.Company1, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company1" },
        {TechSelectPackType.Company2, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company2" },
        {TechSelectPackType.Company3, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company3" },
        {TechSelectPackType.Company4, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company4" },
        {TechSelectPackType.Company5, Constants.RESOURCES_PATH_SHOPITEMS + "/TechSelectPack_Company5" },
    };

    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        // SpawnReward만 호출. RewardSelectionFinished를 바로 호출하면 안됨(아직 구매/선택 안됨).
        SpawnReward();
    }

    public override IInteractable[] SpawnReward()
    {
        // 안전 검사: 스폰 포인트 유효성
        if (_rewardSpawnPoints == null || _rewardSpawnPoints.Length == 0)
        {
            Debug.LogWarning($"ShopRoom.SpawnReward: reward spawn points are not set. (room={name})");
            return Array.Empty<IInteractable>();
        }

        // DecideRewards는 항상 유효한 리스트(빈 리스트 포함)를 반환
        List<ShopItem> decided = DecideRewards(4);
        if (decided == null || decided.Count == 0)
            return Array.Empty<IInteractable>();

        // IInteractable 배열로 변환하여 반환
        return decided.Cast<IInteractable>().ToArray();
    }

    /// <summary>
    /// count 개수만큼 상점 아이템을 결정해서 풀/리소스에서 스폰한다.
    /// 항상 중복(같은 프리팹 키) 없이, HealPack 1개와 TechSelect 1개 이상을 보장한다.
    /// 실패시는 가능한 범위 내에서 채우고 실패 로그 출력.
    /// </summary>
    List<ShopItem> DecideRewards(int count)
    {
        // 기본 방어
        if (count < 1)
        {
            Debug.LogWarning("ShopRoom.DecideRewards: count must be >= 1");
            return new List<ShopItem>();
        }

        _decidedShopItems.Clear();

        // 의존성 검사
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("ShopRoom.DecideRewards: GameManager.Instance is null");
            return new List<ShopItem>();
        }
        var pool = gm.PoolManager;
        var resMgr = gm.ResourceManager;

        // 스폰 위치 안전성
        if (_rewardSpawnPoints == null || _rewardSpawnPoints.Length == 0)
        {
            Debug.LogWarning("ShopRoom.DecideRewards: reward spawn points not configured");
            return new List<ShopItem>();
        }

        // 사용한 키 추적해서 중복 방지
        HashSet<string> usedKeys = new HashSet<string>(StringComparer.Ordinal);

        Vector3 basePos = _rewardSpawnPoints[0].position;

        // 1) HealPack 반드시 1개
        if (_shopItemMap.TryGetValue(ShopItemType.HealPack, out var healKey) && !string.IsNullOrEmpty(healKey))
        {
            GameObject healObj = SafeSpawn(healKey, basePos, pool, resMgr, 0);
            if (healObj != null)
            {
                if (TryGetShopItemComponent(healObj, out var healItem))
                {
                    _decidedShopItems.Add(healItem);
                    usedKeys.Add(healKey);
                }
                else
                {
                    ReleaseOrDestroy(pool, healObj);
                    Debug.LogWarning($"ShopRoom.DecideRewards: Heal prefab for key '{healKey}' does not contain ShopItem component.");
                }
            }
            else Debug.LogWarning($"ShopRoom.DecideRewards: Failed to spawn HealPack for key '{healKey}'.");
        }
        else
        {
            Debug.LogWarning("ShopRoom.DecideRewards: HealPack key not found in _shopItemMap");
        }

        // 2) TechSelect 반드시 1개 (랜덤 회사)
        if (_techSelectPackMap.Count == 0)
        {
            Debug.LogWarning("ShopRoom.DecideRewards: _techSelectPackMap is empty");
        }
        else
        {
            // 랜덤 회사 선택 (int index)
            var techKeys = _techSelectPackMap.Keys.ToArray();
            int randIndex = UnityEngine.Random.Range(0, techKeys.Length);
            var chosenTech = techKeys[randIndex];
            if (_techSelectPackMap.TryGetValue(chosenTech, out var techKey) && !string.IsNullOrEmpty(techKey))
            {
                GameObject techObj = SafeSpawn(techKey, basePos + Vector3.right * 1f, pool, resMgr, 1);
                if (techObj != null)
                {
                    if (TryGetShopItemComponent(techObj, out var techItem))
                    {
                        _decidedShopItems.Add(techItem);
                        usedKeys.Add(techKey);
                    }
                    else
                    {
                        ReleaseOrDestroy(pool, techObj);
                        Debug.LogWarning($"ShopRoom.DecideRewards: TechSelect prefab for key '{techKey}' does not contain ShopItem component.");
                    }
                }
                else Debug.LogWarning($"ShopRoom.DecideRewards: Failed to spawn TechSelectPack for key '{techKey}'.");
            }
        }

        // 3) 나머지 슬롯 채우기 (중복 방지, 확률 기반)
        float totalProb = _techSelectChance + _upgradePackChance + _mutantChance;
        if (totalProb <= 0f)
        {
            Debug.LogWarning("ShopRoom.DecideRewards: probabilities sum to 0 or less. Defaulting to equal chance.");
            totalProb = 1f;
            _techSelectChance = _upgradePackChance = _mutantChance = 1f / 3f;
        }

        int attemptsLimit = 20; // 무한 루프 방지
        int filled = _decidedShopItems.Count;
        int nextSlotIndex = 2;
        int attempts = 0;

        while (filled < count && attempts < attemptsLimit)
        {
            attempts++;
            float r = UnityEngine.Random.Range(0f, totalProb);

            string pickKey = null;
            ShopItemType? pickedType = null;
            // tech 선택
            if (r < _techSelectChance)
            {
                // pick a tech pack that isn't used yet (if possible)
                var availableTechs = _techSelectPackMap
                    .Where(kv => !usedKeys.Contains(kv.Value))
                    .ToArray();
                if (availableTechs.Length == 0)
                {
                    // 모든 tech 키가 이미 사용되었으면 allow any (will cause duplicate prefab maybe)
                    availableTechs = _techSelectPackMap.ToArray();
                }

                var pick = availableTechs[UnityEngine.Random.Range(0, availableTechs.Length)];
                pickKey = pick.Value;
                // mark as ShopItemType.TechSelectPack for potential checks (not strictly required)
                pickedType = ShopItemType.TechSelectPack;
            }
            else if (r < _techSelectChance + _upgradePackChance)
            {
                if (_shopItemMap.TryGetValue(ShopItemType.TechUpgradePack, out var key) && !usedKeys.Contains(key))
                {
                    pickKey = key;
                    pickedType = ShopItemType.TechUpgradePack;
                }
                else
                {
                    // 이미 사용되었거나 키 없음: skip this attempt
                    continue;
                }
            }
            else
            {
                if (_shopItemMap.TryGetValue(ShopItemType.MutantPack, out var key) && !usedKeys.Contains(key))
                {
                    pickKey = key;
                    pickedType = ShopItemType.MutantPack;
                }
                else
                {
                    continue;
                }
            }

            if (string.IsNullOrEmpty(pickKey))
                continue;

            // 중복 검사: 같은 키가 이미 사용되었으면 skip
            if (usedKeys.Contains(pickKey))
                continue;

            // 스폰
            GameObject spawned = SafeSpawn(pickKey, basePos + Vector3.right * (nextSlotIndex * 1.1f), pool, resMgr, nextSlotIndex);
            if (spawned == null)
            {
                Debug.LogWarning($"ShopRoom.DecideRewards: SafeSpawn failed for key '{pickKey}'");
                continue;
            }

            if (!TryGetShopItemComponent(spawned, out var spawnedItem))
            {
                ReleaseOrDestroy(pool, spawned);
                Debug.LogWarning($"ShopRoom.DecideRewards: Spawned prefab for key '{pickKey}' does not contain ShopItem.");
                continue;
            }

            // 성공적으로 얻음
            _decidedShopItems.Add(spawnedItem);
            usedKeys.Add(pickKey);
            filled++;
            nextSlotIndex++;
        }

        if (filled < count)
        {
            Debug.LogWarning($"ShopRoom.DecideRewards: requested {count} items but only filled {filled} after {attempts} attempts.");
        }

        return _decidedShopItems;
    }

    // 안전한 스폰(풀 -> ResourceManager -> Resources.Load 순서)
    GameObject SafeSpawn(string key, Vector3 pos, PoolManager pool, ResourceManager resMgr, int slotIndex)
    {
        GameObject instance = null;

        if (pool != null)
        {
            try
            {
                instance = pool.GetFromPool(key, pos, Quaternion.identity, this.transform);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"SafeSpawn: PoolManager.GetFromPool threw for key '{key}': {ex.Message}");
                instance = null;
            }
        }

        if (instance == null && resMgr != null)
        {
            var prefab = resMgr.LoadResource<GameObject>(key);
            if (prefab != null)
                instance = Instantiate(prefab, pos, Quaternion.identity, this.transform);
        }

        if (instance == null)
        {
            // 마지막 폴백: Resources.Load 경로 규칙을 쓴 경우
            var prefab = Resources.Load<GameObject>(key);
            if (prefab != null)
                instance = Instantiate(prefab, pos, Quaternion.identity, this.transform);
        }

        return instance;
    }

    // ShopItem 컴포넌트 추출(루트 또는 자식)
    bool TryGetShopItemComponent(GameObject go, out ShopItem shopItem)
    {
        shopItem = null;
        if (go == null) return false;

        shopItem = go.GetComponent<ShopItem>() ?? go.GetComponentInChildren<ShopItem>(true);
        if (shopItem != null)
        {
            shopItem.Initialize();
            return true;
        }

        return false;
    }

    void ReleaseOrDestroy(PoolManager pool, GameObject go)
    {
        if (go == null) return;
        if (pool != null)
            pool.ReleaseToPool(go);
        else
            Destroy(go);
    }
}