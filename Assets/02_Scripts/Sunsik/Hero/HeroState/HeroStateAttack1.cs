using UnityEngine;

public class HeroStateAttack1 : HeroState
{
    float _timer;           // 공격에 걸리는 애니메이션 시간을 측정하기 위한 변수
    bool _canCombo;         // 연속 공격이 가능한지를 판단하는 플래그

    public HeroStateAttack1(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
    {

    }

    public override HeroStateType StateType => HeroStateType.Attack1;

    public override void Enter()
    {
        _timer = 0f;
        _canCombo = false;

        // 공격 가능 토큰을 하나 소모했으니까 초기화
        _hero.ClearAttackInput();
        _hero.Animator.OnAttack1();
        _hero.Animator.ResetAttack2();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        // 콤보 가능 플래그 ON
        if (_timer >= 0.7f) _canCombo = true;

        // 콤보 가능한 상태일 때 공격 가능하다는 토큰을 받았다면
        if (_canCombo && _hero.AttackInput)
        {
            // 공격 토큰 써서 공격할거니까 다시 초기화해주고
            _hero.ClearAttackInput();
            // 두번째 공격콤보 상태로 전환
            _stateMachine.ChangeState(_stateMachine.States[HeroStateType.Attack2]);
            return;
        }

        // 콤보 가능한 상태이지만 토큰을 못받은채로 1초가 지났으면
        if (_timer > 1f)
        {
            // 다시 Idle 상태로 초기화
            _stateMachine.ChangeState(_stateMachine.States[HeroStateType.Idle]);    
        }
    }
}