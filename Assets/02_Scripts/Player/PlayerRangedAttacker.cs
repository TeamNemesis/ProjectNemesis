using UnityEngine;

public class PlayerRangedAttacker : PlayerAttacker
{
    public override void Attack()
    {
        Debug.Log("PlayerRangedAttacker의 원거리 공격 실행");
    }
}