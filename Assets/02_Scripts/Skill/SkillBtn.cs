using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public SkillBase skillCompany;
    public SkillData skillData;

    public int skillIdx;

    public Text skillImage;
    public Text skillScirpt;
    public Text skillLevel;

    public void SetSkillInfo(SkillData choosedSkill, SkillBase skillCompany, bool isPre)
    {
        skillData = choosedSkill;
        this.skillCompany = skillCompany;
				skillLevel.text = choosedSkill.skillLevel.ToString();
				skillScirpt.text = skillCompany.GetType().Name + choosedSkill._skillScript + isPre;
				skillImage.text = choosedSkill._skillImagePath;
			  skillIdx = choosedSkill._skillIdx;
		}
}