using System;
using System.Collections;
using UnityEngine;

public class HealPackInteractor : RewardInteractableObject
{
    public override event Action OnRewardGiven;

    protected override IEnumerator RewardCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        // 이 부분에서 PlayerModel을 직접찾기보다 인터페이스를 통해
        // Heal이 가능한 대상인지 확인하는 것이 더 좋을 수 있음
        _player.gameObject.GetComponent<PlayerModel>().Heal(50);
        OnRewardGiven?.Invoke();
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }
}