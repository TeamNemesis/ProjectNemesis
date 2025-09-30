using UnityEngine;

public enum HeroStateType
{
    Idle,
    Move,
    Attack1,
    Attack2,
    Attack3,
    Jump,
    Dodge,
    Dead,
    Count,
}

/// <summary>
/// Hero의 상태 클래스들의 공통 부모 추상 클래스
/// </summary>
public abstract class HeroState
{
    protected Hero _hero;       // 참조할 Hero 변수
    protected HeroStateMachine _stateMachine; // 상태 머신 참조 변수

    /// <summary>
    /// 각 상태를 나타낼 프로퍼티 변수
    /// </summary>
    public abstract HeroStateType StateType { get; }

    /// <summary>
    /// Hero 참조 변수에 등록하는 함수
    /// </summary>
    /// <param name="hero">상태를 적용할 Hero 객체(컴포넌트)</param>
    public HeroState(Hero hero, HeroStateMachine stateMachine)
    {
        _hero = hero;
        _stateMachine = stateMachine;
    }
    

    /// <summary>
    /// 상태 진입 시 호출되는 함수
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 상태 유지 시 호출되는 함수
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// 상태 종료 시 호출되는 함수
    /// </summary>
    public abstract void Exit();
}
