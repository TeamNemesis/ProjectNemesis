using UnityEngine;

public class PlayerBladeNormalAttacker : PlayerNormalAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;
    public  override void Attack()
    {
        throw new System.NotImplementedException();
    }
}