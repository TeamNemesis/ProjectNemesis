using System;
using UnityEngine;

/// <summary>
/// 플레이어의 상태, 속성, 데이터(체력, 경험치, 레벨 등)를 관리하는 클래스입니다.
/// 게임 내에서 플레이어의 다양한 정보와 상태값을 저장하고 제공합니다.
/// </summary>
public class PlayerModel : CharacterModelBase
{



    #region Test
    public event Action<Transform> Attack;
    public Transform currentTarget;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            OnAttack();
        }
    }

    public void OnAttack()
    {
        Attack?.Invoke(currentTarget);
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

    public override void Initialize()
    {
        base.Initialize();
        //currentHealth = maxHealth; // 초기화 시 현재 체력을 최대 체력으로 설정
        //OnHpChangedEventPlay(currentHealth); // 초기 체력 이벤트 발행
        //debuffHandler.Initialize();
    }
}
