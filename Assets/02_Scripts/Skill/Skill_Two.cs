using System;
using UnityEngine;

/// <summary>
/// 추후 파이로 하트 모션
/// </summary>
public class Skill_Two : SkillBase
{

    public override void ActivateSkill(SkillData choosedSkill)
    {

        switch (choosedSkill.skillIdx)
        {
            case 20:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                
                break;
            case 21:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 22:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 23:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

            case 24:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 25:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 26:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 27:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 28:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 29:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }
}


