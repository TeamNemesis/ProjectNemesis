using System;
using System.Collections;

public class HealPackInteractor : RewardInteractableObject
{
    public override event Action OnRewardGiven;

    protected override IEnumerator RewardCoroutine()
    {
        throw new NotImplementedException();
    }
}