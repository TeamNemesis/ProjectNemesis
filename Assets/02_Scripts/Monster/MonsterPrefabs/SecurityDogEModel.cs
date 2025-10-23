using System.Collections;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class SecurityDogEModel : MonsterBase
{
    [SerializeField]
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Move,   // 플레이어를 추격 중일 때
        Attack, // 공격
        Die     // 죽음
    }
    [Header("Local Stats")]
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private float jumpPrepareTime = 0.5f; // 점프 준비 시간
    [SerializeField] private float jumpSpeed = 15f; // 점프 속도
    [SerializeField] private float jumpDuration = 0.8f; // 점프 지속 시간
    [SerializeField] private float jumpCoolTime = 5;
    [SerializeField] private float currentCoolTime = 0;
    [SerializeField] private Vector3 jumpDirection;
    [SerializeField] private float jumpTimer;
    

    [SerializeField]
    private State currentState = State.Idle;

    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;
        currentCoolTime += Time.deltaTime;

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
                if (!_isAttacking && currentCoolTime > jumpCoolTime)
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
        if (distance <= detectionRange && CanSeePlayer())
        {
            currentState = State.Move;
        }
    }
    private void HandleMove()
    {
        if (_target == null) return;
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance > detectionRange || !CanSeePlayer())
        {
            agent.ResetPath();
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(_target.position);

        if (distance <= attackRange && CanSeePlayer())
        {
            agent.ResetPath();
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance < attackRange)
        {
            //점프 준비
            agent.isStopped = true;
            yield return new WaitForSeconds(jumpPrepareTime);

            jumpDirection = (_target.position - transform.position).normalized;

            jumpTimer = 0f;

            agent.enabled = false;

            while (jumpTimer < jumpDuration && _isAttacking&& !isStunned && !isBindned)
            {
                jumpTimer += Time.deltaTime;
                transform.position += jumpDirection * jumpSpeed * Time.deltaTime;
                yield return null;
            }

            currentCoolTime = 0f;
            agent.enabled = true;
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
        _isAttacking = false;
        currentState = State.Move;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isAttacking && other.CompareTag(targetTag))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

}
