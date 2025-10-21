using System;
using System.Collections;
using UnityEngine;

public class Drone : PoolableObject, IInitializePoolable, IReleasePoolable
{
    [SerializeField]
    private enum State
    {
        Idle, // 몬스터 감지 못함
        Attack // 공격
    }

    /// <summary>
    /// 공격 적중시 이벤트 실행
    /// </summary>
    public event Action<Transform> Attack;

    /// <summary>
    /// 공격 실행시 이벤트 실행
    /// </summary>
    public event Action attackTry;


    /// <summary>
    /// 현재 상태
    /// </summary>
    [SerializeField]
    private State _currentState = State.Idle;

    /// <summary>
    /// 드론 공격력
    /// </summary>
    [SerializeField]
    private float _attackDamage;

    /// <summary>
    /// 플레이어 모든 데미지 증가 배수
    /// </summary>
    private float _totalDamage;

    /// <summary>
    /// 드론 모든 데미지 증가 * 드론 공격력
    /// </summary>
    private float _finalDamage;


    /// <summary>
    /// 공격 쿨타임
    /// </summary>
    [SerializeField]
    private float _attackCoolTime;

    /// <summary>
    /// 사정거리
    /// </summary>
    [SerializeField]
    private float _attackRange;

    /// <summary>
    /// 탐색 콜라이더 리스트
    /// </summary>
    [SerializeField]
    private Collider[] _results;


    /// <summary>
    /// 공격 중인지
    /// </summary>
    [SerializeField]
    private bool _bIsAttacking;

    /// <summary>
    /// 현재 공격 목표
    /// </summary>
    [SerializeField]
    private Transform _currentTarget;
    public Transform currentTarget { get { return _currentTarget; } }

    /// <summary>
    /// 타겟 피격 이펙트
    /// </summary>
    public GameObject hitEffectPrefab;

    /// <summary>
    /// 이벤트 중복 연결 방지
    /// </summary>
    private bool isInit = false;

    private void Update()
    {
        //if(GameManager.Instance.player.playerModel.isDead == true)
        //{
        //    return;
        //}

        switch (_currentState)
        {
            case State.Idle:
                SearchEnemy();
                break;
            case State.Attack:
                if (currentTarget == null)
                {
                    return;
                }
                LookTarget();
                if (!_bIsAttacking)
                {
                    StartCoroutine(AttackTarget());
                }
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Attack?.Invoke(_currentTarget.transform);
        }
    }


    /// <summary>
    /// 타겟 설정 
    /// </summary>
    public void SearchEnemy()
    {
        // 콜라이더 탐색 (필요하다면 레이어마스크 설정)
        int hitColliders = Physics.OverlapSphereNonAlloc(transform.position, _attackRange, _results);

        // 임시 저장할 변수
        MonsterBase targetMonster = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < hitColliders; i++)
        {
            // Monster인지 판단
            MonsterBase currentMonster = _results[i].GetComponent<MonsterBase>();
            if (currentMonster != null)
            {
                // Monster라면 거리 비교
                float distacne = Vector3.Distance(_results[i].transform.position, transform.position);
                if (distacne < minDistance)
                {
                    minDistance = distacne;
                    targetMonster = currentMonster;

                }
            }

        }

        // 몬스터가 탐색되었다면 타겟 설정
        if (targetMonster != null)
        {
            _currentTarget = targetMonster.transform;
            _currentState = State.Attack;
        }

    }

    /// <summary>
    /// 드론이 몬스터를 바라보게
    /// </summary>
    public void LookTarget()
    {
        Vector3 direction = (_currentTarget.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * Constants.DRONE_ROTATION_SPEED);
        }
    }

    /// <summary>
    /// 몬스터 공격
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttackTarget()
    {
        _bIsAttacking = true;
        // 타겟이 존재하고 사정거리 안이라면
        if (_currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) < _attackRange)
        {
            attackTry?.Invoke();
            IDamageable target = _currentTarget.GetComponent<IDamageable>();
            if (target != null)
            {
                Attack?.Invoke(currentTarget);
                target.TakeDamage(_finalDamage);
                yield return new WaitForSeconds(_attackCoolTime);
            }
        }
        _bIsAttacking = false;
        _currentState = State.Idle;
    }

    public void UpdateFinalDamage()
    {
        _finalDamage = _attackDamage * _totalDamage;

    }


    public void Initialize(object data = null)
    {
        if (GameManager.Instance.skillManager.attackTech != null)
        {
            Attack += GameManager.Instance.skillManager.attackTech.HitEnemy;
        }
        PlayerStatManager playerStatManager = GameManager.Instance.playerStatManager;

        _results = new Collider[Constants.DRONE_SEARCHNUM];
        _attackDamage = playerStatManager.droneAttack;
        _attackRange = playerStatManager.droneAttackRange;
        _attackCoolTime = playerStatManager.droneAttackTime;
        _totalDamage = playerStatManager.totalMultiDamage;
        UpdateFinalDamage();
        if (!isInit)
        {
            playerStatManager.onDroneAttackChange += OnDroneAttackChange;
            playerStatManager.onDroneAttackRangeChange += OnDroneAttackRangeChange;
            playerStatManager.onDronAttackTimeChange += OnDroneAttackTimeChange;
            playerStatManager.onTotalDamageChange += OnDroneTotalDamageChange;
            isInit = true;
        }


    }
    public void ReleaseObjectPool()
    {
        PlayerStatManager playerStatManager = GameManager.Instance.playerStatManager;

        Attack = null;
        attackTry = null;

        if (isInit)
        {
            playerStatManager.onDroneAttackChange -= OnDroneAttackChange;
            playerStatManager.onDroneAttackRangeChange -= OnDroneAttackRangeChange;
            playerStatManager.onDronAttackTimeChange -= OnDroneAttackTimeChange;
            playerStatManager.onTotalDamageChange -= OnDroneTotalDamageChange;
            isInit = false;
        }
    }

    public void OnDroneAttackChange()
    {
        _attackDamage = GameManager.Instance.playerStatManager.droneAttack;
        UpdateFinalDamage();


    }

    public void OnDroneAttackRangeChange()
    {
        _attackRange = GameManager.Instance.playerStatManager.droneAttackRange;
    }

    public void OnDroneAttackTimeChange()
    {
        _attackCoolTime = GameManager.Instance.playerStatManager.droneAttackTime;
    }

    public void OnDroneTotalDamageChange()
    {
        _totalDamage = GameManager.Instance.playerStatManager.totalMultiDamage;
        UpdateFinalDamage();


    }

}
