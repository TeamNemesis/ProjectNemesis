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
    [Header("Local Stats")]
    [SerializeField] private float _poisonFieldDuration = 5f; // 독성 구름 지속 시간
    [SerializeField] private float _poisonFieldRadius = 3f;   // 독성 구름 반경
    [SerializeField] private float _poisonFieldDelay = 2f;
    [SerializeField] private bool _isAttacking = false;

    [Header("PoisonFieldPrefab"),SerializeField]
    private PoolableObject poisonFieldPrefab; // 독성 구름 프리팹

    [Header("AttackDecalPrefab"),SerializeField]
    private PoolableObject attackDecalPrefab; // 공격 장판 프리팹

    [SerializeField]
    private State currentState = State.Idle;

    private void Update()
    {
        if (isDead || _target == null) return;
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
        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            poisonFieldPrefab.GetComponent<PoisonField>().SetLifeTime(_poisonFieldDuration); // 독성 구름 지속 시간 설정

            Vector3 attackPos = _target.position;

            GameObject decalObj = ObjectPool.Instance.GetFromPool(attackDecalPrefab, attackPos);
            decalObj.GetComponent<AttackDecalEffect>().Play(_poisonFieldDelay, _poisonFieldRadius / 2);

            // 장판 생성 후 딜레이동안 대기
            yield return new WaitForSeconds(_poisonFieldDelay);

            GameObject poisonObj = ObjectPool.Instance.GetFromPool(poisonFieldPrefab, attackPos);// 플레이어에게 독성 구름 발사
            PoisonField poisonField =  poisonObj.GetComponent<PoisonField>();
            poisonField.Initialize(targetTag, _poisonFieldDuration, _poisonFieldRadius);

            // 독성 구름을 발사한 후 일정 시간 대기
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }
}
