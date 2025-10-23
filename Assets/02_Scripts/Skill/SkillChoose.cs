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
    public void SetSkillCompany(TechSelectPackType techCompany)
    {
				switch (techCompany)
				{
						case TechSelectPackType.Company1:
								_skillCompany =  GameManager.Instance.skillManager.skill_One;
								break;
						case TechSelectPackType.Company2:
								_skillCompany = GameManager.Instance.skillManager.skill_Two;

								break;
						case TechSelectPackType.Company3:
								_skillCompany = GameManager.Instance.skillManager.skill_Three;

								break;
						case TechSelectPackType.Company4:
								_skillCompany = GameManager.Instance.skillManager.skill_Four;

								break;
						case TechSelectPackType.Company5:
								_skillCompany = GameManager.Instance.skillManager.skill_Five;

								break;
				}

		}

		#region chooseSkill

		/// <summary>
		/// 초기 스킬 선택 버튼 생성
		/// </summary>
		public void SetBtn()
    {
        // 콜라보 스킬에서 복원을 위한 임시 보관
        SkillBase tempSkillCompany = _skillCompany;
        int skillNum = _skillCompany.skillList.Count;
        List<int> indexList;
        bool bCheckCollab = GameManager.Instance.skillManager.CheckCollabo(_skillCompany, out indexList);
        if (bCheckCollab && indexList.Count>0)
        {
            skillNum += indexList.Count;
        }

        GameManager.Instance.UIManager.SetActiveSkillBtnPanel(true);

        if (_skillCompany == null || skillNum == 0)
        {
            Debug.Log("Error");
            GameManager.Instance.UIManager.SetActiveSkillBtnPanel(false);
            return;
        }

        Debug.Log(skillNum);
        for (int i = 0; i < Mathf.Min(Constants.SKILLCNT, skillNum); i++)
        {
            int tempNum = 0;
            // 임시 인트
            do
            {
                _skillCompany = tempSkillCompany;
                tempNum = Random.Range(0, 100);
                loopCnt++;
                if (bCheckCollab && indexList.Count>0 && tempNum < Constants.COLLABPER)
                {
                    _skillCompany = GameManager.Instance.skillManager.skill_Collab;
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
    /// 버튼에 스킬 정보 세팅
    /// </summary>
    public bool SetSkillBtn(int skillNum)
    {
        // 예외처리
        if (skillNum > _skillCompany.skillList.Count)
        {
            return true;
        }
        if (!_tempSkillList.Contains(_skillCompany.skillList[skillNum].skillIdx))
        {
            SkillBtn skillBtn = GameManager.Instance.UIManager.MakeSkillBtn();
            skillBtn.SetSkillInfo(_skillCompany.skillList[skillNum]);
            skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillBtnClick(skillBtn));
            _tempSkillList.Add(skillBtn.skillData.skillIdx);
            return false;
        }
        else return true;
    }
    #endregion

    #region Upgrade
    /// <summary>
    /// 스킬 업그레이드 버튼 생성
    /// </summary>
    public void SetUpgradeBtn()
    {
        GameManager.Instance.UIManager.SetActiveSkillBtnPanel(true);
        int upgradeSkillNum = GameManager.Instance.skillManager.upgradeSkillList.Count;


        if (upgradeSkillNum == 0)
        {
            Debug.Log("Error");
            GameManager.Instance.UIManager.SetActiveSkillBtnPanel(false);
            return;
        }

        for (int i = 0; i < upgradeSkillNum; i++)
        {

            int tempNum = 0;
            // 임시 인트
            do
            {
                tempNum = Random.Range(0, upgradeSkillNum);

                loopCnt++;
            }
            while (SetUpgradeSkillBtn(tempNum) && loopCnt < Constants.LOOPCNT);
            Debug.Log("Loop : " + loopCnt);
            loopCnt = 0;
        }

        _tempSkillList.Clear();

    }

    #endregion

    public bool SetUpgradeSkillBtn(int skillNum)
    {
        if (!_tempSkillList.Contains(GameManager.Instance.skillManager.upgradeSkillList[skillNum].skillIdx))
        {
            SkillBtn skillBtn = GameManager.Instance.UIManager.MakeSkillBtn();
            skillBtn.SetSkillInfo(GameManager.Instance.skillManager.upgradeSkillList[skillNum]);
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
        if(skillBtn.skillData == null)
        {
            Debug.Log("skillData is Null");
        }
        if (skillBtn.skillData.ChooseSkill())
        {
            skillBtn.skillData.skillCompany.ChooseSkill(skillBtn.skillData);

        }
        skillBtn.skillData.skillCompany.ActivateSkill(skillBtn.skillData);

        GameManager.Instance.UIManager.DestroyChildObject(skillBtn.transform.parent);

        GameManager.Instance.UIManager.SetActiveSkillBtnPanel(false);
    }


    /// <summary>
    /// 후에 삭제, 테스트용 함수들
    /// </summary>
    #region testBtn
    public void OnClick_DrawSkillCompany()
    {
        SetSkillCompany(GameManager.Instance.skillManager.GetSkillPackTypes(1)[0]);
    }

    public void OnClick_skillCompanyOne()
    {
        SetSkillCompany(TechSelectPackType.Company1);
    }

    public void OnClick_skillCompanyTwo()
    {
        SetSkillCompany(TechSelectPackType.Company2);
    }

    public void OnClick_skillCompanyThree()
    {
        SetSkillCompany(TechSelectPackType.Company3);
    }

    public void OnClick_skillCompanyFour()
    {
        SetSkillCompany(TechSelectPackType.Company4);
    }

    public void OnClick_skillCompanyFive()
    {
        SetSkillCompany(TechSelectPackType.Company5);
    }

    public void OnClick_ListBtn()
    {
        GameManager.Instance.UIManager.MakeCurrentSkillList();
    }


		#endregion
}
