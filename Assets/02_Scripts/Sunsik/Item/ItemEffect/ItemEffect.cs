using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 효과 베이스 클래스
/// 설정 데이터 + 적용 동작.
/// </summary>
public abstract class ItemEffect : ScriptableObject
{
    /// <summary>
    /// 아이템 효과를 적용하는 함수
    /// </summary>
    /// <param name="inventory"></param>
    public virtual void Apply(Inventory inventory)
    {
    }

    /// <summary>
    /// 아이템 효과를 해제하는 함수
    /// </summary>
    /// <param name="inventory"></param>
    public virtual void Disapply(Inventory inventory)
    {
    }
}
