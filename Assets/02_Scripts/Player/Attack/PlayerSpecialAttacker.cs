using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 특수공격을 담당하는 추상 클래스
/// - 무기타입별로 상속하여 구현
/// - Initialize(owner)을 통해 owner를 주입받아 안전하게 코루틴을 실행하도록 한다.
/// - 기본적으로 차지형/즉시형 모두를 지원할 수 있는 API 제공
/// </summary>
public abstract class PlayerSpecialAttacker : MonoBehaviour
{
    public abstract WeaponType WeaponType { get; }

    // 특수공격 이벤트
    public event Action OnSpecialAttackStarted;

    protected Player _player;
    Coroutine _attackRoutine;

    // 상태 플래그
    protected bool _isAttacking = false;
    public bool CanAttack() => _isAttacking;

    public virtual void Initialize(Player player)
    {
        _player = player;
    }
    
    /// <summary>
    /// 특수공격을 시도하는 함수(상태 진입 시 호출)
    /// </summary>
    /// <returns></returns>
    public virtual bool RequestAttack()
    {
        if (!CanAttack())
        {
            Debug.Log("특수공격을 할수 없는 상태입니다.");
            return false;
        }

        // 이미 공격 중이면 중복 요청 방지
        if (_attackRoutine != null)
        {
            Debug.Log("이미 특수공격이 진행 중입니다.");
            return false;
        }
        _attackRoutine = StartCoroutine(StartAttackRoutine());
        OnSpecialAttackStarted?.Invoke();

        return true;
    }

    /// <summary>
    /// 각 무기별 특수공격 시작 코루틴
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator StartAttackRoutine();
}