using System.Collections;
using UnityEngine;

public class NebulaChemicalDisease : MonsterBase
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
    [SerializeField] private float _poisinFieldDuration = 5f; // 독성 구름 지속 시간
    [SerializeField] private float _poisinFieldRadius = 3f;   // 독성 구름 반경
    [SerializeField] private bool _isAttacking = false;

    [Header("PoisonFieldPrefab"),SerializeField]
    private GameObject poisonFieldPrefab; // 독성 구름 프리팹

    [SerializeField]
    private State currentState = State.Idle;

    private void Update()
    {
        if (isDead || player == null) return;

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
            poisonFieldPrefab.gameObject.transform.localScale = new Vector3(_poisinFieldRadius, 1, _poisinFieldRadius);
            poisonFieldPrefab.GetComponent<PoisinField>().SetDuration(_poisinFieldDuration); // 독성 구름 지속 시간 설정
            Instantiate(poisonFieldPrefab, transform.position, transform.rotation); // 플레이어에게 독성 구름 발사
            // 독성 구름을 발사한 후 일정 시간 대기
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }
}
