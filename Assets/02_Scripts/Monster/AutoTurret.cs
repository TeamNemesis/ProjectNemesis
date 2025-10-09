using System.Collections;
using UnityEngine;

public class AutoTurret : MonsterBase, IDamageAble
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

    /// <summary>
    /// 플레이어를 바라보게 하는 함수
    /// </summary>
    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void HandleIdle()
    {
        // 플레이어와 거리
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            yield return new WaitForSeconds(attackDelay);

            GameObject bullet = Instantiate(turretBulletPrefab, transform.position + transform.forward, transform.rotation);
            TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();
            if (turretBullet != null)
            {
                turretBullet.SetDamage(attackDamage);
            }
        }
        _isAttacking = false;
        currentState = State.Idle; // 공격 후 다시 대기 상태로 전환
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= (int)damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
