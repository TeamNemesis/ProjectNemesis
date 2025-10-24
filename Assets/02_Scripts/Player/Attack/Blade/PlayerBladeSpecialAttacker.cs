using UnityEngine;

public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;

    protected override void Fire()
    {
        throw new System.NotImplementedException();
    }
}