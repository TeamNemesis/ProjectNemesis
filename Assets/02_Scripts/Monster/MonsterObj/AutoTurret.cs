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
    [Header("Local Stats")]
    [SerializeField] private bool _isAttacking = false;

    [Header("TurretBulletPrefab"), SerializeField]
    private PoolableObject turretbullet; // 터렛 총알 프리펩

    [SerializeField]
    private State currentState = State.Idle;

    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;

        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Attack:
                LookAtPlayer();
                if (!_isAttacking)
                {
                    StartCoroutine(PerformAttack());
                }
                break;
            case State.Die:
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
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;

        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            GameObject bullet = ObjectPool.Instance.GetFromPool(turretbullet, transform.position);
            bullet.transform.rotation = transform.rotation;
            TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();
            if (turretBullet != null)
            {
                turretBullet.Initialize(targetTag, attackDamage, bulletLifeTime);
            }
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Idle; // 공격 후 다시 대기 상태로 전환
    }
}
