using UnityEngine;

public interface ISkillUser
{
    public bool CanUseSkill { get; }
    public void UseSkill();
}
