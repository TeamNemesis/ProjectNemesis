using System;
using UnityEngine;

/// <summary>
/// 추후 GEAR 기술 목록
/// </summary>
public class Skill_Three : SkillBase
{

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            
        }


        switch (choosedSkill.skillIdx)
        {
            case 30:
                ActiveTech skillAttack = new Skill_Three_Attck(choosedSkill);
                if (_skillManager.attachTech != null)
                {
                    _skillManager.attachTech.Deactivate(player);
                }
                skillAttack.Activate(_skillManager, player);
                break;
            case 31:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 32:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 33:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

            case 34:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 35:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 36:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 37:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 38:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 39:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
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
