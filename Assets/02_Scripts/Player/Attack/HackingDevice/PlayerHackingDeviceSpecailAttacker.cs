using UnityEngine;

public class PlayerHackingDeviceSpecailAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.HackingDevice;

    protected override void Fire()
    {
        throw new System.NotImplementedException();
    }
}