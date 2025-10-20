using UnityEngine;

public class PlayerBladeNormalAttacker : PlayerNormalAttacker
{
    public override void Attack()
    {
        // 근접 일반공격 구현
        Debug.Log("블레이드 일반공격");
    }
}