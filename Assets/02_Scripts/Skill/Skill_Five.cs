using UnityEngine;

/// <summary>
/// LUX 강화 목록
/// </summary>
public class Skill_Five : SkillBase
{
    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            case 50:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 51:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 52:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 53:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

            case 54:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 55:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 56:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 57:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 58:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 59:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }
}
