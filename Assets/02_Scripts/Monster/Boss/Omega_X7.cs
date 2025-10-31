using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Omega_X7 : MonsterBase
{
    [Header("Local Stats")]


    [Header("Missile")]
    [SerializeField] private int missileAttackCounter = 0;
    [SerializeField] private int maxMissileAttackCount = 5;

    [Header("Bullet")]
    [SerializeField] float bulletLifeTime = 8f;

    [Header("Prefabs")]
    [SerializeField] private PoolableObject missilePrefab;
    [SerializeField] private PoolableObject eliteBulletPrefab;

    [Header("CoolTimes")]
    [SerializeField] private float bulletAttackCoolTime = 5f;
    [SerializeField] private float missileAttackCoolTime = 7f;

    private float lastHealthCheckThreshold = 1f;
    private bool _isPhase2 = false;


    private void Update()
    {
        CoolTimeController();
        if (isDead || _target == null) return;
        if (isStunned) return;

        switch (baseState)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Attack:
                if (!_isAttacking)
                {
                    StartCoroutine(MissileAttack());
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
        if (distance <= attackRange && CanSeePlayer())
        {
            baseState = MonsterState.Attack;
        }
    }

    #region 총알 패턴 코루틴
    private IEnumerator BulletAttack()
    {
        _isAttacking = true;
        {
            yield return null;
        }
        _isAttacking = false;
    }
    #endregion

    #region 미사일 패턴 코루틴
    private IEnumerator MissileAttack()
    {
        _isAttacking = true;

        // 4방향 벡터 정의 (X-Z 평면)
        Vector3[] directions = new Vector3[]
        { Vector3.right, Vector3.left};

        while (!isDead)
        {
            // 랜덤하게 방향 선택
            Vector3 randomDirection = directions[Random.Range(0, directions.Length)];

            // 선택된 방향으로 스폰 위치 계산
            Vector3 spawnPos = transform.position + randomDirection * transform.localScale.z;
            spawnPos.y = 0;

            GameObject missileObj = GameManager.Instance.PoolManager.GetFromPool(missilePrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(3f);
        }

        _isAttacking = false;
        baseState = MonsterState.Attack;
    }
    #endregion

    #region 쿨타임 컨트롤러
    private void CoolTimeController()
    {
        if (bulletAttackCoolTime > 0)
            bulletAttackCoolTime -= Time.deltaTime;
        if (missileAttackCoolTime > 0)
            missileAttackCoolTime -= Time.deltaTime;
    }
    #endregion

    #region 랜덤 스킬 사용
    private void TryUseSkill()
    {
        // 사용 가능한 스킬 코루틴 리스트
        List<IEnumerator> availableSkills = new List<IEnumerator>();

        if (bulletAttackCoolTime <= 0)
        {
            availableSkills.Add(BulletAttack());
        }
        if (missileAttackCoolTime <= 0)
        {
            availableSkills.Add(MissileAttack()); availableSkills.Add(MissileAttack());
        }

        // 사용 가능한 스킬이 있으면 랜덤으로 선택
        if (availableSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            StartCoroutine(availableSkills[randomIndex]);
        }
        else
        {
            baseState = MonsterState.Attack;
        }
    }
    #endregion

    public override void TakeDamage(float damage, Transform attacker)
    {
        base.TakeDamage(damage, attacker);

        float healthRatio = (float)currentHealth / (float)maxHealth;

        // 체력 임계값을 넘었을 때만 체크
        if (!_isPhase2 && healthRatio <= 0.7f && lastHealthCheckThreshold > 0.7f)
        {
            _isPhase2 = true;
        }

        lastHealthCheckThreshold = healthRatio;
    }
}
