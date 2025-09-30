using UnityEngine;

public class EnemyStateIdle : EnemyState
{
    public EnemyStateIdle(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override EnemyStateType StateType => EnemyStateType.Idle;

    public override void Enter()
    {
        Debug.Log("대기 상태 진입");
    }

    public override void Exit()
    {
        Debug.Log("대기 상태 종료");
    }

    public override void Update()
    {
        _enemy.DetectTarget();
    }
}
