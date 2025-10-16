using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    //TODO 방생성 알고리즘 테스트용, 추후 삭제
    public string skillBaseString;

    protected PlayerModel player;

    protected SkillManager _skillManager;

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

    public virtual void InitializeSkill(SkillManager skillManager)
    {
        Debug.Log("skill Initialize");
        ReadJsonFile();
        player = GameManager.Instance.player;
        _skillManager = skillManager;
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
    // JsonProperty("INDEX")
    private int _skillIdx;
    public int skillIdx { get { return _skillIdx; } }

    // JsonProperty("기술이름")
    private string _skillName;
    public string skillName { get { return _skillName; } }

    // JsonProperty("영문 기술이름")
    private string _skillNameEn;
    public string skillNameEn { get { return _skillNameEn; } }

    // JsonProperty("설명")
    private string _skillScript;
    public string skillScript { get { return _skillScript; } }

    // JsonProperty("영문 설명")
    private string _skillScriptEn;
    public string skillScriptEn { get { return _skillScriptEn; } }

    // JsonProperty("적용수치 설명 텍스트")
    private string _skillValueScript;
    public string skillValueScript { get { return _skillValueScript; } }

    // JsonProperty("영문 적용수치 설명 텍스트")
    private string _skillValueScriptEn;
    public string skillValueScriptEn { get { return _skillValueScriptEn; } }

    // JsonProperty("이미지경로")
    private string _skillImagePath;
    public string skillImagePath { get { return _skillImagePath; } }

    // JsonProperty("기업분류")
    private string _skillCompanyName;
    public string skillCompanyName { get { return _skillCompanyName; } }

    // JsonProperty("태그")
    private string _skillTag;
    public string skillTag { get { return _skillTag; } }

    // JsonProperty("최대레벨")
    private int? _skillMaxLevel;
    public int? skillMaxLevel { get { return _skillMaxLevel; } }

    // JsonProperty("레벨당성장수치1")
    private float? _skillLevelValue_1;
    public float? skillLevelValue_1 { get { return _skillLevelValue_1; } }

    // JsonProperty("기본밸류1")
    private float? _skillBaseValue_1;
    public float? skillBaseValue_1 { get { return _skillBaseValue_1; } }

    // JsonProperty("레벨당성장수치2")
    private float? _skillLevelValue_2;
    public float? skillLevelValue_2 { get { return _skillLevelValue_2; } }

    // JsonProperty("기본밸류2")
    private float? _skillBaseValue_2;
    public float? skillBaseValue_2 { get { return _skillBaseValue_2; } }

    // JsonProperty("레벨당성징수치3")
    private float? _skillLevelValue_3;
    public float? skillLevelValue_3 { get { return _skillLevelValue_3; } }

    // JsonProperty("기본밸류3")
    private float? _skillBaseValue_3;
    public float? skillBaseValue_3 { get { return _skillBaseValue_3; } }

    // JsonProperty("포함키워드")
    private List<string> _keywords;
    public List<string> keywords { get { return _keywords; } }

    private int _skillLevel;
    public int skillLevel { get { return _skillLevel; } }

    private SkillBase _skillCompany;
    public SkillBase skillCompany { get { return _skillCompany; } }

    public SkillData(skillJsonData data, SkillBase skillCompany)
    {
        _skillIdx = data.index;
        _skillName = data.skillName;
        _skillNameEn = data.skillNameEn;
        _skillScript = data.skillScript;
        _skillScriptEn = data.skillScriptEn;
        _skillValueScript = data.skillValueScript;
        _skillValueScriptEn = data.skillValueScriptEn;
        _skillImagePath = data.skillImagePath;
        _skillCompanyName = data.skillCompany;
        _skillTag = data.skillTag;
        _skillMaxLevel = data.skillMaxLevel;
        _skillLevelValue_1 = data.skillLevelValue_1;
        _skillBaseValue_1 = data.skillBaseValue_1;
        _skillLevelValue_2 = data.skillLevelValue_2;
        _skillBaseValue_2 = data.skillBaseValue_2;
        _skillLevelValue_3 = data.skillLevelValue_3;
        _skillBaseValue_3 = data.skillBaseValue_3;
        _keywords = string.IsNullOrEmpty(data.Keywords) ? new List<string>() : new List<string>(data.Keywords.Split(';'));

        _skillLevel = 0;
        _skillCompany = skillCompany;
    }

    public bool LevelUp()
    {
        _skillLevel++;
        return _skillLevel == 1;
    }

    public void RemoveList()
    {
        _skillLevel = 0;
        _skillCompany.currentSkillData.Remove(this);
        _skillCompany.skillList.Add(this);
    }
}




public class skillJsonData
{
    [JsonProperty("INDEX")]
    public int index;

    [JsonProperty("기술이름")]
    public string skillName;

    [JsonProperty("영문 기술이름")]
    public string skillNameEn;

    [JsonProperty("설명")]
    public string skillScript;

    [JsonProperty("영문 설명")]
    public string skillScriptEn;

    [JsonProperty("적용수치 설명텍스트")]
    public string skillValueScript;

    [JsonProperty("영문 적용수치 설명텍스트")]
    public string skillValueScriptEn;

    [JsonProperty("이미지경로")]
    public string skillImagePath;

    [JsonProperty("기업분류")]
    public string skillCompany;

    [JsonProperty("태그")]
    public string skillTag;

    [JsonProperty("최대레벨")]
    public int? skillMaxLevel;

    [JsonProperty("레벨당성장수치1")]
    public float? skillLevelValue_1;

    [JsonProperty("기본밸류1")]
    public float? skillBaseValue_1;

    [JsonProperty("레벨당성장수치2")]
    public float? skillLevelValue_2;

    [JsonProperty("기본밸류2")]
    public float? skillBaseValue_2;

    [JsonProperty("레벨당성징수치3")]
    public float? skillLevelValue_3;

    [JsonProperty("기본밸류3")]
    public float? skillBaseValue_3;

    [JsonProperty("포함키워드")]
    public string Keywords;

}




