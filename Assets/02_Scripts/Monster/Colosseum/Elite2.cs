using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Elite2 : MonsterBase
{
    [Header("Local Stats")]
    [SerializeField] private int laserAttackCount = 0; // 연속 공격 횟수

    [Header("PoisonField")]
    [SerializeField] private float _poisonFieldDuration = 999f; // 독성 구름 지속 시간
    [SerializeField] private float _poisonFieldRadius = 6f;   // 독성 구름 반경
    [SerializeField] private float _poisonFieldDelay = 2f;

    [Header("Bullet")]
    [SerializeField] float bulletLifeTime = 8f;
    [SerializeField] int maxBulletAttackCounter = 0;

    [Header("Prefabs")]
    [SerializeField] private PoolableObject squareDecalPrefab;
    [SerializeField] private PoolableObject attackDecalPrefab;
    [SerializeField] private PoolableObject poisonFieldPrefab;
    [SerializeField] private PoolableObject eliteBulletPrefab;

    [Header("CoolTimes")]
    [SerializeField] private float bulletAttackCoolTime = 5f;
    [SerializeField] private float poisonLaserAttackCoolTime = 5f;
    [SerializeField] private float poisonFieldAttackCoolTime = 10f;

    private void Update()
    {
        CoolTimeController();
        if (isDead || _target == null) return;
        if (isStunned) return;

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
                if (!_isAttacking)
                {
                    TryUseSkill();
                }
                break;
            case MonsterState.Die:
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
            baseState = MonsterState.Move;
        }
    }
    private void HandleMove()
    {
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

    /// <summary>
    /// 독성 장판 공격
    /// </summary>
    private IEnumerator PoisonFieldAttack()
    {
        _isAttacking = true;
        poisonFieldAttackCoolTime = 10f; // 독성 장판 공격 쿨타임 초기화

        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            poisonFieldPrefab.GetComponent<PoisonField>().SetLifeTime(_poisonFieldDuration); // 독성 구름 지속 시간 설정

            Vector3 attackPos = _target.position;

            GameObject decalObj = GameManager.Instance.PoolManager.GetFromPool(attackDecalPrefab, attackPos, attackDecalPrefab.transform.rotation);
            decalObj.GetComponent<AttackDecalEffect>().Play(_poisonFieldDelay, _poisonFieldRadius / 2);

            // 장판 생성 후 딜레이동안 대기
            yield return new WaitForSeconds(_poisonFieldDelay);

            GameObject poisonObj = GameManager.Instance.PoolManager.GetFromPool(poisonFieldPrefab, attackPos, poisonFieldPrefab.transform.rotation);// 플레이어에게 독성 구름 발사
            PoisonField poisonField = poisonObj.GetComponent<PoisonField>();
            poisonField.Initialize(targetTag, _poisonFieldDuration, _poisonFieldRadius);

            // 공격 후 일정 시간 대기
            yield return new WaitForSeconds(attackDelay);
        }
        _isAttacking = false;
        baseState = MonsterState.Move; // 공격 후 다시 추격 상태로 전환
    }

    /// <summary>
    /// 독성 레이저 공격
    /// </summary>
    private IEnumerator PoisonLaserAttack()
    {
        _isAttacking = true;
        poisonFieldAttackCoolTime = 5f; // 독성 레이저 공격 쿨타임 초기화

        while (laserAttackCount < 5)
        {
            if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
            {
                Vector3 spawnPos = transform.position + transform.forward * 40;
                spawnPos.y = 0;
                GameObject decalobj = GameManager.Instance.PoolManager.GetFromPool(squareDecalPrefab, spawnPos, squareDecalPrefab.transform.rotation);

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
        baseState = MonsterState.Move; // 공격 후 다시 추격 상태로 전환
    }


    private IEnumerator BulletAttack()
    {
        _isAttacking = true;
        bulletAttackCoolTime = 5f; // 총알 공격 쿨타임 초기화

        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            while (maxBulletAttackCounter < 2)
            {
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * 45f;
                    Quaternion rotation = Quaternion.Euler(0, angle, 0);

                    GameObject bullet = GameManager.Instance.PoolManager.GetFromPool(eliteBulletPrefab, transform.position, rotation);
                    EliteBullet turretBullet = bullet.GetComponent<EliteBullet>();
                    if (turretBullet != null)
                    {
                        turretBullet.Initialize(targetTag, attackDamage, bulletLifeTime, gameObject);
                    }
                }
                maxBulletAttackCounter++;
                yield return new WaitForSeconds(attackDelay);
            }
        }
        _isAttacking = false;
        maxBulletAttackCounter = 0;
        baseState = MonsterState.Move; // 공격 후 다시 추격 상태로 전환
    }

    private void CoolTimeController()
    {
        if (bulletAttackCoolTime > 0)
            bulletAttackCoolTime -= Time.deltaTime;
        if (poisonLaserAttackCoolTime > 0)
            poisonLaserAttackCoolTime -= Time.deltaTime;
        if (poisonFieldAttackCoolTime > 0)
            poisonFieldAttackCoolTime -= Time.deltaTime;
    }

    private void TryUseSkill()
    {
        // 사용 가능한 스킬 코루틴 리스트
        List<IEnumerator> availableSkills = new List<IEnumerator>();

        if (bulletAttackCoolTime <= 0)
        {
            availableSkills.Add(BulletAttack());
        }
        if (poisonLaserAttackCoolTime <= 0)
        {
            availableSkills.Add(PoisonLaserAttack());
        }
        if (poisonFieldAttackCoolTime <= 0)
        {
            availableSkills.Add(PoisonFieldAttack());
        }

        // 사용 가능한 스킬이 있으면 랜덤으로 선택
        if (availableSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            StartCoroutine(availableSkills[randomIndex]);
        }
        else
        {
            // 모든 스킬이 쿨타임이면 다시 추격
            baseState = MonsterState.Move;
        }
    }
}
