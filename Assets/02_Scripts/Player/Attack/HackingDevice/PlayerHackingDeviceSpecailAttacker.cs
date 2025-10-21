using UnityEngine;

public class PlayerHackingDeviceSpecailAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.HackingDevice;

    public override void SpecialAttack()
    {
        Debug.Log("해킹 디바이스 특수 공격!");
    }
}