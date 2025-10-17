using System;
using UnityEngine;

/// <summary>
/// 플레이어의 상태, 속성, 데이터(체력, 경험치, 레벨 등)를 관리하는 클래스입니다.
/// 게임 내에서 플레이어의 다양한 정보와 상태값을 저장하고 제공합니다.
/// </summary>
public class PlayerModel : CharacterModelBase
{



    #region Test
    /// <summary>
    /// 테스트용 공격 실행 이벤트
    /// </summary>
    public event Action AttackTry;
    /// <summary>
    /// 테스트용 공격 적중시 이벤트
    /// </summary>
    public event Action<Transform> AttackHit;

    /// <summary>
    /// 테스트용 특수 공격 실행 이벤트
    /// </summary>
    public event Action SPAttackTry;

    /// <summary>
    /// 테스트용 특수 공격 적중 이벤트
    /// </summary>
    public event Action<Transform> SPAttackHit;


    /// <summary>
    /// 플레이어 테스트용 공격력
    /// </summary>
    public int playerAttack;
    public Transform currentTarget;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            GameManager.Instance.skillManager.skill_One.SpreadPoison();
            OnAttackHit();
        }
    }

  

    public void OnAttackHit()
    {
        AttackHit?.Invoke(currentTarget);
    }
    #endregion

    public void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        SetCurrentHp(maxHealth); // 초기화 시 현재 체력을 최대 체력으로 설정
        OnHpChangedEventPlay(currentHealth); // 초기 체력 이벤트 발행
        debuffHandler.InitializePlayer();
    }
}
