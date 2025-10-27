using System;
using System.Collections;
using UnityEngine;

public abstract class RewardInteractableObject : InteractableObject
{
    Coroutine _rewardCoroutine;
    protected Transform _player;

    public override InteractableType InteractableType => InteractableType.Reward;

    public abstract event Action OnRewardGiven;
    public override event Action<IInteractable> OnInteracted;

    public override void StartInteract(Transform subject)
    {
        _player = subject;
        _rewardCoroutine = StartCoroutine(RewardCoroutine());
        OnInteracted?.Invoke(this);
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