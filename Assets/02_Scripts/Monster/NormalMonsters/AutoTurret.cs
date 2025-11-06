using System.Collections;
using UnityEngine;
public class AutoTurret : MonsterBase
{
    [SerializeField] float bulletLifeTime = 8f;
    [Header("TurretBulletPrefab"), SerializeField]
    private PoolableObject turretbullet; // 터렛 총알 프리펩
    [SerializeField] private Transform shootPos;
    [SerializeField] private PoolableObject muzzleFlashPrefab;

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
        monsterAnimator.SetTrigger("Attack");
        GetEffectFromPool(muzzleFlashPrefab, shootPos.position, shootPos.rotation);
        _isAttacking = true;
        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            GameObject bullet = GameManager.Instance.PoolManager.GetFromPool(turretbullet, transform.position, transform.rotation);
            TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();
            if (turretBullet != null)
            {
                // 혼란 상태인지 확인 (targetTag가 Monster면 혼란 상태)
                bool isConfused = (targetTag == Constants.TAG_MONSTER);
                turretBullet.Initialize(targetTag, attackDamage, bulletLifeTime, gameObject, isConfused);
            }
            yield return new WaitForSeconds(attackDelay);
        }

        _isAttacking = false;
        baseState = MonsterState.Idle; // 공격 후 다시 대기 상태로 전환
    }
}