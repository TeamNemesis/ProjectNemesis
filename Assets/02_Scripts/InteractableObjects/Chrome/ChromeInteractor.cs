using System;
using System.Collections;

public class ChromeInteractor : RewardInteractableObject
{
    public override event Action OnRewardGiven;

    protected override IEnumerator RewardCoroutine()
    {
        throw new NotImplementedException();
    }
}