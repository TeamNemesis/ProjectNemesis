using UnityEngine;

public class Skill_Two : SkillBase
{
    protected override string skillBaseString { get => "skill_Two"; set => throw new System.NotImplementedException(); }
    public override void ActivateSkill(SkillData choosedSkill)
    {
        Debug.Log("2번 회사 스킬 선택");
    }
}
