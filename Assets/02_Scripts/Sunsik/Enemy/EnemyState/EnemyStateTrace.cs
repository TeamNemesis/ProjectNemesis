using UnityEngine;

public class EnemyStateTrace : EnemyState
{
    public override EnemyStateType StateType => EnemyStateType.Trace;
    public EnemyStateTrace(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override void Enter()
    {

    }

    public override void Update()
    {
        if (_enemy is ISkillUser skillUser && skillUser.CanUseSkill)
        {
            _enemy.Mover.SetMoveSpeed(0f);
            skillUser.UseSkill();
            return;
        }

        _enemy.FollowTarget();
    }

    public override void Exit()
    {

    }
}