using UnityEngine;

public class HeroStateAttack3 : HeroState
{
    float _timer;

    public HeroStateAttack3(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
    {

    }

    public override HeroStateType StateType => HeroStateType.Attack3;

    public override void Enter()
    {
        _timer = 0f;
        _hero.Animator.OnAttack3();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > 1f)
        {
            _hero.ClearAttackInput();
            _stateMachine.ChangeState(_stateMachine.States[HeroStateType.Idle]);
        }
    }
}