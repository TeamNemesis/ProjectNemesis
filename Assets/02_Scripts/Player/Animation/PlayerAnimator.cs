using UnityEngine;

/// <summary>
/// 플레이어의 애니메이션을 담당하는클래스
/// 무기타입에 따라 _animator의 RuntimeAnimatorController를 변경해준다.
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] protected Animator _animator; //플레이어 애니메이터 컴포넌트

    public Animator Animator => _animator;

    public void SetAnimator(RuntimeAnimatorController runtimeAnimatorController)
    {
        _animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    public void OnMove(float speed)
    {
        _animator.SetFloat(Constants.ANIPARAM_MOVESPEED, speed);
    }

    public void OnDash()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONDASH);
    }

    /// <summary>
    /// 플레이어의 일반공격 입력을 받아 공격 애니메이션을 실행하는 함수
    /// </summary>
    public void OnNormalAttack()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONNORMALATTACK);
    }

    public void OnSpecialAttack()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONSPECIALATTACK);
    }

    public void OnSpecialAttackEnd()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONSPECIALATTACKEND);
    }
}
