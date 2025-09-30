using UnityEngine;

public class HeroStateAttack2 : HeroState
{
    float _timer;
    bool _canCombo;

    public HeroStateAttack2(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
    {

    }

    public override HeroStateType StateType => HeroStateType.Attack2;

    public override void Enter()
    {
        _timer = 0f;
        _canCombo = false;
        _hero.Animator.OnAttack2();
        _hero.Animator.ResetAttack3();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= 0.7f) _canCombo = true;

        if (_canCombo && _hero.AttackInput)
        {
            _hero.ClearAttackInput();
            _stateMachine.ChangeState(_stateMachine.States[HeroStateType.Attack3]);
            return;
        }

        if (_timer > 1f)
        {
            _hero.ClearAttackInput();
            _stateMachine.ChangeState(_stateMachine.States[HeroStateType.Idle]);
        }
    }
}
