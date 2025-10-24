using System;
using UnityEngine;

/// <summary>
/// 플레이어의 상태, 속성, 데이터(체력, 경험치, 레벨 등)를 관리하는 클래스입니다.
/// 게임 내에서 플레이어의 다양한 정보와 상태값을 저장하고 제공합니다.
/// </summary>
public class PlayerModel : CharacterModelBase
{



    #region Test
    ///// <summary>
    ///// 테스트용 공격 실행 이벤트
    ///// </summary>
    //public event Action AttackTry;
    /// <summary>
    /// 테스트용 공격 적중시 이벤트
    /// </summary>
    //public event Action<Transform> AttackHit;

    ///// <summary>
    ///// 테스트용 유탄 착탄 이벤트
    ///// </summary>
    //public event Action<Vector3> GrenadeBomb;

    ///// <summary>
    ///// 테스트용 특수 공격 실행 이벤트
    ///// </summary>
    //public event Action SPAttackTry;

    ///// <summary>
    ///// 테스트용 특수 공격 적중 이벤트
    ///// </summary>
    //public event Action<Transform> SPAttackHit;


    //public Transform currentTarget;

    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {

    //        AttackHit.Invoke(currentTarget);
    //    }

    //}


    #endregion


    /// <summary>
    /// 플레이어 피격시
    /// </summary>
    public event Action<Transform> PlayerHit;

    /// <summary>
    /// 회피 가능한지
    /// </summary>
    private bool bIsAvoid;

    /// <summary>
    /// 회피율
    /// </summary>
    private float _avoidNum;
    public float avoidNum { get { return _avoidNum; } }
    public void SetAvoidNum(float avoidNum)
    {
        _avoidNum = avoidNum;
    }

    /// <summary>
    /// 데미지 감소 효과 적용
    /// </summary>
    private bool BisReduceDamage;
    private float _damageReducePercent;

    public override void TakeDamage(float damage)
    {
        if (bIsAvoid)
        {
            int tempNum = UnityEngine.Random.Range(0, 100);
            if (tempNum > 100 * _avoidNum)
            {
                return;
            }
        }

        if(BisReduceDamage)
        {
            damage*=(1f-_damageReducePercent);
        }


        base.TakeDamage(damage);
    }
    public override void Initialize()
    {
        base.Initialize();
        SetCurrentHp(maxHealth); // 초기화 시 현재 체력을 최대 체력으로 설정
        debuffHandler.InitializePlayer();
        Debug.Log("연결");
        GameManager.Instance.PlayerStatManager.OnplayerAvoidanceChange += OnPlayerAvoidanceChange;
        GameManager.Instance.PlayerStatManager.OnplayerHitPercentChange += OnPlayerHitReducePercentChange;
    }

    private void OnPlayerHitReducePercentChange(float reducePercent)
    {
        if (reducePercent > 0)
        {
            BisReduceDamage = true;
        }
        else
        {
            BisReduceDamage = false;
        }
        _avoidNum = reducePercent;
    }

    private void OnPlayerAvoidanceChange(float avoidance)
    {
        if (avoidance > 0)
        {
            bIsAvoid = true;
        }
        else
        {
            bIsAvoid = false;
        }
        _avoidNum = avoidance;
    }

    public void OnPlayerHit(Transform monsterTransform)
    {
        PlayerHit?.Invoke(monsterTransform);
    }
}
