using UnityEngine;

public class HeroStateIdle : HeroState
{
    public HeroStateIdle(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
    {

    }

    public override HeroStateType StateType => HeroStateType.Idle;

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        if (_hero.AttackInput)
        {
            _hero.ClearAttackInput();
            _stateMachine.ChangeState(_stateMachine.States[HeroStateType.Attack1]);
        }
    }
}
