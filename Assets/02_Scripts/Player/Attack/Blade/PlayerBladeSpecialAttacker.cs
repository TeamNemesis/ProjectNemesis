using UnityEngine;

public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;
    public override void SpecialAttack()
    {
        Debug.Log("블레이드 스페셜 공격!");
    }
}