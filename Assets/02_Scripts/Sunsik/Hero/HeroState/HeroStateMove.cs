using UnityEngine;

public class HeroStateMove : HeroState
{
    

    public HeroStateMove(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
    {
        
    }

    public override HeroStateType StateType => HeroStateType.Move;

    public override void Enter()
    {
        
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}