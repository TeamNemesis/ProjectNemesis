using UnityEngine;

/// <summary>
/// 추후 GEAR 기술 목록
/// </summary>
public class Skill_Three : SkillBase
{
    [SerializeField]
    private RedShift _redshiftPrefab;

    public override void InitializeSkill(SkillManager skillManager)
    {
        base.InitializeSkill(skillManager);
        if(_redshiftPrefab == null)
        {
            _redshiftPrefab = Resources.Load<RedShift>("Prefabs/Skill/SkillObject/Skill_Three/RedShift");
        }
    }

    public override void ActivateSkill(SkillData choosedSkill)
    {


        switch (choosedSkill.skillIdx)
        {
            // 중력자 무기
            case 30:
                ActiveTech skillAttack = new Skill_Three_Attack(choosedSkill);
                if (_skillManager.attackTech != null)
                {

                    _skillManager.attackTech.Deactivate(player, _skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                skillAttack.Activate(_skillManager, player);
                break;

            // 소용돌이
            case 31:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech SkillGrenade = new Skill_Three_Grenade(choosedSkill);
                if (_skillManager.bombTech != null)
                {

                    _skillManager.bombTech.Deactivate(player, _skillManager.bombTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                SkillGrenade.Activate(_skillManager, player);
                break;

            // 반동
            case 32:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech SkillSPAttack = new Skill_Three_SPAttack(choosedSkill);
                if (_skillManager.skillTech != null)
                {

                    _skillManager.skillTech.Deactivate(player, _skillManager.skillTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                SkillSPAttack.Activate(_skillManager, player);
                break;

            // 절대영역
            case 33:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillDashAttack = new Skill_Three_Dash(choosedSkill);
                if (_skillManager.dashTech != null)
                {

                    _skillManager.dashTech.Deactivate(player, _skillManager.dashTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                skillDashAttack.Activate(_skillManager, player);
                break;


            // 불운
            case 34:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateMisfortune(choosedSkill);
                break;

            // 적색 편이
            case 35:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateRedShift(choosedSkill);
                break;

            // 중력 증폭
            case 36:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

            // 사건의 지평선
            case 37:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;
        }

    }

    #region 불운
    private void ActivateMisfortune(SkillData skill)
    {
        //처음 습득시
        if (skill.skillLevel == 1)
        {
            skillManager.playerStatManager.AddKockBackDamageMulti(skill.skillBaseValue_1 + skill.skillLevelValue_1);
        }
        // 그 이후는 레벨 계수만 추가
        else
        {
            skillManager.playerStatManager.AddKockBackDamageMulti(skill.skillLevelValue_1);

        }
    }

    private void ActivateRedShift(SkillData skill)
    {
        skillManager.player.playerModel.PlayerHit += MakeRedshift;
    }
    private void MakeRedshift(Transform monsterTransform)
    {

    }
    #endregion

}



