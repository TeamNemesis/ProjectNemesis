using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class SkillChoose : MonoBehaviour
{


		/// <summary>
		/// 회사 Dictionary
		/// </summary>
		private Dictionary<string, List<Vector2Int>> CompanyDic = new Dictionary<string, List<Vector2Int>>();

		[SerializeField] private Skill_One skillOne;
		[SerializeField] private Skill_Two skillTwo;
		[SerializeField] private Skill_Three skillThree;
		[SerializeField] private Skill_Four skillFour;
		[SerializeField] private Skill_Five skillFive;

		public List<SkillBase> choosedCompanyList;
		public SkillBase skillCompany;


		public GameObject skillBtnPanel;
		public SkillBtn[] skillBtns;

		[Header("스킬 확률")]
		public float skillPer;

		void Start()
		{
				// 회사 스킬 초기화
				skillOne.InitSkillDictionary();
				skillTwo.InitSkillDictionary();
				skillThree.InitSkillDictionary();
				skillFour.InitSkillDictionary();
				skillFive.InitSkillDictionary();

				// 회사 스킬 Dictionary에 저장
				CompanyDic[skillOne.GetType().Name] = new List<Vector2Int>();
				for (int i = 0; i < 10; i++)
				{
						CompanyDic[skillOne.GetType().Name].Add(new Vector2Int(i,0));
				}

				CompanyDic[skillTwo.GetType().Name] = new List<Vector2Int>();
				for (int i = 0; i < 10; i++)
				{
						CompanyDic[skillTwo.GetType().Name].Add(new Vector2Int(i, 0));
				}

				CompanyDic[skillThree.GetType().Name] = new List<Vector2Int>();
				for (int i = 0; i < 10; i++)
				{
						CompanyDic[skillThree.GetType().Name].Add(new Vector2Int(i, 0));
				}

				CompanyDic[skillFour.GetType().Name] = new List<Vector2Int>();
				for (int i = 0; i < 10; i++)
				{
						CompanyDic[skillFour.GetType().Name].Add(new Vector2Int(i, 0));
				}

				CompanyDic[skillFive.GetType().Name] = new List<Vector2Int>();
				for (int i = 0; i < 10; i++)
				{
						CompanyDic[skillFive.GetType().Name].Add(new Vector2Int(i, 0));
				}

				for (int i = 0; i < skillBtns.Length; i++)
				{
						int index = i;
						skillBtns[index].GetComponent<Button>().onClick.AddListener(
								() => SkillLevelUpBtnClick(skillBtns[index]));
				}

		}

		public void OnClickBtn()
		{
				skillBtnPanel.SetActive(true);
				Debug.Log("Click");

				for(int i =0; i< skillBtns.Length; i++)
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
										// 해당 회사의 스킬을 뽑고 버튼에 세팅
										tempNum = Random.Range(0,choosedCompanyList.Count);
										skillCompany = choosedCompanyList[tempNum];
										tempNum = Random.Range(0, 10);
										SetSkillBtn(tempNum, skillBtns[i],true);
										continue;
								}
						}


						tempNum = Random.Range(0, 5);
						switch (tempNum)
						{
								case 0:
										skillCompany = skillOne;
										break;
								case 1:
										skillCompany = skillTwo;
										break;
								case 2:
										skillCompany = skillThree;
										break;
								case 3:
										skillCompany = skillFour;
										break;
								case 4:
										skillCompany = skillFive;
										break;
								default:
										Debug.Log("오류발생 : " + tempNum);
										break;
						}

						// 해당 회사의 스킬을 뽑고 버튼에 세팅
						tempNum = Random.Range(0, 10);
						SetSkillBtn(tempNum, skillBtns[i],false);
				}

				

		}

		/// <summary>
		/// 뽑은 스킬 업그레이드
		/// </summary>
		public void SkillUpgrade(int skillNum)
		{
				// 업그레이드 할 스킬 레벨 저장을 위한 임시 스킬 레벨 저장
				Vector2Int upgradeSkill = CompanyDic[skillCompany.GetType().Name][skillNum];
				Debug.Log(skillCompany.GetType().Name + upgradeSkill.ToString());
				upgradeSkill.y += 1;
				if (upgradeSkill.y == 1) skillCompany.currentSkillCnt++;
				CompanyDic[skillCompany.GetType().Name][skillNum] = upgradeSkill;
		}

		/// <summary>
		/// 버튼에 스킬 정보 세팅
		/// </summary>
		public void SetSkillBtn(int skillNum, SkillBtn btn, bool isPre)
		{
				btn.SetSkillInfo(skillCompany.skillLevel[skillNum],skillCompany,isPre);

		}

		/// <summary>
		/// 스킬 레벨업 버튼 선택
		/// </summary>
		public void SkillLevelUpBtnClick(SkillBtn skillBtn)
		{
				Vector2Int tempVector = CompanyDic[skillCompany.GetType().Name][skillBtn.skillIdx-1];
				if(tempVector.y == 0) skillCompany.currentSkillCnt++; 
				tempVector.y += 1;
				CompanyDic[skillCompany.GetType().Name][skillBtn.skillIdx-1] = tempVector;

				if(!choosedCompanyList.Contains(skillCompany))
				choosedCompanyList.Add(skillCompany);

				skillBtn.skillCompany.skillLevel[skillBtn.skillIdx-1].LevelUp();
				Debug.Log(skillBtn.skillCompany.skillLevel[skillBtn.skillIdx-1].skillLevel);
				skillBtnPanel.SetActive(false);
		}

		
}
