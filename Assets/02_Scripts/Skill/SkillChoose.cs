using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoose : MonoBehaviour
{
    /// <summary>
    /// 임시 스킬 인덱스 보관 리스트
    /// </summary>
    private List<int> _tempSkillList = new List<int>();

    /// <summary>
    /// 무한루프 방지 int
    /// </summary>
    private int loopCnt;

    /// <summary>
    /// 뽑을 스킬 회사
    /// </summary>
    private SkillBase _skillCompany;
    public SkillBase skillCompany { get { return _skillCompany; } }
    public void SetSkillComapany(SkillBase skill)
    {
        _skillCompany = skill;
    }


    [SerializeField]
    private GameObject _skillBtnPanel;
    [SerializeField]
    private GameObject _parentPanel;
    [SerializeField]
    private SkillBtn _skillBtnPrefab;


    /// <summary>
    /// 초기 스킬 선택 버튼 생성
    /// </summary>
    public void SetBtn(SkillBase skillComapany)
    {
        _skillCompany = skillComapany;
        int skillNum = _skillCompany.skillList.Count;
        List<int> indexList;
        bool bCheckCollab = SkillManager.Instance().CheckCollabo(_skillCompany, out indexList);
        if (bCheckCollab && indexList.Count>0)
        {
            skillNum += indexList.Count;
        }

        _skillBtnPanel.SetActive(true);

        if (_skillCompany == null || skillNum == 0)
        {
            Debug.Log("Error");
            _skillBtnPanel.SetActive(false);
            return;
        }

        Debug.Log(skillNum);
        for (int i = 0; i < Mathf.Min(Constants.SKILLCNT, skillNum); i++)
        {
            int tempNum = 0;
            // 임시 인트
            do
            {
                _skillCompany = skillComapany;
                tempNum = Random.Range(0, 100);
                loopCnt++;
                if (bCheckCollab && indexList.Count>0 && tempNum < Constants.COLLABPER)
                {
                    _skillCompany = SkillManager.Instance().skill_Collab;
                    tempNum = Random.Range(0,indexList.Count);
                    tempNum = indexList[tempNum];
                }
                else
                {
                    tempNum = Random.Range(0, _skillCompany.skillList.Count);

                }
            }
            while (SetSkillBtn(tempNum) && loopCnt < Constants.LOOPCNT);
            loopCnt = 0;
        }

        _tempSkillList.Clear();
    }

    /// <summary>
    /// 스킬 업그레이드 버튼 생성
    /// </summary>
    public void SetUpgradeBtn()
    {
        _skillBtnPanel.SetActive(true);



        if (_skillCompany == null || SkillManager.Instance().GetTotalSkillNumber() == 0)
        {
            Debug.Log("Error");
            _skillBtnPanel.SetActive(false);
            return;
        }

        for (int i = 0; i < Mathf.Min(Constants.SKILLCNT, SkillManager.Instance().GetTotalSkillNumber()); i++)
        {

            int tempNum = 0;
            // 임시 인트
            do
            {
                _skillCompany = SkillManager.Instance().DrawSkillCompany();

                while (_skillCompany.currentSkillData.Count == 0 || loopCnt > 10)
                {
                    _skillCompany = SkillManager.Instance().DrawSkillCompany();

                }
                Debug.Log(_skillCompany.currentSkillData.Count + _skillCompany.name);
                tempNum = Random.Range(0, _skillCompany.currentSkillData.Count);

                loopCnt++;
            }
            while (SetUpgradeSkillBtn(tempNum) && loopCnt < Constants.LOOPCNT);
            Debug.Log("Loop : " + loopCnt);
            loopCnt = 0;
        }

        _tempSkillList.Clear();

    }


    /// <summary>
    /// 버튼에 스킬 정보 세팅
    /// </summary>
    public bool SetSkillBtn(int skillNum)
    {
        // 예외처리
        if(skillNum > _skillCompany.skillList.Count)
        {
            return true;
        }
        if (!_tempSkillList.Contains(_skillCompany.skillList[skillNum].skillIdx))
        {
            SkillBtn skillBtn = Instantiate(_skillBtnPrefab, _parentPanel.transform);
            skillBtn.SetSkillInfo(_skillCompany.skillList[skillNum]);
            skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillBtnClick(skillBtn));
            _tempSkillList.Add(skillBtn.skillData.skillIdx);
            return false;
        }
        else return true;
    }

    public bool SetUpgradeSkillBtn(int skillNum)
    {
        if (!_tempSkillList.Contains(_skillCompany.currentSkillData[skillNum].skillIdx))
        {
            SkillBtn skillBtn = Instantiate(_skillBtnPrefab, _parentPanel.transform);
            skillBtn.SetSkillInfo(_skillCompany.currentSkillData[skillNum]);
            skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillBtnClick(skillBtn));
            _tempSkillList.Add(skillBtn.skillData.skillIdx);
            return false;
        }
        else return true;
    }

    /// <summary>
    /// 스킬 버튼 선택
    /// </summary>
    public void OnClick_SkillBtnClick(SkillBtn skillBtn)
    {
        if (skillBtn.skillData.LevelUp())
        {
            skillBtn.skillData.skillCompany.ChooseSkill(skillBtn.skillData);

        }
        skillBtn.skillData.skillCompany.ActivateSkill(skillBtn.skillData);

        foreach (Transform child in _parentPanel.transform)
        {
            Destroy(child.gameObject);
        }


        _skillBtnPanel.SetActive(false);
    }


    /// <summary>
    /// 후에 삭제, 테스트용 함수들
    /// </summary>
    #region testBtn
    public void OnClick_DrawSkillCompany()
    {
        SetSkillComapany(SkillManager.Instance().DrawSkillCompany());
    }

    public void OnClick_skillCompanyOne(Skill_One skillCompany)
    {
        SetSkillComapany(skillCompany);
    }

    public void OnClick_skillCompanyTwo(Skill_Two skillCompany)
    {
        SetSkillComapany(skillCompany);
    }

    public void OnClick_skillCompanyThree(Skill_Three skillCompany)
    {
        SetSkillComapany(skillCompany);
    }

    public void OnClick_skillCompanyFour(Skill_Four skillCompany)
    {
        SetSkillComapany(skillCompany);
    }

    public void OnClick_skillCompanyFive(Skill_Five skillCompany)
    {
        SetSkillComapany(skillCompany);
    }




    #endregion
}
