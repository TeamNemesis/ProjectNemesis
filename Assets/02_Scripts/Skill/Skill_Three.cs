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
                if (player.attackSkill != null)
                {
                    player.attackSkill.Deactivate(player);
                }
                skillAttack.Activate(player);
                break;
        }

    }
}

public class Skill_Three_Attck : ActiveTech
{
    public override TechTriggerType TriggerType => throw new NotImplementedException();

    public override event Action OnTechUsed;
    public override void Activate(PlayerModel player)
    {
        base.Activate(player);
        player.Attack += Use;
    }
    public override void Deactivate(PlayerModel player)
    {
        base.Deactivate(player);
        player.Attack -= Use;

    }
    public override void Use()
    {
        Debug.Log("Use " + _skillData.skillIdx);

    }

    public Skill_Three_Attck(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}
