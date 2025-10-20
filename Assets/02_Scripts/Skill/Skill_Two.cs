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
            // 베고, 찌르고, 부수고
            case 20:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                
                break;

                // 폭발!
            case 21:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 비밀무기
            case 22:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 깜짝선물
            case 23:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

                // 전투장비 강화
            case 24:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 폭격 클러스터
            case 25:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 폭사
            case 26:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 기폭제
            case 27:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                //TODO 범위 스킬 구현에 대한 계수 회의 필요
                // 범위 증가
                Constants.SKILL_EXTENT = 1.1f;
                break;
            
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }
}


