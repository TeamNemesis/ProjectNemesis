using System.Collections;
using UnityEngine;

public class SecurityDogEModel : MonsterBase
{
    [Header("Local Stats")]
    [SerializeField] private float jumpPrepareTime = 0.5f;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpCoolTime = 5f;
    [SerializeField] private float currentCoolTime = 0f;

    // Start() 제거!

    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;

        currentCoolTime += Time.deltaTime;

        if (CanSeePlayer())
        {
            LookAtPlayer();
        }

        switch (baseState)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Move:
                HandleMove();
                break;
            case MonsterState.Attack:
                HandleAttack();
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }

    private void HandleIdle()
    {
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= detectionRange && CanSeePlayer())
        {
            baseState = MonsterState.Move;
        }
    }

    private void HandleMove()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > detectionRange || !CanSeePlayer())
        {
            agent.ResetPath();
            baseState = MonsterState.Idle;
            return;
        }

        agent.SetDestination(_target.position);

        if (distance <= attackRange && CanSeePlayer())
        {
            agent.ResetPath();
            baseState = MonsterState.Attack;
        }
    }

    private void HandleAttack()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        // 쿨타임 중이거나 공격 중이면
        if (_isAttacking || currentCoolTime <= jumpCoolTime)
        {
            // 범위 밖으로 나가면 Move로 전환
            if (distance > attackRange || !CanSeePlayer())
            {
                baseState = MonsterState.Move;
            }
            return;
        }

        // 공격 가능한 상태
        if (distance <= attackRange && CanSeePlayer())
        {
            _isAttacking = true;
            StartCoroutine(PerformAttack());
        }
        else
        {
            baseState = MonsterState.Move;
        }
    }

    private IEnumerator PerformAttack()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance < attackRange)
        {
            // NavMeshAgent 정지 및 비활성화
            agent.isStopped = true;
            agent.ResetPath();
            yield return new WaitForSeconds(0.1f); // Agent가 완전히 멈출 시간

            agent.enabled = false;

            // 현재 속도 초기화
            if (monsterRigidbody != null)
            {
                monsterRigidbody.linearVelocity = Vector3.zero;
                monsterRigidbody.angularVelocity = Vector3.zero; // 회전 속도도 초기화
            }

            yield return new WaitForSeconds(jumpPrepareTime);

            // 점프 방향 계산
            Vector3 jumpDirection = (_target.position - transform.position).normalized;
            jumpDirection.y = 0; // 수평으로만

            // Rigidbody가 있는지 확인 후 AddForce
            if (monsterRigidbody != null)
            {
                // 킨마틱 모드 확인 (킨마틱이면 일시적으로 해제)
                bool wasKinematic = monsterRigidbody.isKinematic;
                if (wasKinematic)
                {
                    monsterRigidbody.isKinematic = false;
                }

                monsterRigidbody.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
                Debug.Log($"점프 실행! 힘: {jumpForce}, 방향: {jumpDirection}, Kinematic: {wasKinematic}");

                // 돌진 시간
                yield return new WaitForSeconds(1f);

                // 속도 초기화
                monsterRigidbody.linearVelocity = Vector3.zero;
                monsterRigidbody.angularVelocity = Vector3.zero;

                // 킨마틱 모드 복원
                if (wasKinematic)
                {
                    monsterRigidbody.isKinematic = true;
                }
            }
            else
            {
                Debug.LogError("monsterRigidbody가 null입니다!");
                yield return new WaitForSeconds(1f);
            }

            // NavMeshAgent 재활성화
            yield return new WaitForSeconds(0.1f);

            if (!agent.enabled)
            {
                // NavMesh 위에 있는지 확인
                UnityEngine.AI.NavMeshHit hit;
                if (!agent.isOnNavMesh && UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                }

                agent.enabled = true;

                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
            }

            currentCoolTime = 0f;
        }

        _isAttacking = false;
        baseState = MonsterState.Move;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isAttacking && other.CompareTag(targetTag))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage, transform);
            }
        }
    }
}