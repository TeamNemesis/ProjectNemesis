using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public string skillBaseString;


    /// <summary>
    /// Json파일로 부터 데이터를 저장할 리스트
    /// </summary>
    private List<skillJsonData> _skillJsonDataList = new List<skillJsonData>();
    public List<skillJsonData> skillJsonDataList { get { return _skillJsonDataList; } }

    /// <summary>
    /// 아직 고르지 않은 회사 스킬 리스트
    /// </summary>
    private List<SkillData> _skillList = new List<SkillData>();
    public List<SkillData> skillList { get { return _skillList; } }


    /// <summary>
    /// 플레이어가 가지고 있는 스킬 종류
    /// </summary>
    private List<SkillData> _currentSkillData = new List<SkillData>();
    public List<SkillData> currentSkillData { get { return _currentSkillData; } }


    /// <summary>
    /// 고른 스킬의 개수 반환
    /// </summary>
    /// <returns></returns>
    public int GetNumberSkillList()
    {
        return currentSkillData.Count;
    }


    /// <summary>
    /// 스킬 데이터 경로
    /// </summary>
    [SerializeField]
    private string _skillDataPath;

    public void Start()
    {
        ReadJsonFile();
    }

    public void ReadJsonFile()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(_skillDataPath);
        if (jsonFile == null)
        {
            Debug.Log("오류 : 파일 없음 " + _skillDataPath);
            return;
        }
        else
        {
            string jsonText = jsonFile.text;
            _skillJsonDataList = JsonConvert.DeserializeObject<List<skillJsonData>>(jsonText);
            InitSkillDictionary();
        }

    }

    /// <summary>
    /// 회사 스킬 레벨 초기화
    /// </summary>
    public void InitSkillDictionary()
    {
        for (int i = 0; i < skillJsonDataList.Count; i++)
        {
            _skillList.Add(new SkillData(skillJsonDataList[i],this));
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

    /// <summary>
    /// 스킬 선택시 발동
    /// </summary>
    /// <param name="choosedSkill"></param>
    public abstract void ActivateSkill(SkillData choosedSkill);


}

public class SkillData
{

    /// <summary>
    /// 스킬 인덱스
    /// </summary>
    private int _skillIdx;
    public int skillIdx { get { return _skillIdx; } }

    /// <summary>
    /// 스킬 설명
    /// </summary>
    private string _skillScript;
    public string skillScript { get { return _skillScript; } }

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
    /// 해당 스킬 소속 회사
    /// </summary>
    private SkillBase _skillCompany;
    public SkillBase skillCompany { get { return _skillCompany; } }
    /// <summary>
    /// 초기화 용
    /// </summary>
    /// <param name="skillDataPath"></param>
    public SkillData(skillJsonData data,SkillBase skillCompany)
    {
        _skillIdx = data.IDX;
        _skillScript = data.SCRIPT;
        _skillImagePath = data.IMAGE;

        _skillLevel = 0;
        _skillCompany = skillCompany;
    }

    public bool LevelUp()
    {
        _skillLevel++;
        if (_skillLevel == 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


}



public class skillJsonData
{
    public int IDX;
    public string SCRIPT;
    public string IMAGE;
}


