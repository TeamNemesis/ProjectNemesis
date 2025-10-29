using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// DoorDecider: 다음 방 후보군을 구성하고 확률적으로 선택지를 반환하는 정책 담당 클래스.
/// 
/// 수정/보강 요약:
/// - special-index 체크(0: start, 13: pre-boss, 14: boss) 버그 수정.
/// - DataManager null/빈 맵 방어 추가.
/// - GetNormalRoomTypes에서 가중치 합 0일 때 균등 분포 분기 버그 수정.
/// - BuildCandidateMap에서 null-safe 처리 및 기본 가중치 폴백 제공.
/// - 전반적인 방어 코드(예외/로그) 보강.
/// </summary>
public class DoorDecider : MonoBehaviour
{
    [Header("다음 방 선택지 개수 확률 (기본값)")]
    [SerializeField] float _oneDoorChance = 0.1f; // 다음 방 1개 확률
    [SerializeField] float _twoDoorChance = 0.6f; // 다음 방 2개 확률
    [SerializeField] float _threeDoorChance = 0.3f; // 다음 방 3개 확률

    [Header("NormalRoom의 세부 타입 확률 (디폴트, Inspector에서 조정 가능)")]
    [SerializeField] float door_CreditChance = 0.2f;
    [SerializeField] float door_HealChance = 0.2f;
    [SerializeField] float door_UpgradeChance = 0.2f;
    [SerializeField] float door_ChromeChance = 0.2f;
    [SerializeField] float door_SkillPackChance = 0.2f;

    /// <summary>초기화 훅(필요시 사용)</summary>
    public void Initialize() { /* reserved for future use */ }

    // -------------------- public API --------------------

    public int GetNextDoorCount(int currentRoomIndex)
    {
        if (currentRoomIndex < 1 || currentRoomIndex > 14)
            throw new ArgumentOutOfRangeException(nameof(currentRoomIndex), "방 인덱스는 1~14 사이여야 합니다.");

        // 특수 규칙: 시작 방(index 1), 보스 직전(index 13), 보스 방(index 14)
        if (currentRoomIndex == 1)
        {
            Debug.Log("[DoorDecider] GetNextDoorCount: start room -> force 1");
            return 1;
        }
        if (currentRoomIndex == 13)
        {
            Debug.Log("[DoorDecider] GetNextDoorCount: pre-boss -> force 1");
            return 1;
        }
        if (currentRoomIndex == 14)
        {
            Debug.Log("[DoorDecider] GetNextDoorCount: boss room -> force 0");
            return 0;
        }

        float one = Mathf.Max(0f, _oneDoorChance);
        float two = Mathf.Max(0f, _twoDoorChance);
        float three = Mathf.Max(0f, _threeDoorChance);

        float totalChance = one + two + three;
        if (totalChance <= 0f)
        {
            Debug.LogWarning("[DoorDecider] door-count weights sum to 0, using uniform fallback.");
            // uniform among 1..3
            return UnityEngine.Random.Range(1, 4); // 1..3
        }

        float r = UnityEngine.Random.Range(0f, totalChance);
        if (r < one) return 1;
        if (r < one + two) return 2;
        return 3;
    }

    /// <summary>
    /// count 길이의 RoomType 배열을 반환한다.
    /// 반드시 length == count를 만족해야 한다.
    /// </summary>
    public RoomType[] GetNextRoomTypes(int count, RoomType currentRoomType, int currentRoomIndex, bool hasLabRoomAppeared, out int normalRoomCount)
    {
        ValidateGetNextRoomTypesArgs(count, currentRoomIndex);

        normalRoomCount = 0;
        if (count == 0) return Array.Empty<RoomType>();

        // 1) 우선 포함 규칙 적용 (특수 인덱스)
        var chosen = new List<RoomType>();
        ApplySpecialRules(currentRoomIndex, chosen);

        // 2) 후보군 구성 (DataManager 기반, null-safe)
        var candidate = BuildCandidateMap();

        // 3) 후보군 정리(이미 등장한 Lab, 현재 방 타입, 이미 선택된 특수 타입 제거)
        PruneCandidates(candidate, currentRoomType, hasLabRoomAppeared, chosen);

        // 4) 남은 슬롯 채우기
        FillRemainingSlots(chosen, candidate, count);

        normalRoomCount = chosen.Count(x => x == RoomType.Normal);
        return chosen.ToArray();
    }

