using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy의 상태 전이를 관리하는 상태 머신 클래스
/// </summary>
public class EnemyStateMachine 
{
    Enemy _enemy;

    EnemyStateIdle _idleStae;
    EnemyStateFirstMeet _firstMeetState;
    EnemyStateTrace _traceState;
    EnemyStateCombat _combatState;
    EnemyStateDead _deadState;

    EnemyState _currentState;
    Dictionary<EnemyStateType, EnemyState> _states = new Dictionary<EnemyStateType, EnemyState>();

    public EnemyState CurrentState => _currentState;

    public EnemyStateMachine(Enemy enemy)
    {
        _enemy = enemy;

        // 상태 객체 생성
        _idleStae = new EnemyStateIdle(_enemy, this);
        _firstMeetState = new EnemyStateFirstMeet(_enemy, this);
        _traceState = new EnemyStateTrace(_enemy, this);
        _combatState = new EnemyStateCombat(_enemy, this);
        _deadState = new EnemyStateDead(_enemy, this);

        // 딕셔너리에 상태 등록
        _states[EnemyStateType.Idle] = _idleStae;
        _states[EnemyStateType.FirstMeet] = _firstMeetState;
        _states[EnemyStateType.Trace] = _traceState;
        _states[EnemyStateType.Combat] = _combatState;
        _states[EnemyStateType.Dead] = _deadState;

        // 상태 초기화
        _currentState = _idleStae;
    }

    public void ChangeState(EnemyStateType newStateType)
    {
        _currentState.Exit();
        _currentState = _states[newStateType];
        _currentState.Enter();
    }

    public void UpdateState()
    {
        _currentState.Update();
    }
}
