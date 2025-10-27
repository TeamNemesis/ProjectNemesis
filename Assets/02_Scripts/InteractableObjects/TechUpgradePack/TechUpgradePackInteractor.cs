using System;
using System.Collections;
using UnityEngine;

public class TechUpgradePackInteractor : RewardInteractableObject
{
    [SerializeField] TechItem _techItem;

    public override event Action OnRewardGiven;

    public override void StartInteract(Transform subject)
    {
        base.StartInteract(subject);
        GameManager.Instance.UIManager.onRewardSelect += RaiseRewardGivenEvent;
    }

    protected override IEnumerator RewardCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // 보상 선택 UI 열리는 시간 대기
        _techItem.SkillUpgrade();
    }

    void RaiseRewardGivenEvent()
    {
        OnRewardGiven?.Invoke();
    }
}