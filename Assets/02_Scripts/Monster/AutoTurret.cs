using System.Collections;
using UnityEngine;

public class AutoTurret : MonsterBase
{
    [SerializeField]
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Attack, // 공격
        Die     // 죽음
    }
    [Header("Stats")]
    [SerializeField] private bool _isAttacking = false;

    [Header("TurretBulletPrefab"), SerializeField]
    private GameObject turretBulletPrefab; // 터렛 총알 프리펩

    [SerializeField]
    private State currentState = State.Idle;

    private void Update()
    {
        if (isDead || player == null) return;
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
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange && CanSeePlayer())
        {
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {

            GameObject bullet = Instantiate(turretBulletPrefab, transform.position + transform.forward, transform.rotation);
            TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();
            if (turretBullet != null)
            {
                turretBullet.SetDamage(attackDamage);
            }
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Idle; // 공격 후 다시 대기 상태로 전환
    }
}
