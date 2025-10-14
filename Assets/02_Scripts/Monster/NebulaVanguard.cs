using System.Collections;
using UnityEngine;

public class NebulaVanguard : MonsterBase
{
    [SerializeField]
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Move,   // 플레이어를 추격 중일 때
        Attack, // 공격
        Die     // 죽음
    }
    [Header("Stats")]
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private float a;

    [SerializeField]
    private State currentState = State.Idle;

    private void Update()
    {
        if (isDead || player == null) return;
        if (isStunned) return;

        if (CanSeePlayer())
        {
            LookAtPlayer();
        }

        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Move:
                HandleMove();
                break;
            case State.Attack:
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
        if (distance <= detectionRange && CanSeePlayer())
        {
            currentState = State.Move;
        }
    }
    private void HandleMove()
    {
        if (player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange || !CanSeePlayer())
        {
            agent.ResetPath();
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(player.position);

        if (distance <= attackRange && CanSeePlayer())
        {
            agent.ResetPath();
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // 공격 로직
            yield return null;
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }
}
