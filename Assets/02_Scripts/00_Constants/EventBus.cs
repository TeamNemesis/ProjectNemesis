using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임의 다양한 이벤트들의 허브 역할
/// </summary>
public static class EventBus
{
    /// <summary>
    /// 몬스터 적중시 이벤트 <무기 타입,공격 타입, 적 트랜스폼, 공격자 트랜스폼(플레이어, 총알 드론 등)
    /// </summary>
    public static event Action<WeaponType, ATTACKTYPE, Transform,Transform> OnMonsterHit;
    public static event Action<int> OnFailBuy;

    public static void MonsterHit(WeaponType weaponType, ATTACKTYPE attackType,Transform monster,Transform attacker)
    {
        OnMonsterHit?.Invoke(weaponType, attackType, monster,attacker);
    }

    /// <summary>
    /// 방 2개 넘어갈 시 진화 스킬 발동을 위한 이벤트
    /// </summary>
    public static event Action OnEvolution;

    public static void Evolution()
    {
        Debug.LogError("이벤트 버스 호출");
        OnEvolution?.Invoke();
    }

    public static void FailBuy(int price)
    {
        OnFailBuy?.Invoke(price);
    }

    /// <summary>
    /// 몬스터 넉백 시 발생하는 이벤트
    /// </summary>
    public static event Action<Vector3> OnMonsterKnockBack;

    public static void MonsterKnockBack(Vector3 monsterPosition)
    {
        Debug.LogError("이벤트 호출");
        OnMonsterKnockBack?.Invoke(monsterPosition);
    }

    /// <summary>
    /// 유탄 폭발 이벤트
    /// </summary>
    public static event Action<Vector3> OnGrenadeBomb;
    public static void GrenadeBomb(Vector3 bombPosition)
    {
        OnGrenadeBomb?.Invoke(bombPosition);
    }

    public static bool IsColosseumRoom = false;
    public static event Action<bool> OnColosseumRoomSet;
    public static void SetColosseumRoom(bool isColosseum)
    {
        IsColosseumRoom = isColosseum;
        Debug.Log("IsColosseumRoom set to: " + isColosseum);
        OnColosseumRoomSet?.Invoke(isColosseum);
    }

    public static MonsterBase SpawnedMonster { get; set; }
}



