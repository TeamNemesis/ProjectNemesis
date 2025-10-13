using System;
using UnityEngine;

public enum TechTriggerType
{
    OnNormalAttack,            // 일반공격이 발동될 때
    OnNormalAttackHit,         // 일반공격이 적중했을 때
    OnSpecialAttack,           // 특수공격이 발동될 때
    OnSpecialAttackHit,        // 특수공격이 적중했을 때
    OnGrenadeAttack,           // 유탄공격이 발동될 때
    OnGrenadeAttackHit,        // 유탄공격이 적중했을 때
    OnDashStart,               // 대시가 시작될 때
    OnDashAfterSeconds,        // 대시 시작 후 n초 뒤
}

/// <summary>
/// 플레이어가 획득하는 액티브형 기술 기본 클래스
/// </summary>
public abstract class ActiveTech : MonoBehaviour
{
    [Header("기술 정보")]
    [SerializeField] protected string techName; // 기술의 이름
    [SerializeField] protected string techDescription; // 기술의 설명
    [SerializeField] protected Sprite techIcon; // 기술의 아이콘 이미지

    [SerializeField] protected int techLevel; // 기술의 레벨

    public abstract TechTriggerType TriggerType { get; } // 기술이 발동되는 조건을 나타내는 속성
    public string TechName => techName; // 기술의 이름을 반환하는 속성
    public string TechDescription => techDescription; // 기술의 설명을 반환하는 속성
    public Sprite TechIcon => techIcon; // 기술의 아이콘 이미지를 반환하는 속성
    public int TechLevel => techLevel; // 기술의 레벨을 반환하는 속성

    public virtual event Action<ActiveTech> OnTechAdded; // 기술이 추가될 때 발생하는 이벤트
    public virtual event Action<ActiveTech> OnTechRemoved; // 기술이 제거될 때 발생하는 이벤트
    public abstract event Action OnTechUsed;  // 기술이 사용될 때 발생하는 이벤트

    /// <summary>
    /// 기술이 추가될 때 호출되는 메서드
    /// </summary>
    public virtual void Activate()
    {
        OnTechAdded?.Invoke(this);
    }

    /// <summary>
    /// 기술이 제거될 때 호출되는 메서드
    /// </summary>
    public virtual void Deactivate()
    {
        OnTechRemoved?.Invoke(this);
    }

    /// <summary>
    /// 기술이 사용될 때 호출되는 메서드
    /// </summary>
    public abstract void Use();
}
