using UnityEngine;

public class EnemyAnimator : CharacterAnimatorHandler
{
    [Header("Component References")]
    [SerializeField] Animator _animator;

    public void PlayWelcome()
    {
        _animator.Play("First1");
    }

    public void PlayRun(float moveSpeed)
    {
        _animator.SetFloat("MoveSpeed", moveSpeed);
    }

    public void PlayAttack()
    {
        _animator.SetTrigger("OnAttack");
    }
}