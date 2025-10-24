using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 다양한 이벤트들의 허브 역할
/// </summary>
public static class EventBus
{
    public static event Action<WeaponType, ATTACKTYPE, Transform> OnMonsterHit;

    public static void MonsterHit(WeaponType weaponType, ATTACKTYPE attackType,Transform monster)
    {
        OnMonsterHit?.Invoke(weaponType, attackType, monster);
    }
}

public enum ATTACKTYPE
{
    NONE,
    NORMAL,
    GRENADE,
    SPECIALATTACK,
    DASH,
    COUNT
}


