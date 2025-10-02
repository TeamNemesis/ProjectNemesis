using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoose : MonoBehaviour
{


    /// <summary>
    /// 회사 Dictionary
    /// </summary>
    private List<SkillBase> CompanyList;

    /// <summary>
    /// 뽑은 스킬 임시 저장 리스트
    /// </summary>
    private List<int> skillIDX = new List<int>();

    [SerializeField] private Skill_One skillOne;
    [SerializeField] private Skill_Two skillTwo;
    [SerializeField] private Skill_Three skillThree;
    [SerializeField] private Skill_Four skillFour;
    [SerializeField] private Skill_Five skillFive;

    public Dictionary<SkillBase, int> choosedCompanyList = new Dictionary<SkillBase, int>();
    public SkillBase skillCompany;


    public GameObject skillBtnPanel;
    public SkillBtn[] skillBtns;

    [Header("스킬 확률")]
    public float skillPer;

    void Start()
    {
        CompanyList = new List<SkillBase>();
        CompanyList.Add(skillOne);
        CompanyList.Add(skillTwo);
        CompanyList.Add(skillThree);
        CompanyList.Add(skillFour);
        CompanyList.Add(skillFive);

        // 스킬 베이스 초기화
        foreach(SkillBase skill in CompanyList)
        {
            skill.InitSkillDictionary();
        }

        for (int i = 0; i < skillBtns.Length; i++)
        {
            int index = i;
            skillBtns[index].GetComponent<Button>().onClick.AddListener(
                    () => OnClick_SkillBtnClick(skillBtns[index]));
        }

    }

    public void OnClickBtn()
    {
        skillBtnPanel.SetActive(true);
        Debug.Log("Click");

        

        for (int i = 0; i < skillBtns.Length; i++)
        {
            // 임시 인트
            int tempNum = 0;
            //캐릭터가 직전에 회사를 뽑았다면
            if (choosedCompanyList.Count > 0)
            {
                // 캐릭터가 직전에 뽑았던 회사의 스킬을 뽑을지 (25%)
                tempNum = Random.Range(0, 100);
                if (tempNum < skillPer)
                {
                    do
                    {
                        
                        // 해당 회사의 스킬을 뽑고 버튼에 세팅
                        tempNum = Random.Range(0, choosedCompanyList.Count);
                        skillCompany = choosedCompanyList.ElementAt(tempNum).Key;

                        // 뽑은 회사의 남은 스킬이 2개 이하라면 다시 처음으로
                        if(skillCompany.skillList.Count <3)
                        {
                            i--;
                            //continue;
                            Debug.Log("다 뽑음");
                            break;
                        }
                        tempNum = Random.Range(0, skillCompany.skillList.Count);
                        
                    }
                    //중복이라면 반복
                    while (SetSkillBtn(tempNum, skillBtns[i], true));

                    // 스킬 뽑았으므로 i++
                    continue;
                }
            }


            tempNum = Random.Range(0, 5);
            skillCompany = CompanyList[tempNum];

            // 해당 회사의 스킬을 뽑고 버튼에 세팅
            tempNum = Random.Range(0, skillCompany.skillList.Count);
            SetSkillBtn(tempNum, skillBtns[i], false);
        }

        // 뽑은 스킬 리스트 초기화
        skillIDX.Clear();

    }


    /// <summary>
    /// 버튼에 스킬 정보 세팅
    /// </summary>
    public bool SetSkillBtn(int skillNum, SkillBtn btn, bool isPre)
    {
        if (!skillIDX.Contains(skillCompany.skillList[skillNum].skillIdx))
        {
            btn.SetSkillInfo(skillCompany.skillList[skillNum], skillCompany, isPre);
            skillIDX.Add(skillCompany.skillList[skillNum].skillIdx);
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
        skillBtn.skillCompany.ChooseSkill(skillBtn.skillData);
        if(choosedCompanyList.ContainsKey(skillBtn.skillCompany))
        {
            choosedCompanyList[skillBtn.skillCompany]++;
        }
        else
        {
            choosedCompanyList.Add(skillBtn.skillCompany, 1);
        }
            skillBtnPanel.SetActive(false);
    }


}
