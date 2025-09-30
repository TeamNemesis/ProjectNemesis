using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 영웅의 상태머신을 담당하는 클래스
/// </summary>
public class HeroStateMachine
{
    Hero _hero;
    
    HeroStateIdle _idleState;
    HeroStateAttack1 _attackState1;
    HeroStateAttack2 _attackState2;
    HeroStateAttack3 _attackState3;
    HeroStateMove _moveState;

    HeroState _currentState;
    Dictionary<HeroStateType, HeroState> _states = new();

    public HeroState CurrentState => _currentState;
    public Dictionary<HeroStateType, HeroState> States => _states;

    public HeroStateMachine(Hero hero)
    {
        _hero = hero;

        // 상태 객체 생성
        _idleState = new HeroStateIdle(_hero, this);
        _attackState1 = new HeroStateAttack1(_hero, this);
        _attackState2 = new HeroStateAttack2(_hero, this);
        _attackState3 = new HeroStateAttack3(_hero, this);
        _moveState = new HeroStateMove(_hero, this);

        // 딕셔너리에 상태 등록
        _states[HeroStateType.Idle] = _idleState;
        _states[HeroStateType.Attack1] = _attackState1;
        _states[HeroStateType.Attack2] = _attackState2;
        _states[HeroStateType.Attack3] = _attackState3;
        _states[HeroStateType.Move] = _moveState;

        // 상태 초기화
        _currentState = _idleState;
    }

    public void ChangeState(HeroState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void UpdateState()
    {
        _currentState.Update();
    }
}
