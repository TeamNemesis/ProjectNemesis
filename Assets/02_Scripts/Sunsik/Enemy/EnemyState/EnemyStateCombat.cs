using UnityEngine;

public class EnemyStateCombat : EnemyState
{
    public override EnemyStateType StateType => EnemyStateType.Combat;
    
    public EnemyStateCombat(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }
    public override void Enter()
    {
        Debug.Log("전투 상태 진입");
    }

    public override void Update()
    {
        // 보스가 전투 상태에서 해야할 행동 구현
        if (!_enemy.IsAttacking)
        {
            if (Vector3.Distance(_enemy.transform.position, _enemy.Target.position) > _enemy.AttackRange)
            {
                _stateMachine.ChangeState(EnemyStateType.Trace);
            }

            else
            {
                _enemy.Attack();
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("전투 상태 종료");
    }
}