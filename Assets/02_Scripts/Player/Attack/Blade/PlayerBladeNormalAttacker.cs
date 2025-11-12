using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 3단 콤보 근거리 무기 예제
/// - 애니메이터에 Attack1/Attack2/Attack3 트리거를 만들어 두고,
///   애니메이션 이벤트로 콤보 윈도우(open/close)와 히트 타이밍(hit)을 호출한다.
/// - RequestAttack은 공격 시작 또는 콤보 큐잉 역할을 함.
/// </summary>
public class PlayerBladeNormalAttacker : PlayerNormalAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;

    public override void Attack()
    {
        throw new NotImplementedException();
    }
}