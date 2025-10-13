using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    
    private Skill_One _skill_One;
    public Skill_One skill_One { get { return _skill_One; } }

    private Skill_Two _skill_Two;
    public Skill_Two skill_Two { get { return _skill_Two; } }

    private Skill_Three _skill_Three;
    public Skill_Three skill_Three { get { return _skill_Three; } }

    private Skill_Four _skill_Four;
    public Skill_Four skill_Four { get { return _skill_Four; } }

    private Skill_Five _skill_Five;
    public Skill_Five skill_Five { get { return _skill_Five; } }

    private Skill_Collab _skill_Collab;
    public Skill_Collab skill_Collab { get { return _skill_Collab; } }

    [SerializeField]
    private SkillBtn _skillBtnPrefab;

    [SerializeField]
    private Text _skillImageText;
    [SerializeField]
    private Text _skillScriptText;
    [SerializeField]
    private Text _skillLevelText;

    public void Awake()
    {
      

        _skill_One = GetComponent<Skill_One>();
        _skill_Two = GetComponent<Skill_Two>();
        _skill_Three = GetComponent<Skill_Three>();
        _skill_Four = GetComponent<Skill_Four>();
        _skill_Five = GetComponent<Skill_Five>();
        _skill_Collab = GetComponent<Skill_Collab>();

    }

    /// <summary>
    /// 고른 스킬 리스트 순회해서 버튼으로 보여줌
    /// </summary>
    public void CheckChooseSkillList()
    {
        GameObject parentContent = GameObject.Find("Content");

        if (_skill_One.GetNumberSkillList() != 0)
        {
            foreach (SkillData skillData in _skill_One.currentSkillData)
            {
                MakeSkillBtn(skillData, parentContent.transform);
            }
        }

        if (_skill_Two.GetNumberSkillList() != 0)
        {
            foreach (SkillData skillData in _skill_Two.currentSkillData)
            {
                MakeSkillBtn(skillData, parentContent.transform);
            }

        }

        if (_skill_Three.GetNumberSkillList() != 0)
        {
            foreach (SkillData skillData in _skill_Three.currentSkillData)
            {
                MakeSkillBtn(skillData, parentContent.transform);
            }
        }

        if (_skill_Four.GetNumberSkillList() != 0)
        {
            foreach (SkillData skillData in _skill_Four.currentSkillData)
            {
                MakeSkillBtn(skillData, parentContent.transform);
            }
        }

        if (_skill_Five.GetNumberSkillList() != 0)
        {
            foreach (SkillData skillData in _skill_Five.currentSkillData)
            {
                MakeSkillBtn(skillData, parentContent.transform);
            }
        }

        if (_skill_Collab.GetNumberSkillList() != 0)
        {
            foreach (SkillData skillData in _skill_Collab.currentSkillData)
            {
                MakeSkillBtn(skillData, parentContent.transform);
            }
        }

    }

    /// <summary>
    /// 버튼 생성 함수
    /// </summary>
    /// <param name="skillData"></param>
    /// <param name="parentContent"></param>
    public void MakeSkillBtn(SkillData skillData, Transform parentContent)
    {
        SkillBtn skillBtn = Instantiate(_skillBtnPrefab, parentContent);
        skillBtn.SetSkillInfo(skillData);
        skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillListBtn(skillBtn));

    }

    /// <summary>
    /// 확률에 따른 스킬 회사 뽑기
    /// </summary>
    /// <returns></returns>
    public SkillBase DrawSkillCompany()
    {
        int skillOneNum = _skill_One.GetNumberSkillList() + 1;
        int skillTwoNum = _skill_Two.GetNumberSkillList() + 1;
        int skillThreeNum = _skill_Three.GetNumberSkillList() + 1;
        int skillFourNum = _skill_Four.GetNumberSkillList() + 1;
        int skillFiveNum = _skill_Five.GetNumberSkillList() + 1;

        int totalNum = skillOneNum + skillTwoNum + skillThreeNum + skillFourNum + skillFiveNum;


        // 임시 숫자를 저장할 변수
        int tempNum = Random.Range(0, totalNum);


        Debug.Log("TempNum : " + tempNum);
        if (0 <= tempNum && tempNum < skillOneNum)
        {
            return _skill_One;
        }
        else if (tempNum < skillOneNum + skillTwoNum)
        {
            return _skill_Two;
        }
        else if (tempNum < skillOneNum + skillTwoNum + skillThreeNum)
        {
            return _skill_Three;
        }
        else if (tempNum < skillOneNum + skillTwoNum + skillThreeNum + skillFourNum)
        {
            return _skill_Four;
        }
        else
        {
            return _skill_Five;
        }

    }

    /// <summary>
    /// 현재 가지고 있는 스킬 총 개수
    /// </summary>
    /// <returns></returns>
    public int GetTotalSkillNumber()
    {
        int skillOneNum = _skill_One.GetNumberSkillList();
        int skillTwoNum = _skill_Two.GetNumberSkillList();
        int skillThreeNum = _skill_Three.GetNumberSkillList();
        int skillFourNum = _skill_Four.GetNumberSkillList();
        int skillFiveNum = _skill_Five.GetNumberSkillList();

        return skillOneNum + skillTwoNum + skillThreeNum + skillFourNum + skillFiveNum;
    }

    public void OnClick_SkillListBtn(SkillBtn skillBtn)
    {
        _skillImageText.text = skillBtn.skillData.skillImagePath;
        _skillScriptText.text = skillBtn.skillData.skillIdx.ToString() + "\n" + skillBtn.skillData.skillScript;
        _skillLevelText.text = skillBtn.skillData.skillLevel.ToString();
    }

    /// <summary>
    /// 해당 회사의 콜라보 스킬 해금 조건이 만족되었는지 판단
    /// </summary>
    /// <param name="skillCompany"></param>
    public bool CheckCollabo(SkillBase skillCompany, out List<int> indexList)
    {
        indexList = new List<int>();
        bool bCheck = false;
        // 매개 변수의 회사가 만족하는지 판단
        if (skillCompany.currentSkillData.Count < Constants.COLLABCNT)
        {
            indexList = null;
            return bCheck;
        }

        if (skillCompany == _skill_One)
        {
            // 관련 회사 조건 검사
            if (_skill_Two.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 201);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }

            if (_skill_Five.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 105);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }
        }
        else if (skillCompany == _skill_Two)
        {
            // 관련 회사 조건 검사
            if (_skill_Three.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 302);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }

            if (_skill_One.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 201);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }
        }
        else if (skillCompany == _skill_Three)
        {
            // 관련 회사 조건 검사
            if (_skill_Four.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 403);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }

            if (_skill_Two.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 302);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }
        }
        else if (skillCompany == _skill_Four)
        {
            // 관련 회사 조건 검사
            if (_skill_Five.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 504);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }

            if (_skill_Three.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 403);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }
        }
        else if (skillCompany == _skill_Five)
        {
            // 관련 회사 조건 검사
            if (_skill_Four.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 504);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }

            if (_skill_One.currentSkillData.Count >= Constants.COLLABCNT)
            {
                int index = _skill_Collab.skillList.FindIndex(skillData => skillData.skillIdx == 105);
                if (index != Constants.NOCONTAININDEX)
                {
                    indexList.Add(index);
                }

                bCheck = true;
            }
        }
            return bCheck;
    }

}
