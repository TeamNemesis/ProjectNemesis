using UnityEngine;

public class Skill_Collab : SkillBase
{
    /// <summary>
    /// 콜라보 스킬의 경우 해당하는 회사 개수 up
    /// </summary>
    /// <param name="skilldata"></param>
    /// <param name="num"></param>
    public override void SkillNumUp(SkillData skilldata,int num)
    {
        switch(skilldata.skillIdx)
        {
            case Constants.INDEX_FIVE_ONE:
                _skillManager.skill_One.SkillNumUp(skilldata,num);
                _skillManager.skill_Five.SkillNumUp(skilldata,num);
                Debug.Log("collab" + _skillManager.skill_One.skillNum);
                break;
            case Constants.INDEX_ONE_TWO:
                _skillManager.skill_One.SkillNumUp(skilldata, num);
                _skillManager.skill_Two.SkillNumUp(skilldata,num);
                break;
            case Constants.INDEX_TWO_THREE:
                _skillManager.skill_Two.SkillNumUp(skilldata, num);
                _skillManager.skill_Three.SkillNumUp(skilldata,num);
                break;
            case Constants.INDEX_THREE_FOUR:
                _skillManager.skill_Three.SkillNumUp(skilldata, num);
                _skillManager.skill_Four.SkillNumUp(skilldata, num);
                break;
            case Constants.INDEX_FOUR_FIVE:
                _skillManager.skill_Five.SkillNumUp(skilldata, num);
                _skillManager.skill_Four.SkillNumUp(skilldata, num);
                break;
        }
        
    }


    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            // 폭군
            case 60:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 최상위 포식자
            case 61:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // GravityFlare
            case 62:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 전자기 폭풍
            case 63:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

                // 전기 인간
            case 64:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
        
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }
}
