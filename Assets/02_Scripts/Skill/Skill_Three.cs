using System;
using UnityEngine;

public class Skill_Three : SkillBase
{

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            case 30:
                ActiveTech skillAttack = new Skill_Three_Attck(choosedSkill);
                if (_skillManager.attachTech != null)
                {
                    _skillManager.attachTech.Deactivate(player);
                }
                skillAttack.Activate(_skillManager,player);
                break;
        }

    }
}

public class Skill_Three_Attck : ActiveTech
{
    public override TechTriggerType TriggerType => throw new NotImplementedException();

    public override event Action OnTechUsed;
    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        base.Activate(skillManager,player);
        player.Attack += Use;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if(drones.Length > 0)
        {
            foreach(Drone drone in drones)
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

    }

    public Skill_Three_Attck(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}
