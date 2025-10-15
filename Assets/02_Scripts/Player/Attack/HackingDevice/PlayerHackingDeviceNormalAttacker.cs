using UnityEngine;

public class PlayerHackingDeviceNormalAttacker : PlayerNormalAttacker
{
    public override void Attack()
    {
        Debug.Log("해킹 디바이스 일반 공격!");
    }
}
