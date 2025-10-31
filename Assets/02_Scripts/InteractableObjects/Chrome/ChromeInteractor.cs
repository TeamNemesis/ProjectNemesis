using System;
using System.Collections;
using UnityEngine;

public class ChromeInteractor : RewardInteractableObject
{
    public override event Action OnRewardGiven;

    protected override IEnumerator RewardCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        GameManager.Instance.CurrencyManager.AddChrome(10);
        OnRewardGiven?.Invoke();
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }
}