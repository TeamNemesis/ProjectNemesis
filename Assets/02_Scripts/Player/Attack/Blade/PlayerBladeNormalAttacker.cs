using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 3단 콤보 근거리 무기 예제
/// - 애니메이터에 Attack1/Attack2/Attack3 트리거를 만들어 두고,
///   애니메이션 이벤트로 콤보 윈도우(open/close)와 히트 타이밍(hit)을 호출한다.
/// - RequestAttack은 공격 시작 또는 콤보 큐잉 역할을 함.
/// </summary>
public class PlayerBladeNormalAttacker : PlayerNormalAttacker
{
    [Header("----- 플레이어 데이터 -----")]
    [SerializeField] int _maxCombo = 3; // 최대 콤보 수
    [SerializeField] float _comboInputWindow = 0.5f; // 콤보 입력 허용 시간 (초)

    [Header("----- 읽기 전용 -----")]
    [SerializeField] int _currentCombo = 0; // 현재 콤보 단계 (1부터 시작)
    [SerializeField] bool _waitingForNext = false; // 다음 콤보 입력 대기 중인지 여부
    [SerializeField] bool _queued = false; // 다음 콤보 입력이 큐잉되었는지 여부

    public override event Action OnAttackStarted;
    //public override event Action OnAttackEnded;

    public override WeaponType WeaponType => WeaponType.Blade;

    protected override bool CanStartAttack() => !_isAttacking || (_isAttacking && _waitingForNext);

    public void Initialize(Player player)
    {
        _player = player;
    }

    public override bool RequestAttack()
    {
        if (!CanStartAttack()) return false;

        if (!_isAttacking)
        {
            StartAttack();
        }
        else if (_waitingForNext)
        {
            // 콤보 입력 큐잉
            _queued = true;
        }
        return true;
    }

    protected override void StartAttack()
    {
        if (_isAttacking == false)
        {
            _isAttacking = true;
            _currentCombo = 1;


            Debug.Log("PlayerBladeNormalAttacker.StartAttack: Starting new combo.");
        }
        else
        {
            _isAttacking = true;
            _currentCombo++;
        }
        OnAttackStarted?.Invoke();
        PlayComboAnimation(_currentCombo);
    }

    public override void Attack()
    {
        // 콤보는 StartAttack/애니메이션 이벤트로 제어하므로 여기서는 빈 구현으로 둠
    }

    private void PlayComboAnimation(int comboIndex)
    {
        if (_player.Animator == null)
        {
            Debug.Log("PlayerBladeNormalAttacker.PlayComboAnimation: Animator is null.");
        }
        _player.Animator.OnNormalAttack();
    }

    //// 애니메이션 이벤트: 실제 히트 타이밍
    //public void Animation_OnAttackHit()
    //{
    //    OnAttackHit?.Invoke();
    //}

    // 애니메이션 이벤트: 콤보 입력을 받을 준비가 됐음을 알림
    public void Animation_OnComboWindowOpen()
    {
        _waitingForNext = true;
        StartCoroutine(ComboWindowTimer());
        Debug.Log(" PlayerBladeNormalAttacker.Animation_OnComboWindowOpen: Combo window opened. Waiting for next input.");
    }

    // 애니메이션 이벤트: 콤보 윈도우가 닫히거나 애니메이션이 끝났을 때 호출
    public void Animation_OnComboWindowClose()
    {
        _waitingForNext = false;
        if (_queued && _currentCombo < _maxCombo)
        {
            _queued = false;
            _currentCombo++;
            PlayComboAnimation(_currentCombo);
            Debug.Log("PlayerBladeNormalAttacker.Animation_OnComboWindowClose: Combo continued to " + _currentCombo);
        }
        else
        {
            EndAttack();
            Debug.Log("PlayerBladeNormalAttacker.Animation_OnComboWindowClose: Combo ended.");
        }
    }

    IEnumerator ComboWindowTimer()
    {
        float timer = 0f;
        while (timer < _comboInputWindow && _waitingForNext)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (_waitingForNext)
        {
            // 타임아웃으로 콤보 닫기
            Animation_OnComboWindowClose();
        }
    }

    
    
}