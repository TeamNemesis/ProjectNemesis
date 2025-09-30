using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투 캐릭터의 모델 클래스
/// (체력, 공격력, 방어력 등 런타임 데이터를 관리)
/// </summary>
public class CombatCharacterModel : MonoBehaviour, IDamageable, IAttackable
{
    [Header("----- 이동 -----")]
    [SerializeField] float _moveSpeed;  // 이동 속력
    [SerializeField] float _rotSpeed;   // 회전 속력
    [SerializeField] float _dashRate;   // 이동 시 증가할 이동속도 비율

    [Header("----- 피격 -----")]
    [SerializeField] float _maxHp;      // 최대 체력
    [SerializeField] float _currentHp;  // 현재 체력
    [SerializeField] float _armor;      // 방어력

    [Header("----- 공격 -----")]
    [SerializeField] float _damage;     // 공격력

    float originalSpeed;

    public float MoveSpeed => _moveSpeed;
    public float RotSpeed => _rotSpeed;
    public Transform Transform => transform;

    public event Action<float, float> OnHpChanged;
    public event Action Ondead;
    
    /// <summary>
    /// 전투 캐릭터 모델을 초기화하는 함수
    /// </summary>
    public void Initialize()
    {
        _currentHp = _maxHp;
        OnHpChanged?.Invoke(_currentHp, _maxHp);
    }

    public void Hit(IDamageable damageable)
    {
        damageable.TakeHit(_damage);
    }

    public void TakeHit(float damage)
    {
        // 들어온 데미지에 방어력 적용
        damage = Mathf.Max(damage - _armor, 0);

        // 데미지에 따라 현재 체력 계산
        _currentHp = Mathf.Min(_currentHp - damage, _maxHp);

        // 체력 변경 이벤트 발행
        OnHpChanged?.Invoke(_currentHp, _maxHp);

        // 사망 시 이벤트 발행
        if (_currentHp <= 0)
        {
            Ondead?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        // 체력 회복량이 0 이하인 경우 0으로 설정
        amount = Mathf.Max(amount, 0);

        // 체력 회복
        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);

        // 체력 변경 이벤트 발행
        OnHpChanged?.Invoke(_currentHp, _maxHp);
    }

    /// <summary>
    /// 최대 체력을 증가시키는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddMaxHp(float amount)
    {
        // 체력 회복량이 0 이하인 경우 0으로 설정
        amount = Mathf.Max(amount, 0);

        // 최대 체력 증가
        _maxHp += amount;
        _currentHp = Mathf.Max(_currentHp, _currentHp + amount);

        // 체력 변경 이벤트 발행
        OnHpChanged?.Invoke(_currentHp, _maxHp);
    }

    /// <summary>
    /// 방어력을 증가시키는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddArmor(float amount)
    {
        _armor += amount;
    }

    /// <summary>
    /// 공격력을 증가시키는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddDamage(float amount)
    {
        _damage += amount;
    }
}
