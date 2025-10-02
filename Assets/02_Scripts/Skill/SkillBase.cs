using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 가지고 있는 스킬 종류
    /// </summary>
    private List<SkillData> _currentSkillData = new List<SkillData>();
    public List<SkillData> currentSkillData { get { return _currentSkillData; } }


    /// <summary>
    /// 회사 스킬 레벨
    /// </summary>
    private List<SkillData> _skillList = new List<SkillData>(10);
    public List<SkillData> skillList { get { return _skillList; } }


    /// <summary>
    /// 스킬 데이터 경로
    /// </summary>
    [SerializeField]
    private string _skillDataPath;

    /// <summary>
    /// 회사 스킬 레벨 초기화
    /// </summary>
    public void InitSkillDictionary()
    {

        for (int i = 0; i < 10; i++)
        {
            _skillList.Add(new SkillData(i, _skillDataPath));
        }
    }

    public void ChooseSkill(SkillData skillData)
    {
        if (_skillList.Remove(skillData))
        {
            _currentSkillData.Add(skillData);
        }
        else
        {
            Debug.Log($"{skillData}가 없음");
        }

    }

}

public class SkillData
{
    private List<Dictionary<string, object>> _skillCSVInfo;

    /// <summary>
    /// 스킬 인덱스
    /// </summary>
    private int _skillIdx;
    public int skillIdx { get { return _skillIdx; } }

    /// <summary>
    /// 스킬 설명
    /// </summary>
    private string _skillScript;
    public string skillScript {  get { return _skillScript; } } 

    /// <summary>
    /// 스킬 이미지 경로
    /// </summary>
    private string _skillImagePath;
    public string skillImagePath { get { return _skillImagePath; } }


    /// <summary>
    /// 스킬 레벨
    /// </summary>
    private int _skillLevel;
    public int skillLevel { get { return _skillLevel; } }

    /// <summary>
    /// 초기화 용
    /// </summary>
    /// <param name="skillDataPath"></param>
    public SkillData(int i, string skillDataPath)
    {
        _skillCSVInfo = CSVReader.Read(skillDataPath);
        _skillIdx = int.Parse(_skillCSVInfo[i]["IDX"].ToString());

        _skillScript = _skillCSVInfo[i]["SCRIPT"].ToString();
        _skillImagePath = _skillCSVInfo[i]["IMAGE"].ToString();

        _skillLevel = 0;
        Debug.Log("초기화");
    }

    public void LevelUp()
    {
        _skillLevel++;
        Debug.Log("레벨업" + _skillLevel);

    }


}
