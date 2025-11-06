using System;
using System.Linq;
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
        //Debug.LogWarning("PlayerAnimator: OnNormalAttack triggered.");
    }

    public void OnSpecialAttack()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONSPECIALATTACK);
    }

    public void OnSpecialAttackEnd()
    {
        _animator.SetTrigger(Constants.ANIPARAM_ONSPECIALATTACKEND);
    }

    /// <summary>
    /// runtimeAnimatorController (또는 AnimatorOverrideController)를 검색하여 clipName에 해당하는 AnimationClip.length를 반환합니다.
    /// 찾지 못하면 0을 반환합니다.
    /// </summary>
    public float GetClipLengthByName(string clipName)
    {
        if (_animator == null || _animator.runtimeAnimatorController == null || string.IsNullOrEmpty(clipName))
            return 0f;

        // 일반 컨트롤러의 모든 클립 탐색
        var clips = _animator.runtimeAnimatorController.animationClips;
        if (clips != null && clips.Length > 0)
        {
            var c = clips.FirstOrDefault(x => x != null && x.name.Equals(clipName, StringComparison.OrdinalIgnoreCase));
            if (c != null) return c.length;
        }

        // 못 찾음
        return 0f;
    }
}
