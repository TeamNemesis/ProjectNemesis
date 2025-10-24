using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 다양한 이벤트들의 허브 역할
/// </summary>
public static class EventBus
{
    public static event Action<MonsterBase> OnMonsterHit;

    public static void MonsterHit(MonsterBase monster)
    {
        OnMonsterHit?.Invoke(monster);
    }
}
