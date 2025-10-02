using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BallSecurityRobot : MonsterBase
{
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Move,   // 플레이어를 추격 중일 때
        Attack, // 자폭 시도
        Die     // 파괴됨
    }

    [Header("Stats")]
    public float detectionRange = 10f;      // 플레이어 감지 범위
    public float explosionRadius = 3f;      // 실제 폭발 범위

    // attackDamage, attackRange, attackDelay, isDead 등은 MonsterBase에서 상속됨

    [Header("Effects")]
    public GameObject circlePrefab;     // AttackDecalEffect 프리팹 (Inspector에서 지정)

    private bool isFusing = false;
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

            case State.Move:
                HandleMove();
                break;

            case State.Attack:
                if (!isFusing)
                    StartCoroutine(SelfDestructionAttack());
                break;

            case State.Die:
                Die();
                break;
        }
    }

    private void HandleIdle()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            currentState = State.Move;
        }
    }

    private void HandleMove()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            agent.ResetPath();
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(player.position);

        if (distance <= attackRange && !isFusing)
        {
            currentState = State.Attack;
        }
    }

    private IEnumerator SelfDestructionAttack()
    {
        if (player != null && !isFusing)
        {
            isFusing = true;

            // 자폭 카운트다운 원 생성 (로봇의 자식으로 붙임)
            if (circlePrefab != null)
            {
                GameObject circle = Instantiate(circlePrefab, transform.position, circlePrefab.transform.rotation, transform);
                AttackDecalEffect effect = circle.GetComponent<AttackDecalEffect>();
                if (effect != null)
                {
                    effect.Play(attackDelay, explosionRadius);
                }
            }

            // 자폭까지 딜레이
            yield return new WaitForSeconds(attackDelay);

            // 폭발 판정
            float finalPlayerDistance = Vector3.Distance(transform.position, player.position);
            if (finalPlayerDistance <= explosionRadius)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }

            currentState = State.Die;
        }
    }

    public override void Die()
    {
        // 필요 시 폭발 이펙트, 사운드 추가 가능
        base.Die();
    }
}
