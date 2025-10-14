using UnityEngine;

/// <summary>
/// 플레이어의 무기타입이 총일 때의 일반 공격을 담당하는 클래스
/// </summary>
public class PlayerRangedNormalAttacker : PlayerNormalAttacker
{
    public override void Attack()
    {
        Debug.Log("원거리 일반공격 실행");
    }
}