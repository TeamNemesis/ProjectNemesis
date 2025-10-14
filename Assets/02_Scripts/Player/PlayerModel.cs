using System;
using UnityEngine;

/// <summary>
/// 플레이어의 상태, 속성, 데이터(체력, 경험치, 레벨 등)를 관리하는 클래스입니다.
/// 게임 내에서 플레이어의 다양한 정보와 상태값을 저장하고 제공합니다.
/// </summary>
public class PlayerModel : MonoBehaviour, IDamageAble
{
    [SerializeField] int _maxHp = 100; // 최대 체력
    [SerializeField] int _currentHp; // 현재 체력
    [SerializeField] int _moveSpeed = 5; // 이동 속도

    public int MaxHp => _maxHp; // 최대 체력을 반환하는 속성
    public int CurrentHp => _currentHp; // 현재 체력을 반환하는 속성
    public int MoveSpeed => _moveSpeed; // 이동 속도를 반환하는 속성

    public event Action<int> OnHpChanged; // 체력 변경 시 발생하는 이벤트
    public event Action<int> OnMoveSpeedChanged; // 이동 속도 변경 시 발생하는 이벤트
    public event Action OnDead; // 플레이어가 사망했을 때 발생하는 이벤트

    public ActiveTech attackSkill;
    public event Action Attack;

    public void Initialize()
    {
        _currentHp = _maxHp; // 초기화 시 현재 체력을 최대 체력으로 설정
        OnHpChanged?.Invoke(_currentHp); // 초기 체력 이벤트 발행
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= (int)damage; // 데미지만큼 현재 체력 감소
        Debug.Log("플레이어가 " + damage + "의 데미지를 입었습니다. 현재 체력: " + _currentHp);
        OnHpChanged?.Invoke(_currentHp); // 체력 변경 이벤트 발행
        if (_currentHp <= 0)
        {
            Debug.Log("플레이어가 사망했습니다.");
            Die();
        }
    }

    void Die()
    {
        // 플레이어 사망 처리 로직
        OnDead?.Invoke(); // 사망 이벤트 발행
    }

    public void SetMoveSpeed(int speed)
    {
        _moveSpeed = speed;
        OnMoveSpeedChanged?.Invoke(_moveSpeed); // 이동 속도 변경 이벤트 발행
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            OnAttack();   
        }
    }

    public void OnAttack()
    {
        Attack?.Invoke();
    }
}
