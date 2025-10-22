using System;
using System.Collections;
using UnityEngine;

public class Elite2 : MonsterBase
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
    [SerializeField] private int laserAttackCount = 0; // 연속 공격 횟수
    [SerializeField] private float _poisonFieldDuration = 999f; // 독성 구름 지속 시간
    [SerializeField] private float _poisonFieldRadius = 6f;   // 독성 구름 반경
    [SerializeField] private float _poisonFieldDelay = 2f;
    [SerializeField] private bool _isAttacking = false;

    [SerializeField] private float poisonFieldAttackCoolTime = 0;
    [SerializeField] private float poisonLaserAttackCoolTime = 0;
    [SerializeField] private float bulletAttackCoolTime = 0;


    [Header("PoisonFieldPrefab"), SerializeField]
    private PoolableObject poisonFieldPrefab; // 독성 구름 프리팹

    [Header("AttackDecalPrefab"), SerializeField]
    private PoolableObject attackDecalPrefab; // 공격 장판 프리팹

    [Header("SquareDecalPrefab"), SerializeField]
    private PoolableObject squareDecalPrefab;

    [Header("EliteBullet"),SerializeField]
    private PoolableObject eliteBulletPrefab; // 엘리트 총알 프리팹

    [SerializeField] private State currentState = State.Idle;

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
                    StartCoroutine(PoisonLaserAttack());
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

    /// <summary>
    /// 독성 장판 공격
    /// </summary>
    private IEnumerator PoisonFieldAttack()
    {
        _isAttacking = true;
        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            poisonFieldPrefab.GetComponent<PoisonField>().SetLifeTime(_poisonFieldDuration); // 독성 구름 지속 시간 설정

            Vector3 attackPos = _target.position;

            GameObject decalObj = ObjectPool.Instance.GetFromPool(attackDecalPrefab, attackPos,attackDecalPrefab.transform.rotation);
            decalObj.GetComponent<AttackDecalEffect>().Play(_poisonFieldDelay, _poisonFieldRadius / 2);

            // 장판 생성 후 딜레이동안 대기
            yield return new WaitForSeconds(_poisonFieldDelay);

            GameObject poisonObj = ObjectPool.Instance.GetFromPool(poisonFieldPrefab, attackPos, poisonFieldPrefab.transform.rotation);// 플레이어에게 독성 구름 발사
            PoisonField poisonField = poisonObj.GetComponent<PoisonField>();
            poisonField.Initialize(targetTag, _poisonFieldDuration, _poisonFieldRadius);

            // 공격 후 일정 시간 대기
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }

    /// <summary>
    /// 독성 레이저 공격
    /// </summary>
    private IEnumerator PoisonLaserAttack()
    {
        _isAttacking = true;
        while (laserAttackCount < 5) {
            if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
            {
                Vector3 spawnPos = transform.position + transform.forward * 40;
                spawnPos.y = 0;
                GameObject decalobj = ObjectPool.Instance.GetFromPool(squareDecalPrefab, spawnPos, squareDecalPrefab.transform.rotation);

                decalobj.GetComponent<SquareDecalEffect>().Play(attackDelay, transform, new Vector3(90, 0, 0));
                yield return new WaitForSeconds(attackDelay);

                float laserLength = 40f; // 사거리

                // 레이저 시작 위치를 플레이어 높이에 맞춤
                Vector3 startPos = transform.position + transform.forward * 0.5f;
                startPos.y = _target.position.y + 0.5f;

                // 디버그용 레이저 표시
                Debug.DrawRay(startPos, transform.forward * laserLength, Color.green, 0.3f);

                // 벽에 막히는 Raycast
                if (Physics.Raycast(startPos, transform.forward, out RaycastHit hit, laserLength, ~0, QueryTriggerInteraction.Collide))
                {
                    if (hit.collider.CompareTag(targetTag))
                    {
                        DebuffHandler debuffHandler = hit.collider.GetComponent<DebuffHandler>();
                        if (debuffHandler != null)
                        {
                            DebuffHandler.DebuffData poison = DebuffHandler.DebuffData.CreatePoison();
                            debuffHandler.ApplyDebuff(poison);
                        }
                    }
                }
                yield return new WaitForSeconds(attackDelay);
                laserAttackCount++;
            }
        }

        _isAttacking = false;
        laserAttackCount = 0;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }


    private IEnumerator BulletAttack()
    {
               _isAttacking = true;
        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }
}
