using UnityEngine;

public class Skill_Five : SkillBase
{
    protected override string skillBaseString { get => "skill_Five"; set => throw new System.NotImplementedException(); }
    public override void ActivateSkill(SkillData choosedSkill)
    {
        Debug.Log("5번 회사 스킬 선택");
    }
}
