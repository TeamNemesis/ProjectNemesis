using System;
using System.Collections;
using UnityEngine;

public class TechUpgradePackInteractor : RewardInteractableObject
{
    public override event Action OnRewardGiven;

    protected override IEnumerator RewardCoroutine()
    {
        throw new NotImplementedException();
    }
}