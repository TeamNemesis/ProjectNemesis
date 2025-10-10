using UnityEngine;

public class Skill_Three : SkillBase
{
    protected override string skillBaseString { get => "skill_Three"; set => throw new System.NotImplementedException(); }
    public override void ActivateSkill(SkillData choosedSkill)
    {
        Debug.Log("3번 회사 스킬 선택");
    }
}
