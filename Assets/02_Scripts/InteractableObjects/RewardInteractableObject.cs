using System;
using System.Collections;
using UnityEngine;

public abstract class RewardInteractableObject : InteractableObject
{
    Coroutine _rewardCoroutine;

    public override InteractableType InteractableType => InteractableType.Reward;

    public override Vector3 GuidePoint => throw new NotImplementedException();

    public override event Action<IInteractable> OnInteracted;
    public abstract event Action OnRewardGiven;

    public override void StartInteract(Transform subject)
    {
        _rewardCoroutine = StartCoroutine(RewardCoroutine());
    }

    protected abstract IEnumerator RewardCoroutine();

    private void OnDisable()
    {
        if (_rewardCoroutine != null)
        {
            StopCoroutine(_rewardCoroutine);
            _rewardCoroutine = null;
        }
    }
}