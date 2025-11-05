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
            // 폭격 클러스터
            case 70:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                EventBus.HasMutant1 = true;
                break;

                // 로켓런쳐
            case 71:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                EventBus.HasMutant2 = true;

                break;

                // 살육전차
            case 72:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 유도 탄환
            case 73:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                EventBus.HasMutant3 = true;

                break;

            case 74:

                // 프로토콜 : 도미노
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 목베기
            case 75:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 에너지 구체
            case 76:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                EventBus.HasMutant4 = true;

                break;

                // 프로토콜 : 제우스
            case 77:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
           
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }

    
}
