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

    [Header("Shotgun Attack")]
    [SerializeField] private float shotgunDamage = 30f;
    [SerializeField] private float shotgunRange = 1000f;
    [SerializeField] private float shotgunAngle = 120f;
    [SerializeField] private float shotgunWarningDuration = 2f;

    [Header("Prefabs")]
    [SerializeField] private PoolableObject missilePrefab;
    [SerializeField] private PoolableObject eliteBulletPrefab;
    [SerializeField] private PoolableObject shotgunDecalPrefab; // 샷건 범위 표시 프리팹

    [Header("CoolTimes")]
    [SerializeField] private float shotgunttackCoolTime = 5f;
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
        if (distance <= attackRange && CanSeePlayer())
        {
            baseState = MonsterState.Attack;
        }
    }

    #region 샷건 패턴 코루틴
    private IEnumerator ShotgunAttack()
    {
        _isAttacking = true;

        // 플레이어 방향 계산
        if (_target == null)
        {
            _isAttacking = false;
            baseState = MonsterState.Idle;
            yield break;
        }

        Vector3 directionToPlayer = new Vector3(
            _target.position.x - transform.position.x,
            0,
            _target.position.z - transform.position.z
        ).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // 샷건 범위 표시 프리팹 생성 (본체 크기의 절반만큼 앞)
        float forwardOffset = transform.localScale.z / 2f;
        Vector3 spawnPos = transform.position + directionToPlayer * forwardOffset;
        spawnPos.y = 0.1f; // Y 좌표 고정

        GameObject decalObj = GameManager.Instance.PoolManager.GetFromPool(
            shotgunDecalPrefab,
            spawnPos,
            lookRotation
        );

        ShotgunDecalEffect decalEffect = decalObj.GetComponent<ShotgunDecalEffect>();
        if (decalEffect != null)
        {
            decalEffect.Play();
        }

        // 2초 대기 (경고 시간)
        yield return new WaitForSeconds(shotgunWarningDuration);

        // 샷건 공격 실행 (히트스캔) - 같은 위치에서
        PerformShotgunHitscan(spawnPos, directionToPlayer);

        // 쿨타임 리셋
        shotgunttackCoolTime = 5f;

        _isAttacking = false;
        baseState = MonsterState.Idle;
    }

    private void PerformShotgunHitscan(Vector3 attackOrigin, Vector3 attackDirection)
    {
        // 플레이어가 범위 내에 있는지 확인
        if (_target == null) return;

        Vector3 directionToTarget = (_target.position - attackOrigin).normalized;
        float angleToTarget = Vector3.Angle(attackDirection, directionToTarget);
        float distanceToTarget = Vector3.Distance(attackOrigin, _target.position);

        // 각도와 거리 체크
        if (angleToTarget <= shotgunAngle / 2f && distanceToTarget <= shotgunRange)
        {
            // 플레이어에게 데미지
            PlayerHealth playerHealth = _target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(shotgunDamage);
            }
        }
    }
    #endregion

    #region 미사일 패턴 코루틴
    private IEnumerator MissileAttack()
    {
        _isAttacking = true;

        // 2방향 벡터 정의 (X-Z 평면)
        Vector3[] directions = new Vector3[]
        { Vector3.right, Vector3.left};

        int attackCount = 0;
        int maxAttacks = 3;

        while (!isDead && attackCount < maxAttacks)
        {
            // 랜덤하게 방향 선택
            Vector3 randomDirection = directions[Random.Range(0, directions.Length)];

            // 선택된 방향으로 스폰 위치 계산
            Vector3 spawnPos = transform.position + randomDirection * transform.localScale.z;
            spawnPos.y = 0;

            GameObject missileObj = GameManager.Instance.PoolManager.GetFromPool(missilePrefab, spawnPos, Quaternion.identity);

            attackCount++;
            yield return new WaitForSeconds(3f);
        }

        // 쿨타임 리셋
        missileAttackCoolTime = 7f;

        _isAttacking = false;
        baseState = MonsterState.Idle;
    }
    #endregion

    #region 쿨타임 컨트롤러
    private void CoolTimeController()
    {
        if (shotgunttackCoolTime > 0)
            shotgunttackCoolTime -= Time.deltaTime;
        if (missileAttackCoolTime > 0)
            missileAttackCoolTime -= Time.deltaTime;
    }
    #endregion

    #region 랜덤 스킬 사용
    private void TryUseSkill()
    {
        // 사용 가능한 스킬 코루틴 리스트
        List<IEnumerator> availableSkills = new List<IEnumerator>();

        if (shotgunttackCoolTime <= 0)
        {
            availableSkills.Add(ShotgunAttack());
        }
        if (missileAttackCoolTime <= 0)
        {
            availableSkills.Add(MissileAttack());
        }

        // 사용 가능한 스킬이 있으면 랜덤으로 선택
        if (availableSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            StartCoroutine(availableSkills[randomIndex]);
        }
        else
        {
            baseState = MonsterState.Idle;
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