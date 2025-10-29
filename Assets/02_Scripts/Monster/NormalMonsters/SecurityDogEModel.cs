using System.Collections;
using UnityEngine;

public class SecurityDogEModel : MonsterBase
{
    [Header("Local Stats")]
    [SerializeField] private float jumpPrepareTime = 0.5f;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpCoolTime = 5f;
    [SerializeField] private float currentCoolTime = 0f;

    private float fixedYPosition;
    private Quaternion fixedRotation;

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
            // Y축 위치와 회전값 저장
            fixedYPosition = transform.position.y;
            fixedRotation = transform.rotation;

            // NavMeshAgent 정지 및 비활성화
            agent.isStopped = true;
            agent.ResetPath();
            yield return new WaitForSeconds(0.1f); // Agent가 완전히 멈출 시간

            agent.enabled = false;

            // 현재 속도 초기화
            if (monsterRigidbody != null)
            {
                if (!monsterRigidbody.isKinematic)
                {
                    monsterRigidbody.linearVelocity = Vector3.zero;
                    monsterRigidbody.angularVelocity = Vector3.zero;
                }

                // Rigidbody 제약 설정 (Y축 위치와 회전 고정)
                monsterRigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                                               RigidbodyConstraints.FreezeRotation;
            }

            yield return new WaitForSeconds(jumpPrepareTime);

            // 점프 방향 계산
            Vector3 jumpDirection = (_target.position - transform.position).normalized;
            jumpDirection.y = 0; // 수평으로만

            // Rigidbody가 있는지 확인 후 AddForce
            if (monsterRigidbody != null)
            {
                bool wasKinematic = monsterRigidbody.isKinematic;
                if (wasKinematic)
                {
                    monsterRigidbody.isKinematic = false;
                }

                monsterRigidbody.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

                // 돌진 시간
                yield return new WaitForSeconds(1f);

                // 속도 초기화
                monsterRigidbody.linearVelocity = Vector3.zero;
                monsterRigidbody.angularVelocity = Vector3.zero;

                // Y축 위치와 회전값 복원
                Vector3 pos = transform.position;
                pos.y = fixedYPosition;
                transform.position = pos;
                transform.rotation = fixedRotation;

                // 킨마틱 모드 복원
                if (wasKinematic)
                {
                    monsterRigidbody.isKinematic = true;
                }

                // Rigidbody 제약 해제
                monsterRigidbody.constraints = RigidbodyConstraints.None;
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

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        // 공격 중일 때만 충돌 처리
        if (_isAttacking)
        {
            // Y축 위치와 회전값 유지
            Vector3 pos = transform.position;
            pos.y = fixedYPosition;
            transform.position = pos;
            transform.rotation = fixedRotation;

            // 타겟이 맞는지 확인
            if (collision.gameObject.CompareTag(targetTag))
            {
                if (monsterRigidbody != null)
                {
                    monsterRigidbody.linearVelocity = Vector3.zero;
                    monsterRigidbody.angularVelocity = Vector3.zero;
                }

                var playerHealth = collision.gameObject.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage, transform);
                }
            }
            else
            {
                // 타겟이 아닌 대상과 충돌 시 해당 대상의 충돌을 무시
                Collider otherCollider = collision.collider;
                Collider myCollider = GetComponent<Collider>();
                if (otherCollider != null && myCollider != null)
                {
                    Physics.IgnoreCollision(myCollider, otherCollider, true);

                    // 공격이 끝나면 충돌 무시 해제 (코루틴으로 처리)
                    StartCoroutine(ResetCollisionIgnore(myCollider, otherCollider));
                }
            }
        }
    }

    private IEnumerator ResetCollisionIgnore(Collider myCollider, Collider otherCollider)
    {
        // 공격이 끝날 때까지 대기
        yield return new WaitUntil(() => !_isAttacking);

        // 충돌 무시 해제
        if (myCollider != null && otherCollider != null)
        {
            Physics.IgnoreCollision(myCollider, otherCollider, false);
        }
    }
}