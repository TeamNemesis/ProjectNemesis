using System;

public interface IActiveTech
{
    event Action<TechTriggerType> OnTechAdded; // 기술이 추가될 때 발생하는 이벤트
    event Action<TechTriggerType> OnTechRemoved; // 기술이 제거될 때 발생하는 이벤트
    event Action OnTechUsed;  // 기술이 사용될 때 발생하는 이벤트
    /// <summary>
    /// 기술이 추가될 때 호출되는 메서드
    /// </summary>
    void Activate();
    /// <summary>
    /// 기술이 제거될 때 호출되는 메서드
    /// </summary>
    void Deactivate();
    /// <summary>
    /// 기술이 사용될 때 호출되는 메서드
    /// </summary>
    void Use();
}