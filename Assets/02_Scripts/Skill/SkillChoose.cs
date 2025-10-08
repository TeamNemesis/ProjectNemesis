using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoose : MonoBehaviour
{
    /// <summary>
    /// 뽑은 스킬 임시 저장 리스트
    /// </summary>
    private List<int> _skillIDX = new List<int>();

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
    private SkillBtn[] _skillBtns;


    void Start()
    {
        for (int i = 0; i < _skillBtns.Length; i++)
        {
            int index = i;
            _skillBtns[index].GetComponent<Button>().onClick.AddListener(
                    () => OnClick_SkillBtnClick(_skillBtns[index]));
        }

    }

    public void OnClickBtn()
    {
        _skillBtnPanel.SetActive(true);
        Debug.Log("Click");



        for (int i = 0; i < _skillBtns.Length; i++)
        {
            // 임시 인트
            int tempNum = 0;

            // 에러 체크
            if (_skillCompany != null)
            {
                {
                    do
                    {
                        // 뽑은 회사의 남은 스킬이 2개 이하라면 다시 처음으로
                        if (_skillCompany.skillList.Count < 3)
                        {
                            i--;
                            //continue;
                            Debug.Log("다 뽑음");
                            break;
                        }
                        tempNum = Random.Range(0, _skillCompany.skillList.Count);

                    }
                    //중복이라면 반복
                    while (SetSkillBtn(tempNum, _skillBtns[i]));

                    // 스킬 뽑았으므로 i++
                    continue;
                }
            }
        }

        // 뽑은 스킬 리스트 초기화
        _skillIDX.Clear();
    }


    /// <summary>
    /// 버튼에 스킬 정보 세팅
    /// </summary>
    public bool SetSkillBtn(int skillNum, SkillBtn btn)
    {
        if (!_skillIDX.Contains(_skillCompany.skillList[skillNum].skillIdx))
        {
            btn.SetSkillInfo(_skillCompany.skillList[skillNum]);
            _skillIDX.Add(_skillCompany.skillList[skillNum].skillIdx);
            Debug.Log("Setting");
            return false;
        }
        else return true;

    }

    /// <summary>
    /// 스킬 버튼 선택
    /// </summary>
    public void OnClick_SkillBtnClick(SkillBtn skillBtn)
    {
        skillBtn.skillData.LevelUp();
				skillCompany.ChooseSkill(skillBtn.skillData);
        _skillBtnPanel.SetActive(false);
    }


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
