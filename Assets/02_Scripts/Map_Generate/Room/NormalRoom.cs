using UnityEngine;

public class NormalRoom : Room
{
    public SkillBase skillCompany;

    private void Start()
    {
        SetSkillCompany();
    }

    public void SetSkillCompany()
    {
        skillCompany= GameManager.Instance().skillManager.DrawSkillCompany();
        Debug.Log(skillCompany.skillBaseString);
    }
}