    /// <summary>
    /// NormalRoom 타입들을 반환. 길이는 요청한 normalRoomCount와 같음.
    /// techSelectPackCount는 선택된 TechSelect 개수를 out.
    /// </summary>
    public NormalRoomType[] GetNormalRoomTypes(int normalRoomCount, out int techSelectPackCount)
    {
        if (normalRoomCount < 0 || normalRoomCount > 3)
            throw new ArgumentOutOfRangeException(nameof(normalRoomCount), "일반방 개수는 0~3개 사이여야 합니다.");

        var result = new List<NormalRoomType>(normalRoomCount);
        techSelectPackCount = 0;

        if (normalRoomCount == 0) return result.ToArray();

        float credit = Mathf.Max(0f, door_CreditChance);
        float heal = Mathf.Max(0f, door_HealChance);
        float upgrade = Mathf.Max(0f, door_UpgradeChance);
        float chrome = Mathf.Max(0f, door_ChromeChance);
        float skill = Mathf.Max(0f, door_SkillPackChance);

        float total = credit + heal + upgrade + chrome + skill;
        bool useUniform = total <= 0f;
        if (useUniform) Debug.LogWarning("[DoorDecider] GetNormalRoomTypes: normal-type weights sum to 0, using uniform fallback.");

        for (int i = 0; i < normalRoomCount; i++)
        {
            if (useUniform)
            {
                int idx = UnityEngine.Random.Range(0, 5);
                switch (idx)
                {
                    case 0: result.Add(NormalRoomType.Credit); break;
                    case 1: result.Add(NormalRoomType.Heal); break;
                    case 2: result.Add(NormalRoomType.TechUpgrade); break;
                    case 3: result.Add(NormalRoomType.Chrome); break;
                    default: result.Add(NormalRoomType.TechSelect); techSelectPackCount++; break;
                }
                continue;
            }

            float r = UnityEngine.Random.Range(0f, total);
            if (r < credit) result.Add(NormalRoomType.Credit);
            else if (r < credit + heal) result.Add(NormalRoomType.Heal);
            else if (r < credit + heal + upgrade) result.Add(NormalRoomType.TechUpgrade);
            else if (r < credit + heal + upgrade + chrome) result.Add(NormalRoomType.Chrome);
            else { result.Add(NormalRoomType.TechSelect); techSelectPackCount++; }
        }

        return result.ToArray();
    }

    public TechSelectPackType[] GetTechSelectPackTypes(int techSelectPackCount)
    {
        if (techSelectPackCount < 0) techSelectPackCount = 0;
        if (techSelectPackCount == 0) return Array.Empty<TechSelectPackType>();

        var mgr = GameManager.Instance?.skillManager;
        if (mgr == null)
        {
            Debug.LogWarning("[DoorDecider] skillManager is null, returning default tech packs");
            return Enumerable.Repeat(TechSelectPackType.Default, techSelectPackCount).ToArray();
        }

        return mgr.GetSkillPackTypes(techSelectPackCount);
    }

    // ------------------------- 헬퍼들 -------------------------

    void ValidateGetNextRoomTypesArgs(int count, int currentRoomIndex)
    {
        if (count < 0 || count > 3) throw new ArgumentOutOfRangeException(nameof(count), "문의 개수는 0~3개 사이여야 합니다.");
        if (currentRoomIndex < 0 || currentRoomIndex > 14) throw new ArgumentOutOfRangeException(nameof(currentRoomIndex), "방 인덱스는 0~14 사이여야 합니다.");
    }

    /// <summary>
    /// special rules: index==12 -> include Shop, index==13 -> include Boss
    /// (이 함수는 chosen 리스트에 우선 포함 항목을 추가한다)
    /// </summary>
    void ApplySpecialRules(int currentRoomIndex, List<RoomType> chosen)
    {
        if (currentRoomIndex == 12)
        {
            chosen.Add(RoomType.Shop);
            Debug.Log("[DoorDecider] Rule applied: index==12 -> include Shop");
        }
        if (currentRoomIndex == 13)
        {
            chosen.Add(RoomType.Boss);
            Debug.Log("[DoorDecider] Rule applied: index==13 -> include Boss");
        }
    }

