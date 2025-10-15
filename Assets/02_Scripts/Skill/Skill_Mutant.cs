using UnityEngine;

/// <summary>
/// 돌연변이 스킬
/// </summary>
public class Skill_Mutant : SkillBase
{
    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            case 70:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 71:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 72:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 73:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

            case 74:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 75:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 76:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 77:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 78:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 79:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }

    
}
