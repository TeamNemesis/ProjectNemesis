using System.Collections;
using UnityEngine;

public class AutoTurret : MonsterBase
{

    [SerializeField] float bulletLifeTime = 8f;

    [SerializeField]
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Attack, // 공격
        Die     // 죽음
    }

    [Header("TurretBulletPrefab"), SerializeField]
    private PoolableObject turretbullet; // 터렛 총알 프리펩

    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;

        switch (baseState)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Attack:
                LookAtPlayer();
                if (!_isAttacking)
                {
                    StartCoroutine(PerformAttack());
                }
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }

    private void HandleIdle()
    {
        // 플레이어와 거리
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= attackRange && CanSeePlayer())
        {
            baseState = MonsterState.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;

        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            GameObject bullet = GameManager.Instance.PoolManager.GetFromPool(turretbullet, transform.position, transform.rotation);
            TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();
            if (turretBullet != null)
            {
                turretBullet.Initialize(targetTag, attackDamage, bulletLifeTime , gameObject);
            }
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        baseState = MonsterState.Idle; // 공격 후 다시 대기 상태로 전환
    }
}