    /// <summary>
    /// DataManager 기반 후보군 생성. null-safe. RoomDataSO.BaseChance를 사용하되
    /// 없으면 균등 기본 가중치(1)로 폴백.
    /// </summary>
    Dictionary<RoomType, float> BuildCandidateMap()
    {
        var map = new Dictionary<RoomType, float>();

        var dataManager = GameManager.Instance?.DataManager;
        if (dataManager == null || dataManager.RoomDataMap == null || dataManager.RoomDataMap.Count == 0)
        {
            Debug.LogWarning("[DoorDecider] DataManager.RoomDataMap is empty; using uniform default weights for room types.");
            foreach (RoomType rt in Enum.GetValues(typeof(RoomType)))
            {
                // 기본적으로 Start/Boss는 후보에 넣지 않는 것이 일반적이지만
                // 여기서는 모든 타입에 기본 가중치 1을 둡니다(후처리에서 제거 가능)
                map[rt] = 1f;
            }
            return map;
        }

        // 안전하게 복사해서 사용
        foreach (var kv in dataManager.RoomDataMap)
        {
            var rt = kv.Key;
            var rd = kv.Value;
            if (rd == null)
            {
                Debug.LogWarning($"[DoorDecider] RoomData for {rt} is null - skipping");
                continue;
            }

            // BaseChance가 0 이상이면 그대로 사용, 음수면 0으로 처리
            map[rt] = Mathf.Max(0f, rd.BaseChance);
        }

        // If map ended up empty for some reason, populate uniform defaults
        if (map.Count == 0)
        {
            foreach (RoomType rt in Enum.GetValues(typeof(RoomType)))
                map[rt] = 1f;
        }

        return map;
    }

    /// <summary>
    /// 후보군에서 현재 컨텍스트에 따라 항목을 제거.
    /// - 이미 등장한 Lab 제거
    /// - 현재 방 타입(예: 현재가 Shop이면 Shop 제거)
    /// - 이미 chosen에 포함된 특수 타입(중복 불가)은 제거
    /// </summary>
    void PruneCandidates(Dictionary<RoomType, float> candidate, RoomType currentRoomType, bool hasLabRoomAppeared, List<RoomType> alreadyChosen)
    {
        if (candidate == null || candidate.Count == 0) return;

        if (hasLabRoomAppeared)
            candidate.Remove(RoomType.Lab);

        // 현재 방 타입은 후보에서 제거(같은 타입 연속 등장 방지)
        if (candidate.ContainsKey(currentRoomType))
            candidate.Remove(currentRoomType);

        // 이미 선택된 특수 타입은 후보에서 제거 (Normal은 중복 허용)
        foreach (var t in alreadyChosen.Where(t => t != RoomType.Normal).ToArray())
            if (candidate.ContainsKey(t))
                candidate.Remove(t);
    }

    /// <summary>
    /// chosen 리스트에 이미 포함된 항목을 기준으로 남은 슬롯을 채움.
    /// - candidate 비어있으면 Normal로 채움.
    /// - candidate의 가중치 합이 0이면 균등 분포로 폴백.
    /// - 특수 타입(=Normal이 아니면)은 선택 즉시 후보에서 제거.
    /// </summary>
    void FillRemainingSlots(List<RoomType> chosen, Dictionary<RoomType, float> candidate, int targetCount)
    {
        if (chosen == null) chosen = new List<RoomType>();
        if (candidate == null) candidate = new Dictionary<RoomType, float>();

        for (int i = chosen.Count; i < targetCount; i++)
        {
            if (candidate.Count == 0)
            {
                chosen.Add(RoomType.Normal);
                Debug.LogWarning("[DoorDecider] candidate empty -> filling with Normal");
                continue;
            }

            var chosenType = ChooseWeightedRandom(candidate);
            chosen.Add(chosenType);

            if (chosenType != RoomType.Normal)
            {
                // 특수 타입은 중복 금지
                candidate.Remove(chosenType);
            }
        }
    }

    /// <summary>
    /// 안전한 가중 랜덤 선택 유틸:
    /// - 유효한 weights를 복사해서 정규화(또는 균등 대체)한 뒤 선택
    /// - Dictionary 순회로 인한 불확정성은 허용(가중 랜덤 본질)
    /// </summary>
    private static T ChooseWeightedRandom<T>(Dictionary<T, float> weights)
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("weights must not be null or empty", nameof(weights));

        // copy and clamp negatives
        var use = new List<KeyValuePair<T, float>>(weights.Count);
        float total = 0f;
        foreach (var kv in weights)
        {
            float w = Mathf.Max(0f, kv.Value);
            use.Add(new KeyValuePair<T, float>(kv.Key, w));
            total += w;
        }

        // all-zero fallback -> uniform distribution
        if (total <= 0f)
        {
            float per = 1f / use.Count;
            for (int i = 0; i < use.Count; i++)
                use[i] = new KeyValuePair<T, float>(use[i].Key, per);
            total = 1f;
        }

        float r = UnityEngine.Random.Range(0f, total);
        float cum = 0f;
        foreach (var kv in use)
        {
            cum += kv.Value;
            if (r <= cum) return kv.Key;
        }

        // rounding fallback
        return use[use.Count - 1].Key;
    }
}