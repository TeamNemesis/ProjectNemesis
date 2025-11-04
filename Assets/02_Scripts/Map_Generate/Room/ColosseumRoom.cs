using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColosseumRoom : Room
{
    // NormalRoomType -> Pool/Resource 키 매핑 (프로젝트 규칙에 맞춰 값 설정)
    Dictionary<NormalRoomType, string> _normalRewardMap = new Dictionary<NormalRoomType, string>()
    {
        {NormalRoomType.Heal, Constants.RESOURCES_PATH_REWARDS + "/HealPack" },
        {NormalRoomType.Credit, Constants.RESOURCES_PATH_REWARDS + "/Credit" },
        {NormalRoomType.TechUpgrade, Constants.RESOURCES_PATH_REWARDS + "/TechUpgradePack" },
        {NormalRoomType.Chrome, Constants.RESOURCES_PATH_REWARDS + "/Chrome" },
        {NormalRoomType.TechSelect, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack" },
    };
    Dictionary<TechSelectPackType, string> _techSelectPackMap = new Dictionary<TechSelectPackType, string>()
    {
        {TechSelectPackType.Company1, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company1" },
        {TechSelectPackType.Company2, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company2" },
        {TechSelectPackType.Company3, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company3" },
        {TechSelectPackType.Company4, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company4" },
        {TechSelectPackType.Company5, Constants.RESOURCES_PATH_REWARDS + "/TechSelectPack_Company5" },
    };

    // 보상 선택 완료를 판단하기 위한 상태
    // - subscribedRewardables: 실제 이벤트를 구독한 RewardInteractableObject 목록 (언제든 언구독 가능하게 보관)
    // - remainingRewardCount: 플레이어가 획득해야 남은 보상 수 (초기값은 구독한 rewardable 수)
    private readonly List<RewardInteractableObject> _subscribedRewardables = new List<RewardInteractableObject>();
    private int _remainingRewardCount = 0;

    /// <summary>
    /// 일반 보상 맵에서 3개를 무작위로 선택하여 스폰
    /// 단 기술팩은 반드시 하나를 포함해야하고 
    /// </summary>
    /// <returns></returns>
    public override IInteractable[] SpawnReward()
    {
        // 보상 스폰 포인트 유효성 검사
        if (_rewardSpawnPoints == null || _rewardSpawnPoints.Length < 3)
        {
            Debug.LogWarning($"ColosseumRoom.SpawnReward: reward spawn points are not properly set. (room={name})");
            return System.Array.Empty<IInteractable>();
        }

        // 반환할 IInteractable 리스트 생성
        List<IInteractable> results = new List<IInteractable>();

        // 1. 첫번째 보상은 반드시 기술팩으로 설정해서 스폰
        TechSelectPackType techPackType = GameManager.Instance.skillManager.GetSkillPackTypes(1)[0];
        GameObject techPackPrefab = GameManager.Instance.PoolManager.GetFromPool(_techSelectPackMap[techPackType], _rewardSpawnPoints[0].position, Quaternion.identity);
        // 반환할 리스트에 추가
        results.Add(techPackPrefab.GetComponent<IInteractable>());

        // 2. 두번째 보상은 일반 보상 맵에서 무작위로 선택
        List<NormalRoomType> normalRoomTypes = new List<NormalRoomType>(_normalRewardMap.Keys);
        NormalRoomType randomNormalType = normalRoomTypes[Random.Range(0, normalRoomTypes.Count)];
        GameObject normalRewardPrefab = GameManager.Instance.PoolManager.GetFromPool(_normalRewardMap[randomNormalType], _rewardSpawnPoints[1].position, Quaternion.identity);
        // 반환할 리스트에 추가
        results.Add(normalRewardPrefab.GetComponent<IInteractable>());
        // 만약 두번째 보상이 기술팩이었다면 normalRoomTypes에서 제거 후 세번째 보상 선택
        if (randomNormalType == NormalRoomType.TechSelect)
        {
            normalRoomTypes.Remove(NormalRoomType.TechSelect);
        }

        // 3. 세번째 보상은 일반 보상 맵에서 무작위로 선택(남은 것 중에서)
        NormalRoomType randomNormalType2 = normalRoomTypes[Random.Range(0, normalRoomTypes.Count)];
        GameObject normalRewardPrefab2 = GameManager.Instance.PoolManager.GetFromPool(_normalRewardMap[randomNormalType2], _rewardSpawnPoints[2].position, Quaternion.identity);
        // 반환할 리스트에 추가
        results.Add(normalRewardPrefab2.GetComponent<IInteractable>());

        // 여기서 문제는 results들을 rewardableObject로 캐스팅해서 보상선택이 완료되었다는 이벤트를 RewardSelectionFinished에 연결해줘야 하는데
        foreach (var interactable in results)
        {
            if (interactable is RewardInteractableObject rewardableObject)
            {
                rewardableObject.OnRewardGiven += RewardSelectionFinished;
            }
        }
        // 이렇게하면 하나라도 보상을 획득한경우 RewardSelectionFinished가 호출된다.
        // 그러면 보상선택 3개가 완료되지 않아도 보상선택이 완료되었다고 처리되는 문제가 있다.
        // 이를 해결하려면 RewardInteractableObject에서 보상선택이 완료될때마다 카운트를 올리고
        // 3개가 모두 완료되었을때 RewardSelectionFinished를 호출하도록 해야한다.

        // --- 여기부터: RewardInteractableObject 이벤트 구독 및 카운팅 로직 ---
        // 기존에 남아있던 구독은 안전하게 해제
        UnsubscribeAllRewardables();

        _subscribedRewardables.Clear();
        _remainingRewardCount = 0;

        // 판단 로직:
        // - 결과 리스트 중 RewardInteractableObject 인스턴스만 "획득 대기 대상으로" 간주하고 카운팅에 포함
        // - 만약 rewardable 객체가 0개라면(드물지만) 즉시 RewardSelectionFinished 호출
        foreach (var interactable in results)
        {
            if (interactable is RewardInteractableObject rewardableObject)
            {
                // 구독
                rewardableObject.OnRewardGiven += OnSingleRewardGiven;
                _subscribedRewardables.Add(rewardableObject);
            }
            else
            {
                // IInteractable 이지만 RewardInteractableObject가 아닌 경우: 디자인상 즉시 완료로 간주하거나 별도 동작 필요
                // 여기서는 보상 획득 대상으로 간주하지 않아 remaining count에는 포함하지 않음.
            }
        }

        // remainingRewardCount는 구독된 rewardables 수
        _remainingRewardCount = _subscribedRewardables.Count;

        // 만약 획득 대상(구독된 rewardables)이 없다면 바로 완료 처리
        if (_remainingRewardCount == 0)
        {
            RewardSelectionFinished();
        }

        return results.ToArray();
    }

    private void OnSingleRewardGiven()
    {
        // 안전하게 남은 수 감소 및 언구독
        _remainingRewardCount = Mathf.Max(0, _remainingRewardCount - 1);

        // 호출한 rewardable을 찾아 언구독 - RewardInteractableObject에서 호출 시 자체 참조 전달이 없으면
        // 우리는 모든 구독 객체들 중에서 OnRewardGiven 이벤트가 발생한 것을 알 수 없으므로
        // RewardInteractableObject의 OnRewardGiven이 호출될 때 해당 객체가 스스로 EventBus나 Room에
        // 인스턴스 참조를 넘길 수 있다면 더 정확하지만, 여기서는 단순히 카운트 기반으로 처리.
        // (만약 이벤트가 인스턴스 참조를 전달하도록 변경 가능한 경우 OnSingleRewardGiven(RewardInteractableObject r) 형태로 바꾸는 것을 권장)

        // 이미 소비된 보상에 대해 중복 카운팅되지 않도록 모든 구독 객체와의 언구독을 시도.
        // (좀 더 정교한 처리를 원하면 RewardInteractableObject 측에서 자신의 OnRewardGiven에서
        // Room의 Unregister 방식으로 본인을 전달하도록 바꾸세요.)
        // 여기선 간단하게 모든 subscribedRewardables를 확인해 인터널 상태가 'given'이면 언구독 제거 가능.
        // (예: rewardableObject.IsGiven 플래그가 있다면 이를 검사해서 언구독할 수 있음)

        // 완료 체크
        if (_remainingRewardCount <= 0)
        {
            // 모든 보상 획득 완료
            RewardSelectionFinished();
            // 구독 전부 해제(안전)
            UnsubscribeAllRewardables();
        }
    }

    // 모든 구독을 안전하게 해제
    private void UnsubscribeAllRewardables()
    {
        for (int i = _subscribedRewardables.Count - 1; i >= 0; i--)
        {
            var r = _subscribedRewardables[i];
            if (r != null)
            {
                r.OnRewardGiven -= OnSingleRewardGiven;
            }
        }
        _subscribedRewardables.Clear();
    }

    // 방(또는 오브젝트)이 비활성화될 때 구독 정리 및 콜로세움 플래그 해제
    private void OnDisable()
    {
        UnsubscribeAllRewardables();
        EventBus.SetColosseumRoom(false);
    }
}

[CustomEditor(typeof(ColosseumRoom))]
public class ColosseumRoomDebugger : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ColosseumRoom colosseumRoom = (ColosseumRoom)target;
        if (GUILayout.Button("Spawn Rewards"))
        {
            colosseumRoom.SpawnReward();
        }
    }
}