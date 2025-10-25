using System;
using System.Collections;
using UnityEngine;

public class TechSelectPackInteractor : RewardInteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] TechItem _techItem;

    [Header("----- 읽기 전용 -----")]
    [SerializeField] TechSelectPackType _packType;

    public TechSelectPackType PackType => _packType;
    public override InteractableType InteractableType => InteractableType.Reward;
    public override Vector3 GuidePoint => _guidePoint.position;

    public override event Action<IInteractable> OnInteracted;
    public override event Action OnRewardGiven;

    public void Initialize(TechSelectPackType packType)
    {
        _packType = packType;
        GameManager.Instance.UIManager.onRewardSelect += RaiseRewardGivenEvent;
    }

    public override void StartInteract(Transform subject)
    {
        _techItem.GetSkill(_packType);
        OnInteracted?.Invoke(this);
    }

    protected override IEnumerator RewardCoroutine()
    {
        throw new NotImplementedException();
    }

    void RaiseRewardGivenEvent()
    {
        OnRewardGiven?.Invoke();
    }
}
