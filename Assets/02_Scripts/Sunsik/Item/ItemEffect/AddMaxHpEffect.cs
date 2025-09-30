using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최대 Hp를 증가시키는 패시브 아이템 효과
/// </summary>
[CreateAssetMenu(fileName = "AddMaxHpEffect", menuName = "GameSettings/ItemEffect/AddMaxHp")]
public class AddMaxHpEffect : ItemEffect
{
    [Header("----- 최대체력 증가량 -----")]
    [SerializeField] float _amount;

    public float Amount => _amount;

    public override void Apply(Inventory inventory)
    {
        inventory.HeroModel.AddMaxHp(_amount);
    }

    public override void Disapply(Inventory inventory)
    {
        inventory.HeroModel.AddMaxHp(-_amount);
    }
}
