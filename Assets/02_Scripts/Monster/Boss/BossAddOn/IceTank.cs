using UnityEngine;

public class IceTank : MonsterBase
{
    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;
        LookAtPlayer();

        switch (baseState)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }

    private void HandleIdle()
    {

    }

    public void SetStateDie()
    {
        baseState = MonsterState.Die;
    }
}
