using System;
using System.Collections;
using UnityEngine;

public class TechSelectPackInteractor : RewardInteractableObject
{
    [SerializeField] TechItem _techItem;

    [Header("----- 설정값 -----")]
    [SerializeField] TechSelectPackType _packType;

    public TechSelectPackType PackType => _packType;

    public override event Action OnRewardGiven;

    public override void Initialize()
    {
        GameManager.Instance.UIManager.onRewardSelect += RaiseRewardGivenEvent;
    }

    protected override IEnumerator RewardCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // 보상 선택 UI 열리는 시간 대기
        _techItem.GetSkill(_packType);
    }

    void RaiseRewardGivenEvent()
    {
        OnRewardGiven?.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.UIManager.onRewardSelect -= RaiseRewardGivenEvent;
    }

    public override void GetInteractionMessage(out string title, out string instruction)
    {
        title = _packType.ToString();
        instruction = _rewardInstruction;
    }
}
