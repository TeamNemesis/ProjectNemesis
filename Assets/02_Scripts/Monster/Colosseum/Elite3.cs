using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Elite3 : MonsterBase
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

    [Header("PoisonField")]
    [SerializeField] private float _poisonFieldDuration = 999f; // 독성 구름 지속 시간
    [SerializeField] private float _poisonFieldRadius = 6f;   // 독성 구름 반경
    [SerializeField] private float _poisonFieldDelay = 2f;

    [Header("Bullet")]
    [SerializeField] float bulletLifeTime = 8f;
    [SerializeField] int maxBulletAttackCounter = 0;

    [SerializeField] private bool _isAttacking = false;

    [Header("Prefabs")]
    [SerializeField] private PoolableObject squareDecalPrefab;
    [SerializeField] private PoolableObject attackDecalPrefab;
    [SerializeField] private PoolableObject poisonFieldPrefab;
    [SerializeField] private PoolableObject eliteBulletPrefab;

    [Header("CoolTimes")]
    [SerializeField] private float bulletAttackCoolTime = 5f;
    [SerializeField] private float poisonLaserAttackCoolTime = 5f;
    [SerializeField] private float poisonFieldAttackCoolTime = 10f;

    [Header("STATE"), SerializeField] private State currentState = State.Idle;

    private void Update()
    {
        CoolTimeController();
        if (isDead || _target == null) return;
        if (isStunned) return;

        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Attack:
                if (!_isAttacking)
                {

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
        if (distance <= attackRange && CanSeePlayer())
        {
            currentState = State.Attack;
        }
    }


    private IEnumerator BulletAttack()
    {
        _isAttacking = true;
        bulletAttackCoolTime = 5f; // 총알 공격 쿨타임 초기화
        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            float angleOffset = 0f; // 회전 각도 오프셋
            int maxRotations = 20; // 총 회전 수

            for (int rotation = 0; rotation < maxRotations; rotation++)
            {
                // 8방향으로 총알 발사
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * 45f + angleOffset;
                    Quaternion bulletRotation = Quaternion.Euler(0, angle, 0);
                    GameObject bullet = GameManager.Instance.PoolManager.GetFromPool(eliteBulletPrefab, transform.position, bulletRotation);
                    EliteBullet turretBullet = bullet.GetComponent<EliteBullet>();
                    if (turretBullet != null)
                    {
                        turretBullet.Initialize(targetTag, attackDamage, bulletLifeTime, gameObject);
                    }
                }

                angleOffset += 10f; // 매 회전마다 10도씩 회전

                // 360도를 넘으면 초기화 (한 바퀴 완성)
                if (angleOffset >= 360f)
                {
                    angleOffset -= 360f;
                }

                yield return new WaitForSeconds(attackDelay);
            }
        }
        _isAttacking = false;
        maxBulletAttackCounter = 0;
        currentState = State.Attack;
    }


    private IEnumerator MissileAttack()
    {

        yield return null;
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

        }
        if (poisonFieldAttackCoolTime <= 0)
        {

        }

        // 사용 가능한 스킬이 있으면 랜덤으로 선택
        if (availableSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            StartCoroutine(availableSkills[randomIndex]);
        }
        else
        {
            currentState = State.Attack;
        }
    }
}
