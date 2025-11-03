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
    [SerializeField] private float misileAttackDamage = 20f;

    [Header("Shotgun Attack")]
    [SerializeField] private float shotgunDamage = 30f;
    [SerializeField] private float shotgunRange = 1000f;
    [SerializeField] private float shotgunAngle = 120f;
    [SerializeField] private float shotgunWarningDuration = 2f;

    [Header("Prefabs")]
    [SerializeField] private PoolableObject missilePrefab;
    [SerializeField] private PoolableObject laserTurretPrefab;
    [SerializeField] private PoolableObject shotgunDecalPrefab; // 샷건 범위 표시 프리팹
    [SerializeField] private PoolableObject iceTankPrefab;

    [Header("CoolTimes")]
    [SerializeField] private float shotgunttackCoolTime = 5f;
    [SerializeField] private float missileAttackCoolTime = 7f;
    [SerializeField] private float laserTurretSpawnCoolTime = 30f;

    [Header("Local Stats")]
    private bool _isShotgunAttacking = false;
    private bool _isMissileAttacking = false;
    private bool _isIceTankSpawning = false;

    private float lastHealthCheckThreshold = 1f;
    private bool _isPhase2 = false;

    [Header("Phase2 Objects")]
    [SerializeField]private MonsterGrenade monsterGrenade;

    private void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        // MonsterGrenade 컴포넌트 찾기
        monsterGrenade = GetComponentInChildren<MonsterGrenade>();

        if (monsterGrenade == null)
        {
            Debug.LogError($"[Omega_X7] MonsterGrenade component not found in children! GameObject: {gameObject.name}");

            // 모든 자식 오브젝트 출력
            Transform[] children = GetComponentsInChildren<Transform>();
            Debug.Log($"[Omega_X7] Total children count: {children.Length}");
            foreach (Transform child in children)
            {
                MonsterGrenade mg = child.GetComponent<MonsterGrenade>();
                Debug.Log($"[Omega_X7] Child: {child.name}, Has MonsterGrenade: {mg != null}");
            }
        }
        else
        {
            Debug.Log($"[Omega_X7] MonsterGrenade found successfully on GameObject: {monsterGrenade.gameObject.name}");
        }
    }

    private void Update()
    {
        CoolTimeController();
        if (isDead || _target == null) return;
        if (isStunned) return;
        LookAtPlayer();

        switch (baseState)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Attack:
                TryUseSkill();
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

    #region 레이저 터렛 스폰
    private IEnumerator SpawnLaserTurret()
    {
        _isAttacking = true;

        // 터렛 스폰 위치 계산 (본체 기준 상대 좌표)
        Vector3 leftBottomSpawnPos = transform.position + new Vector3(-15, 0, -15);
        Vector3 rightBottomSpawnPos = transform.position + new Vector3(15, 0, -15);
        Vector3 upSpawnPos = transform.position + new Vector3(0, 0, 15);

        // 왼쪽 아래 터렛 스폰
        GameObject leftBottomTurretObj = GameManager.Instance.PoolManager.GetFromPool(
            laserTurretPrefab,
            leftBottomSpawnPos,
            Quaternion.identity
        );

        // 오른쪽 아래 터렛 스폰
        GameObject rightBottomTurretObj = GameManager.Instance.PoolManager.GetFromPool(
            laserTurretPrefab,
            rightBottomSpawnPos,
            Quaternion.identity
        );

        // 위쪽 터렛 스폰
        GameObject upTurretObj = GameManager.Instance.PoolManager.GetFromPool(
            laserTurretPrefab,
            upSpawnPos,
            Quaternion.identity
        );

        // 쿨타임 리셋
        laserTurretSpawnCoolTime = 30f;

        _isAttacking = false;

        yield return null; // 코루틴 형식 유지
    }
    #endregion

    #region 아이스 탱크 스폰
    private IEnumerator SpawnIceTank()
    {
        // 아이스 탱크 스폰 시작
        _isIceTankSpawning = true;
        bool previousAttackingState = _isAttacking;
        _isAttacking = true;

        // 아이스 탱크 스폰 위치 계산
        Vector3 leftTopSpawnPos = transform.position + new Vector3(-15, 0, 15);
        Vector3 rightTopSpawnPos = transform.position + new Vector3(15, 0, 15);
        Vector3 bottomSpawnPos = transform.position + new Vector3(0, 0, -15);

        // 아이스 탱크 스폰 및 리스트에 저장
        List<GameObject> spawnedIceTanks = new List<GameObject>();

        GameObject leftTopIceTank = GameManager.Instance.PoolManager.GetFromPool(
            iceTankPrefab,
            leftTopSpawnPos,
            Quaternion.identity
        );
        spawnedIceTanks.Add(leftTopIceTank);

        GameObject rightTopIceTank = GameManager.Instance.PoolManager.GetFromPool(
            iceTankPrefab,
            rightTopSpawnPos,
            Quaternion.identity
        );
        spawnedIceTanks.Add(rightTopIceTank);

        GameObject bottomIceTank = GameManager.Instance.PoolManager.GetFromPool(
            iceTankPrefab,
            bottomSpawnPos,
            Quaternion.identity
        );
        spawnedIceTanks.Add(bottomIceTank);

        // 10초 대기
        yield return new WaitForSeconds(10f);

        // 10초 후 남아있는 아이스 탱크 개수 확인
        int survivingIceTankCount = 0;
        foreach (GameObject iceTank in spawnedIceTanks)
        {
            if (iceTank != null && iceTank.activeInHierarchy)
            {
                // 남아있는 탱크는 풀로 반환
                GameManager.Instance.PoolManager.ReleaseToPool(iceTank);
                survivingIceTankCount++;
            }
        }

        // 생존한 아이스 탱크 수만큼 보스 데미지 영구 증가 (각 30%)
        if (survivingIceTankCount > 0)
        {
            float damageMultiplier = 1f + (survivingIceTankCount * 0.3f);
            shotgunDamage *= damageMultiplier;

            // 유탄 데미지도 증가
            if (monsterGrenade != null)
            {
                monsterGrenade.IncreaseDamage(damageMultiplier);
            }

            Debug.Log($"[Omega_X7] {survivingIceTankCount} ice tanks survived. Damage multiplier: {damageMultiplier}x");
        }

        _isAttacking = previousAttackingState;
        _isIceTankSpawning = false; // 아이스 탱크 스폰 완료

        // 아이스 탱크 스폰 완료 후 유탄 발사 패턴 시작 (15초마다 반복)
        Debug.Log($"[Omega_X7] Attempting to start grenade pattern. monsterGrenade is null: {monsterGrenade == null}");
        if (monsterGrenade != null)
        {
            monsterGrenade.StartGrenadePattern();
        }
        else
        {
            Debug.LogError("[Omega_X7] Cannot start grenade pattern - monsterGrenade is null!");
        }
    }
    #endregion

    #region 샷건 패턴 코루틴
    private IEnumerator ShotgunAttack()
    {
        _isShotgunAttacking = true;

        // 플레이어 방향 계산
        if (_target == null)
        {
            _isShotgunAttacking = false;
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

        _isShotgunAttacking = false;
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
        _isMissileAttacking = true;

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
            Missile missile = missileObj.GetComponent<Missile>();
            if (missile != null && _target != null)
            {
                missile.SetAttackDamage(misileAttackDamage);
            }

            attackCount++;
            yield return new WaitForSeconds(3f);
        }

        // 쿨타임 리셋
        missileAttackCoolTime = 7f;

        _isMissileAttacking = false;
    }
    #endregion

    #region 쿨타임 컨트롤러
    private void CoolTimeController()
    {
        if (shotgunttackCoolTime > 0)
            shotgunttackCoolTime -= Time.deltaTime;
        if (missileAttackCoolTime > 0)
            missileAttackCoolTime -= Time.deltaTime;
        if (laserTurretSpawnCoolTime > 0)
            laserTurretSpawnCoolTime -= Time.deltaTime;
    }
    #endregion

    #region 랜덤 스킬 사용
    private void TryUseSkill()
    {
        // 사용 가능한 스킬 코루틴 리스트
        List<IEnumerator> availableSkills = new List<IEnumerator>();

        // 샷건은 페이즈2 진입 후 + 아이스 탱크 스폰 완료 후에만 사용 가능
        if (shotgunttackCoolTime <= 0 && !_isShotgunAttacking && !_isIceTankSpawning && _isPhase2)
        {
            availableSkills.Add(ShotgunAttack());
        }

        // 미사일은 항상 사용 가능
        if (missileAttackCoolTime <= 0 && !_isMissileAttacking)
        {
            availableSkills.Add(MissileAttack());
        }

        // 레이저 터렛은 _isAttacking이 false일 때만 사용 가능 (다른 중요 패턴과 겹치지 않도록)
        if (laserTurretSpawnCoolTime <= 0 && !_isAttacking)
        {
            availableSkills.Add(SpawnLaserTurret());
        }

        // 사용 가능한 스킬이 있으면 랜덤으로 선택
        if (availableSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            StartCoroutine(availableSkills[randomIndex]);
        }
        else if (!_isShotgunAttacking && !_isMissileAttacking)
        {
            // 모든 공격이 끝났고 쿨타임 중일 때만 Idle로 전환
            baseState = MonsterState.Idle;
        }
    }
    #endregion

    public override void TakeDamage(float damage, Transform attacker)
    {
        base.TakeDamage(damage, attacker);

        float healthRatio = (float)currentHealth / (float)maxHealth;

        // 체력 70% 이하로 떨어졌을 때 아이스 탱크 스폰 (한 번만)
        if (!_isPhase2 && healthRatio <= 0.7f && lastHealthCheckThreshold > 0.7f)
        {
            _isPhase2 = true;

            // 아이스 탱크 스폰 패턴 시작
            StartCoroutine(SpawnIceTank());
        }

        lastHealthCheckThreshold = healthRatio;
    }

    protected override void Die()
    {
        // 유탄 발사 패턴 중지
        if (monsterGrenade != null)
        {
            monsterGrenade.StopGrenadePattern();
        }

        base.Die();
    }
}