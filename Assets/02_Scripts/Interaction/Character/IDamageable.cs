using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대상이 피해를 받을 수 있도록 하는 인터페이스
/// </summary>
public interface IDamageable
{
    public Transform Transform { get; }

    /// <summary>
    /// 체력이 변경될 때 호출되는 이벤트(현재 체력, 최대 체력)
    /// </summary>
    event Action<float, float> OnHpChanged;

    /// <summary>
    /// 캐릭터가 사망할 때 호출되는 이벤트
    /// </summary>
    event Action Ondead;

    /// <summary>
    /// 피해를 입는 함수
    /// </summary>
    /// <param name="damage">데미지 양</param>
    void TakeHit(float damage);
}