using System;
using UnityEngine;

public class Skill_Two : SkillBase
{

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch(choosedSkill.skillIdx)
        {
            case 20:
               ActiveTech skillAttack =  new Skill_Two_Attck(choosedSkill);
                if(_skillManager.attachTech!=null)
                {
                    _skillManager.attachTech.Deactivate(player);
                }
                skillAttack.Activate(_skillManager,player);
                break;
        }

    }
}

public class Skill_Two_Attck : ActiveTech
{
    public override TechTriggerType TriggerType => throw new NotImplementedException();

    public override event Action OnTechUsed;

    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        base.Activate(skillManager,player);
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
    public override void Deactivate( PlayerModel player)
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


        DebuffHandler.DebuffData poison = new DebuffHandler.DebuffData(Constants.DEBUFF_POISON, 6f, 5f);

        

        DebuffHandler debuffHandler = transform.GetComponent<DebuffHandler>();
        debuffHandler.ApplyDebuff(poison);
    }

    public Skill_Two_Attck(SkillData choosedSkill):base(choosedSkill)
    {
        
    }
}
