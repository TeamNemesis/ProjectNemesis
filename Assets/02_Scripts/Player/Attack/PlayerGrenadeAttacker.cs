using System;
using UnityEngine;

/// <summary>
/// 플레이어의 유탄공격을 담당하는 클래스
/// 무기타입에 상관없이 공통으로 사용
/// </summary>
public class PlayerGrenadeAttacker : MonoBehaviour, IAttacker
{
    public WeaponType WeaponType => throw new NotImplementedException();

    public bool IsAttacking => throw new NotImplementedException();

    public event Action AttackStarted;
    public event Action AttackEnded;

    public void EndAttack()
    {
        throw new NotImplementedException();
    }

    public void GrenadeAttack()
    {
        Debug.Log("유탄 공격 실행");
    }

    public void OnAnimationEnd()
    {
        throw new NotImplementedException();
    }

    public void OnAnimationFire()
    {
        throw new NotImplementedException();
    }

    public bool RequestAttack()
    {

        GrenadeAttack();
        return true;
        throw new NotImplementedException();
    }
}