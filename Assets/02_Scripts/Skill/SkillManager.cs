using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
		private static SkillManager _instance;

		public static SkillManager Instance()
		{
				return _instance;
		}

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
				if (_instance != null)
				{
						Destroy(gameObject);
				}
				else
				{
						_instance = this;
						DontDestroyOnLoad(gameObject);
				}

				_skill_One = GetComponent<Skill_One>();
				_skill_Two = GetComponent<Skill_Two>();
				_skill_Three = GetComponent<Skill_Three>();
				_skill_Four = GetComponent<Skill_Four>();
				_skill_Five = GetComponent<Skill_Five>();

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
								MakeSkillBtn(skillData,parentContent.transform);
						}
				}

		}
		
		/// <summary>
		/// 버튼 생성 함수
		/// </summary>
		/// <param name="skillData"></param>
		/// <param name="parentContent"></param>
		public void MakeSkillBtn(SkillData skillData,Transform parentContent)
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

				int totalSkillNum = skillOneNum + skillTwoNum + skillThreeNum + skillFourNum + skillFiveNum;


				// 임시 숫자를 저장할 변수
				int tempNum = Random.Range(0, totalSkillNum);


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
		public bool CheckCollabo(SkillBase skillCompany, out List<SkillData> collaboSkillList)
		{
				collaboSkillList = null;
				// 매개 변수의 회사가 만족하는지 판단
				if (skillCompany.currentSkillData.Count < 3)
				{
						return false;
				}

				if (skillCompany == _skill_One)
				{
						// 관련 회사 조건 검사
						if(_skill_Two.currentSkillData.Count >=3)
						{
								
						}
						
						if(_skill_Five.currentSkillData.Count >=3)
						{

						}
				}
				else if (skillCompany == _skill_Two)
				{
						// 관련 회사 조건 검사
						if (_skill_Three.currentSkillData.Count >= 3)
						{

						}

						if (_skill_One.currentSkillData.Count >= 3)
						{

						}
				}
				else if (skillCompany == _skill_Three)
				{
						// 관련 회사 조건 검사
						if (_skill_Four.currentSkillData.Count >= 3)
						{

						}

						if (_skill_Two.currentSkillData.Count >= 3)
						{

						}
				}
				else if(skillCompany == _skill_Four)
				{
						// 관련 회사 조건 검사
						if (_skill_Five.currentSkillData.Count >= 3)
						{
								 
						}

						if (_skill_Three.currentSkillData.Count >= 3)
						{

						}
				}
				else if( skillCompany == _skill_Five)
				{
						// 관련 회사 조건 검사
						if (_skill_Four.currentSkillData.Count >= 3)
						{

						}

						if (_skill_One.currentSkillData.Count >= 3)
						{

						}
				}

				// 해금된 콜라보 스킬 있는지 검사
				if (collaboSkillList.Count > 0)
				{
						return true;
				}
				else return false;

		}


}
