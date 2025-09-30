public class EnemyStateDead : EnemyState
{
    public override EnemyStateType StateType => EnemyStateType.Dead;
    public EnemyStateDead(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }
    public override void Enter()
    {
        // 적이 죽었을 때 필요한 초기화 작업 수행
    }
    public override void Update()
    {
        // 죽은 상태에서는 특별한 업데이트 로직이 필요 없을 수 있음
        // 예: 사망 애니메이션 재생 후 오브젝트 제거 등
    }
    public override void Exit()
    {
        // 죽은 상태에서 나갈 때 필요한 작업 수행 (보통은 없음)
    }
}