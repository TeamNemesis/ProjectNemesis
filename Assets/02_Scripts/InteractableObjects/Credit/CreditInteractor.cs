using System;
using System.Collections;
using UnityEngine;

public class CreditInteractor : RewardInteractableObject
{
    public override event Action OnRewardGiven;

    protected override IEnumerator RewardCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        GameManager.Instance.CurrencyManager.AddCredit(500);
        OnRewardGiven?.Invoke();
    }
}