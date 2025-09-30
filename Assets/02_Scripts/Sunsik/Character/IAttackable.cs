using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대상이 공격할 수 있도록 하는 인터페이스
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// 지정된 대상에게 데미지를 입히는 함수
    /// </summary>
    /// <param name="damageable">데미지를 입힐 대상</param>
    void Hit(IDamageable damageable);
}
