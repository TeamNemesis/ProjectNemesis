using System;
using System.Collections;
using UnityEngine;

public abstract class RewardInteractableObject : InteractableObject
{
    Coroutine _rewardCoroutine;

    public override InteractableType InteractableType => InteractableType.Reward;

    public override Vector3 GuidePoint => throw new NotImplementedException();

    public override event Action<IInteractable> OnInteracted;
    public event Action OnRewardGiven;

    public override void StartInteract(Transform subject)
    {
        _rewardCoroutine = StartCoroutine(RewardCoroutine());
    }

    protected virtual IEnumerator RewardCoroutine()
    {
        // 보상 지급 로직 구현
        yield return null;
    }

    private void OnDisable()
    {
        if (_rewardCoroutine != null)
        {
            StopCoroutine(_rewardCoroutine);
            _rewardCoroutine = null;
        }
    }
}