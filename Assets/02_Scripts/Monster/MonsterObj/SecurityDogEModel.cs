using System.Collections;
using UnityEngine;

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
    [SerializeField] private float _jumpRange = 10f;
    [SerializeField] private float _box_Length = 1;
    [SerializeField] private float _box_Height = 1;
    [SerializeField] private float _box_Width = 1;

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

        if (distance <= _jumpRange && CanSeePlayer())
        {
            agent.ResetPath();
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        Debug.Log("코루틴 시작");
        _isAttacking = true;
        float distance = Vector3.Distance(transform.position, player.position);

        // 점프 준비 거리
        if (distance < _jumpRange)
        {
            Debug.Log("점프 시작");
            // 잠시 멈춤 (돌진 준비 동작)
            yield return new WaitForSeconds(attackDelay);

            // 돌진 시작
            agent.speed = originalSpeed * 3;

            // 돌진하면서 플레이어에게 접근
            float dashTime = 0f;
            float maxDashTime = 3f; // 최대 돌진 시간

            while (dashTime < maxDashTime)
            {
                distance = Vector3.Distance(transform.position, player.position);

                // 공격 범위 내에 들어오면 공격 판정
                if (distance <= attackRange)
                {
                    Vector3 center = transform.position + transform.forward * (_box_Length / 2f);
                    Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);
                    Quaternion orientation = Quaternion.LookRotation(transform.forward);
                    Collider[] hitTarget = Physics.OverlapBox(center, halfExtents, orientation);

                    foreach (Collider target in hitTarget)
                    {
                        if (target.CompareTag(targetTag) && target.TryGetComponent(out IDamageable playerHealth))
                        {
                            playerHealth.TakeDamage(attackDamage);
                        }
                    }

                    // 공격 후 잠시 대기
                    yield return new WaitForSeconds(0.5f);
                    break;
                }

                dashTime += Time.deltaTime;
                yield return null; // 다음 프레임까지 대기
            }

            // 속도 원래대로
            agent.speed = originalSpeed;
        }

        _isAttacking = false;
        currentState = State.Move;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + transform.forward * (_box_Length / 2f);
        Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);
        Quaternion orientation = Quaternion.LookRotation(transform.forward);

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
    }

}
