using UnityEngine;

public class PlayerRangedSpecialAttacker : PlayerSpecialAttacker
{
    public override void SpecialAttack()
    {
        Debug.Log("원거리 특수공격 실행");
    }
}