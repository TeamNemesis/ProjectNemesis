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
    [Header("Local Stats")]
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private float _box_Length = 3;
    [SerializeField] private float _box_Height = 3;
    [SerializeField] private float _box_Width = 3;

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
        float distance = Vector3.Distance(transform.position, player.position);
        _isAttacking = true;

        if (player != null && distance <= attackRange)
        {
            // 몬스터 기준 중심 위치 설정
            Vector3 center = transform.position + transform.forward * (_box_Length / 2f);

            // 박스의 반 크기
            Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);

            // 박스의 회전 (몬스터 정면을 기준으로 정렬)
            Quaternion orientation = Quaternion.LookRotation(transform.forward);

            // 박스 영역 안의 적 탐색
            Collider[] hitTarget = Physics.OverlapBox(center, halfExtents, orientation);


            foreach (Collider target in hitTarget)
            {
                if (target.TryGetComponent(out IDamageable playerHealth) && target.tag == targetTag)
                {
                    yield return new WaitForSeconds(0.5f);
                    float finalPlayerDistance = Vector3.Distance(transform.position, player.position);
                    if (finalPlayerDistance <= attackRange)
                    {
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(attackDamage);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
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
