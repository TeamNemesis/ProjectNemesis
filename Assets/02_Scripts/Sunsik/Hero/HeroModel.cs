using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 주인공 캐릭터의 런타임 데이터를 관리하는 클래스
/// </summary>
public class HeroModel : CombatCharacterModel
{
    [Header("----- 질주 -----")]
    [SerializeField] float _sprintRate;     // 질주 시 속력이 얼마나 빨라지는지 비율
    [SerializeField] float _dashSpeed;      // 대시 속도

    public float SprintRate => _sprintRate;
    public float DashSpeed => _dashSpeed;
}
