using UnityEngine;

public class Skill_Four : SkillBase
{
    protected override string skillBaseString { get => "skill_Four"; set => throw new System.NotImplementedException(); }
    public override void ActivateSkill(SkillData choosedSkill)
    {
        Debug.Log("4번 회사 스킬 선택");
    }
}
