using System;
using UnityEngine;

/// <summary>
/// 비브르 모션 일반공격 강화
/// </summary>
public class Skill_One_Attack : ActiveTech
{
    public override TechTriggerType TriggerType => throw new NotImplementedException();

    public override event Action OnTechUsed;

    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        base.Activate(skillManager, player);
        player.Attack += Use;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.Attack += Use;
            }
        }
    }
    public override void Deactivate(PlayerModel player)
    {
        base.Deactivate(player);
        player.Attack -= Use;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.Attack -= Use;
            }
        }

    }

    public override void Use(Transform transform)
    {
        Debug.Log("Use " + _skillData.skillIdx);


        DebuffHandler.DebuffData poison = new DebuffHandler.DebuffData(Constants.DEBUFF_POISON,6f,5f,5);

        DebuffHandler debuffHandler = transform.GetComponent<DebuffHandler>();
        debuffHandler.ApplyDebuff(poison);
    }

    public Skill_One_Attack(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}
