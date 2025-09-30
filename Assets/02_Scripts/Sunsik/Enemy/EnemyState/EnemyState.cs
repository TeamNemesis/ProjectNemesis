using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 상태 패턴(State Pattern)
// 어떤 객체가 상태에 따라 다르게 행동할 때
// 각 상태를 객체화하여 필요에 따라 다르게 행동하도록 위임하는 디자인 패턴
// -> 행동들은 본래 클래스에 함수로 정의
// -> 상태들은 별도 클래스로 분리해서 본래 객체의 행동 함수들을 실행

/// <summary>
/// 적 캐릭터의 상태를 나타내는 열거형
/// </summary>
public enum EnemyStateType
{
    Idle,       // 방치 상태
    FirstMeet,  // 최초 조우 상태
    Trace,      // 추적 상태
    Combat,     // 전투 상태
    Dead,       // 사망 상태
    Count,      // 상태 종류 수 카운트 용
}

/// <summary>
/// 적 캐릭터 상태 클래스들의 공통 부모 추상 클래스
/// </summary>
public abstract class EnemyState
{
    protected Enemy _enemy;
    protected EnemyStateMachine _stateMachine;

    /// <summary>
    /// 상태 종류를 반환
    /// </summary>
    public abstract EnemyStateType StateType { get; }

    /// <summary>
    /// Enemy 상태 객체 생성자
    /// </summary>
    /// <param name="enemy">상태를 적용할 Enemy 객체(컴포넌트)</param>
    public EnemyState(Enemy enemy, EnemyStateMachine stateMachine)
    {
        _enemy = enemy;
        _stateMachine = stateMachine;
    }

    /// <summary>
    /// 상태 진입 시 호출되는 함수
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 상태 유지 시 매 프레임 호출되는 함수
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// 상태 종료 시 호출되는 함수
    /// </summary>
    public abstract void Exit();
}