using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 가지고 있는 스킬 종류
    /// </summary>
    public List<SkillData> currentSkillData = new List<SkillData>();

    /// <summary>
    /// 회사 스킬 레벨
    /// </summary>
    public List<SkillData> skillList = new List<SkillData>(10);

    public string skillDataPath;

    /// <summary>
    /// 회사 스킬 레벨 초기화
    /// </summary>
    public void InitSkillDictionary()
    {

        for (int i = 0; i < 10; i++)
        {
            skillList.Add(new SkillData(i, skillDataPath));
        }
    }

    public void ChooseSkill(SkillData skillData)
    {
        if (skillList.Remove(skillData))
        {
            currentSkillData.Add(skillData);
        }
        else
        {
            Debug.Log($"{skillData}가 없음");
        }

    }

}

public class SkillData
{
    private List<Dictionary<string, object>> skillCSVInfo;

    public int skillIdx;

    public string skillScript;

    public string skillImagePath;

    public int skillLevel;

    /// <summary>
    /// 초기화 용
    /// </summary>
    /// <param name="skillDataPath"></param>
    public SkillData(int i, string skillDataPath)
    {
        skillCSVInfo = CSVReader.Read(skillDataPath);
        skillIdx = int.Parse(skillCSVInfo[i]["IDX"].ToString());

        skillScript = skillCSVInfo[i]["SCRIPT"].ToString();
        skillImagePath = skillCSVInfo[i]["IMAGE"].ToString();

        skillLevel = 0;
        Debug.Log("초기화");
    }

    public void LevelUp()
    {
        skillLevel++;
        Debug.Log("레벨업" + skillLevel);

    }


}
