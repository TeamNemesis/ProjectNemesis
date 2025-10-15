using UnityEngine;

/// <summary>
/// 플레이어의 애니메이션을 담당하는 추상 클래스
/// 무기타입에 따라 상속받아 구현
/// </summary>
public abstract class PlayerAnimator : MonoBehaviour
{
    [SerializeField] protected Animator _animator; //플레이어 애니메이터 컴포넌트

    public virtual void OnMove(float speed)
    {
        _animator.SetFloat(Constants.ANIPARAM_MOVESPEED, speed);
    }

    /// <summary>
    /// 플레이어의 일반공격 입력을 받아 공격 애니메이션을 실행하는 함수
    /// </summary>
    public virtual void OnNormalAttack()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONNORMALATTACK);
    }
}
