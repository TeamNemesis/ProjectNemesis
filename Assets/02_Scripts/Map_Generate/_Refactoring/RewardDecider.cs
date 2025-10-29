using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RewardDecider (MonoBehaviour)
/// - IRewardDecider 구현체로 인스펙터에서 매핑을 편집할 수 있도록 구성했습니다.
/// - 런타임에 고정 맵을 Dictionary로 구성해서 빠른 조회를 제공합니다.
/// - Colosseum / Boss 등 특수 룸 규칙은 DescribeColosseumDecisions 메서드에 추가하세요.
/// - 필요하면 이 클래스를 상속해서 룸별 복잡한 규칙을 구현해도 됩니다.
/// </summary>
public class RewardDecider : MonoBehaviour
{
    [Serializable]
    struct NormalMapEntry
    {
        public NormalRoomType Key;
        public string PrefabKey;
    }

    [Serializable]
    struct TechSelectMapEntry
    {
        public TechSelectPackType Key;
        public string PrefabKey;
    }

    [Header("Normal room -> prefab key map (editable in inspector)")]
    [SerializeField]
    NormalMapEntry[] _normalRewardEntries = new NormalMapEntry[]
    {
        new NormalMapEntry(){ Key = NormalRoomType.Heal, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/HealPack" },
        new NormalMapEntry(){ Key = NormalRoomType.Credit, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/Credit" },
        new NormalMapEntry(){ Key = NormalRoomType.TechUpgrade, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechUpgradePack" },
        new NormalMapEntry(){ Key = NormalRoomType.Chrome, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/Chrome" },
        new NormalMapEntry(){ Key = NormalRoomType.TechSelect, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack" },
    };

    [Header("TechSelect pack -> prefab key map (editable in inspector)")]
    [SerializeField]
    TechSelectMapEntry[] _techSelectEntries = new TechSelectMapEntry[]
    {
        new TechSelectMapEntry(){ Key = TechSelectPackType.Company1, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company1" },
        new TechSelectMapEntry(){ Key = TechSelectPackType.Company2, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company2" },
        new TechSelectMapEntry(){ Key = TechSelectPackType.Company3, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company3" },
        new TechSelectMapEntry(){ Key = TechSelectPackType.Company4, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company4" },
        new TechSelectMapEntry(){ Key = TechSelectPackType.Company5, PrefabKey = Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company5" },
    };

    // 런타임 조회용 딕셔너리
    Dictionary<NormalRoomType, string> _normalRewardMap;
    Dictionary<TechSelectPackType, string> _techSelectPackMap;

    void Awake()
    {
        BuildMaps();
    }

    void OnValidate()
    {
        // 에디터에서 값이 바뀌면 즉시 맵을 재빌드
        BuildMaps();
    }

    void BuildMaps()
    {
        _normalRewardMap = new Dictionary<NormalRoomType, string>();
        if (_normalRewardEntries != null)
        {
            foreach (var e in _normalRewardEntries)
            {
                if (!string.IsNullOrEmpty(e.PrefabKey))
                    _normalRewardMap[e.Key] = e.PrefabKey;
            }
        }

        _techSelectPackMap = new Dictionary<TechSelectPackType, string>();
        if (_techSelectEntries != null)
        {
            foreach (var e in _techSelectEntries)
            {
                if (!string.IsNullOrEmpty(e.PrefabKey))
                    _techSelectPackMap[e.Key] = e.PrefabKey;
            }
        }
    }

    /// <summary>
    /// IRewardDecider 구현: RoomInfo를 기반으로 RewardSpec 배열 반환
    /// - Normal 룸: 항상 1개(요구사항)
    /// - TechSelect인 경우 RoomInfo.TechType이 미리 설정되어 있어야 함 (Door에서 결정)
    /// - Colosseum/Boss/Shop/Lab 규칙은 이곳 또는 서브클래스에서 구현
    /// </summary>
    public virtual RewardSpec[] DecideForRoom(RoomInfo roomInfo, int desiredCount = 1)
    {
        // 유효성 검사
        if (roomInfo == null)
        {
            return Array.Empty<RewardSpec>();
        }

        // Normal 룸 처리 (기본 요구: 보상은 1개)
        if (roomInfo.RoomType == RoomType.Normal && roomInfo.TryGetNormal(out var normalType))
        {
            // TechSelect은 추가 처리 필요 (TechType이 RoomInfo에 있어야 함)
            if (normalType == NormalRoomType.TechSelect)
            {
                if (roomInfo.TryGetTechSelect(out var techType))
                {
                    if (_techSelectPackMap != null && _techSelectPackMap.TryGetValue(techType, out var techKey))
                    {
                        var s = new RewardSpec(prefabKey: techKey, rewardType: , meta: techType, quantity: 1);
                        return new RewardSpec[] { s };
                    }
                    else
                    {
                        Debug.LogWarning($"DefaultRewardDecider: no mapping for TechSelectPackType {techType} (RoomType={roomInfo.RoomType})");
                        return Array.Empty<RewardSpec>();
                    }
                }
                else
                {
                    Debug.LogWarning($"DefaultRewardDecider: TechSelect room without TechType set on RoomInfo (RoomType={roomInfo.RoomType})");
                    return Array.Empty<RewardSpec>();
                }
            }

            // 일반 Normal 타입 매핑
            if (_normalRewardMap != null && _normalRewardMap.TryGetValue(normalType, out var key))
            {
                var s = new RewardSpec(prefabKey: key, rewardType: null, meta: normalType, quantity: 1);
                return new RewardSpec[] { s };
            }
            else
            {
                Debug.LogWarning($"DefaultRewardDecider: no mapping for NormalRoomType {normalType} (RoomType={roomInfo.RoomType})");
                return Array.Empty<RewardSpec>();
            }
        }

        // Colosseum / Boss / Shop / Lab 등 특수 룸 처리 자리
        if (roomInfo.RoomType == RoomType.Colosseum || roomInfo.RoomType == RoomType.Boss)
        {
            // 예: 엘리트/보스 처치 여부에 따라 보상 스펙을 다르게 구성해야 함.
            // 기본 동작: DescribeColosseumDecisions를 호출해 확장 포인트로 사용
            return DescribeColosseumDecisions(roomInfo, desiredCount);
        }

        // Shop, Lab 등은 나중에 추가
        return Array.Empty<RewardSpec>();
    }

    /// <summary>
    /// Colosseum/Boss용 확장 포인트(기본은 빈 배열)
    /// - 여기에서 룰을 구현하거나 이 클래스를 상속/오버라이드하세요.
    /// </summary>
    protected virtual RewardSpec[] DescribeColosseumDecisions(RoomInfo roomInfo, int desiredCount)
    {
        // placeholder: 실제 게임 디자인에 따라 엘리트 수, 보상 등 계산
        // 예: 보스의 등급에 따라 몇 개의 스펙을 반환한다거나
        Debug.Log($"DefaultRewardDecider: DescribeColosseumDecisions called for room {roomInfo.RoomType}. Implement logic as needed.");
        return Array.Empty<RewardSpec>();
    }

    #region 런타임 확장 API

    /// <summary>
    /// 런타임에 매핑을 추가/덮어쓰기 할 수 있는 유틸리티. (테스트/테이블 로드용)
    /// </summary>
    public void SetNormalMapping(NormalRoomType key, string prefabKey)
    {
        if (_normalRewardMap == null) _normalRewardMap = new Dictionary<NormalRoomType, string>();
        _normalRewardMap[key] = prefabKey;
    }

    public void SetTechSelectMapping(TechSelectPackType key, string prefabKey)
    {
        if (_techSelectPackMap == null) _techSelectPackMap = new Dictionary<TechSelectPackType, string>();
        _techSelectPackMap[key] = prefabKey;
    }

    #endregion
}