using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    /// <summary>
    /// 배치된 스킬 데이터
    /// </summary>
    private SkillData _skillData;
    public SkillData skillData { get { return _skillData; } }

    public void SetSkillData(SkillData setSkillData)
    {
        _skillData = setSkillData;
    }

    private Text _skillImage;
    private Text _skillScirpt;
    private Text _skillLevel;

    public void SetSkillInfo(SkillData choosedSkill, bool isPre)
    {
        _skillData = choosedSkill;
        _skillLevel.text = choosedSkill.skillLevel.ToString();
        _skillScirpt.text = choosedSkill.skillScript + isPre;
        _skillImage.text = choosedSkill.skillImagePath;
        _skillLevel.text = choosedSkill.skillIdx.ToString();
    }
}