using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 추후 비브르 기술로 변경
/// </summary>
public class Skill_One : SkillBase
{
    

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            case 10:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillAttack = new Skill_One_Attack(choosedSkill);
                if (_skillManager.attachTech != null)
                {
                    _skillManager.attachTech.Deactivate(player);
                }
                skillAttack.Activate(_skillManager, player);
                break;
            case 11:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 12:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 13:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

            case 14:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 15:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 16:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 17:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 18:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 19:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;
        }

    }

    

}


