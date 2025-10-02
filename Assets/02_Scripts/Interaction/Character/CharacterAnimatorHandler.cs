using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 애니메이션 이벤트(공격 판정 타이밍)을 처리하는 클래스
/// </summary>
public class CharacterAnimatorHandler : MonoBehaviour
{
    /// <summary>
    /// 공격 판정 이벤트
    /// </summary>
    public event Action OnAttacked;

    public void InvokeOnAttacked()
    {
        OnAttacked?.Invoke();
    }
}
