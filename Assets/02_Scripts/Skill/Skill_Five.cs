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
            // 약화 약물
            case 50:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillAttack = new Skill_Five_Attack(choosedSkill);
                if (_skillManager.attackTech != null)
                {
                    _skillManager.attackTech.Deactivate(player, _skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx);
                }
                skillAttack.Activate(_skillManager, player);
                break;

                // 환각 구름
            case 51:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillGrenade = new Skill_Five_Grenade(choosedSkill);
                if (_skillManager.bombTech != null)
                {
                    _skillManager.bombTech.Deactivate(player, _skillManager.bombTech.skillData.skillIdx != choosedSkill.skillIdx);
                }
                skillGrenade.Activate(_skillManager, player);
                break;

                // 섬망
            case 52:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillSPAttack = new Skill_Five_SPAttack(choosedSkill);
                if (_skillManager.skillTech != null)
                {
                    _skillManager.skillTech.Deactivate(player, _skillManager.skillTech.skillData.skillIdx != choosedSkill.skillIdx);
                }
                skillSPAttack.Activate(_skillManager, player);
                break;

                // 아드레날린 레이스
            case 53:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillDash = new Skill_Five_Dash(choosedSkill);
                if (_skillManager.dashTech != null)
                {
                    _skillManager.dashTech.Deactivate(player, _skillManager.dashTech.skillData.skillIdx != choosedSkill.skillIdx);
                }
                skillDash.Activate(_skillManager, player);
                break;


                // 약자 도태
            case 54:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 불릿 타임
            case 55:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 케타민 드리프트
            case 56:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 정제
            case 57:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                
           
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }
}
