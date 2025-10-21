using System;
using UnityEngine;

public class PlayerNormalAttackState : PlayerStateBase
{
    PlayerNormalAttacker _attacker;

    public PlayerNormalAttackState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        // 현재 장착한 attacker 가져오기
        _attacker = _player.NormalAttacker;
        if (_attacker == null)
        {
            // 안전장치: attacker가 없으면 즉시 Idle 요청
            _player.SetToIdle();
            return;
        }

        // 중복 구독 방지
        _attacker.OnAttackEnded -= HandleAttackEnded;
        _attacker.OnAttackEnded += HandleAttackEnded;

        // 애니메이터 트리거(혹은 Player의 애니메이션 제어 메서드)로 애니메이션 시작
        _player.Animator.OnNormalAttack(); // 구현된 메서드가 있다면 호출, 없으면 Animator.SetTrigger("Attack") 등

        // 실제 Attack 로직 호출 (이 함수는 발사는 하지 않고 fallback 타이머만 설정하도록 설계됨)
        _attacker.Attack();
        // 또는: ((PlayerNormalAttacker)_attacker).Attack(); if Attack is public/protected accordingly
        _player.SetIsNormalAttacking(true);
    }

    public override void Update()
    {
        // 상태 내부에서 추가 로직 필요하면 처리
        // 발사(화살/총알)는 애니메이션 이벤트에서 Player.OnAttackFireEvent -> Attacker.FireNow()로 처리됨
    }

    public override void Exit()
    {
        _player.SetIsNormalAttacking(false);

        if (_attacker != null)
        {
            _attacker.OnAttackEnded -= HandleAttackEnded;
            _attacker = null;
        }
    }

    void HandleAttackEnded()
    {
        // 상태 내부에서 직접 ChangeState 하지 말고, Player에게 요청하게 하자
        
        Debug.Log("SetIsNormalttacking false in HandleAttackEnded");
        _player.SetToIdle();
    }
}