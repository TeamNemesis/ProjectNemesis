using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 무기타입이 총일 때의 일반 공격을 담당하는 클래스
/// </summary>
[DisallowMultipleComponent]
public class PlayerRifleNormalAttacker : PlayerNormalAttacker
{
    public override WeaponType WeaponType => WeaponType.Rifle;
    [Header("Rifle Settings")]
    [SerializeField] float fallbackEnd = 0.2f;

    [Header("Bullet")]
    [SerializeField] Transform _firePoint;
    [SerializeField] PoolableObject _bulletPrefab;

    public override event Action OnAttackStarted;

    Coroutine _endRoutine;

    // Initialize로 firePoint, prefab 주입 가능
    public void Initialize(Player player, Transform firePoint, PoolableObject bulletPrefab = null)
    {
        _player = player;
        if (firePoint != null) _firePoint = firePoint;
        if (bulletPrefab != null) _bulletPrefab = bulletPrefab;
    }

    // 이제 Attack()은 애니메이션 트리거를 직접 날리지 않습니다.
    // Player(또는 PlayerAnimator)가 애니메이션을 트리거하고
    // 애니메이션 이벤트에서 FireNow()와 OnAnimationAttackEnd()를 호출합니다.
    public override void Attack()
    {
        _isAttacking = true;
        OnAttackStarted?.Invoke();
        _player.Animator.OnNormalAttack();

        if (_endRoutine != null)
        {
            // StopCoroutine을 호출할 때는 코루틴을 시작한 객체에서 멈춰야 함.
            _player.StopCoroutine(_endRoutine);
            _endRoutine = null;
        }
        // 코루틴은 항상 활성화가 보장되는 _owner에서 시작
        _endRoutine = _player.StartCoroutine(EndAfterDelayCoroutine());
    }

    IEnumerator EndAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(fallbackEnd);
        EndNow();
    }

    // 애니메이션 이벤트에서 호출: 실제 발사 실행
    public void FireNow()
    {
        if (_bulletPrefab == null || _firePoint == null)
        {
            Debug.LogWarning("PlayerRifleNormalAttacker.FireNow: firePoint or bulletPrefab is null.");
        }

        GameObject obj = GameManager.Instance.PoolManager.GetFromPool(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        var bullet = obj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetMoveDir(_firePoint.forward);
        }
    }

    // 애니메이션 이벤트에서 호출: 애니메이션이 끝나면 호출
    public void OnAnimationAttackEnd()
    {
        EndNow();
    }

    public override void EndAttack()
    {
        EndNow();
    }

    void EndNow()
    {
        if (_endRoutine != null)
        {
            _player.StopCoroutine(_endRoutine); _endRoutine = null;
        }
        base.EndAttack();
    }
}