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
		[SerializeField]
		private int _skillCnt = 3;

		/// <summary>
		/// 초기 스킬 선택 버튼 생성
		/// </summary>
		public void SetBtn()
		{
				_skillBtnPanel.SetActive(true);
				Debug.Log("Click");

				if (_skillCompany == null || _skillCompany.skillList.Count == 0)
				{
						Debug.Log("Error");
						_skillBtnPanel.SetActive(false);
						return;
				}

				for (int i = 0; i < Mathf.Min(_skillCnt, _skillCompany.skillList.Count); i++)
				{
						int tempNum = 0;
						// 임시 인트
						do
						{
								tempNum = Random.Range(0, _skillCompany.skillList.Count);

						}
						while (SetSkillBtn(tempNum));

				}

				_tempSkillList.Clear();
		}

		/// <summary>
		/// 스킬 업그레이드 버튼 생성
		/// </summary>
		public void SetUpgradeBtn()
		{
				_skillBtnPanel.SetActive(true);
				Debug.Log("Click");

				if (_skillCompany == null || _skillCompany.currentSkillData.Count == 0)
				{
						Debug.Log("Error");
						_skillBtnPanel.SetActive(false);
						return;
				}

				for (int i = 0; i < Mathf.Min(_skillCnt, _skillCompany.currentSkillData.Count); i++)
				{
						int tempNum = 0;
						// 임시 인트
						do
						{
								tempNum = Random.Range(0, _skillCompany.currentSkillData.Count);

						}
						while (SetUpgradeSkillBtn(tempNum));

				}

				_tempSkillList.Clear();

		}


		/// <summary>
		/// 버튼에 스킬 정보 세팅
		/// </summary>
		public bool SetSkillBtn(int skillNum)
		{
				if (!_tempSkillList.Contains(_skillCompany.skillList[skillNum].skillIdx))
				{
						SkillBtn skillBtn = Instantiate(_skillBtnPrefab, _parentPanel.transform);
						skillBtn.SetSkillInfo(_skillCompany.skillList[skillNum]);
						skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillBtnClick(skillBtn));
						_tempSkillList.Add(skillNum);
						Debug.Log("Setting");
						return false;
				}
				else return true;
		}

		public bool SetUpgradeSkillBtn(int skillNum)
		{
				if (!_tempSkillList.Contains(skillNum))
				{
						SkillBtn skillBtn = Instantiate(_skillBtnPrefab, _parentPanel.transform);
						skillBtn.SetSkillInfo(_skillCompany.currentSkillData[skillNum]);
						skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillBtnClick(skillBtn));
						_tempSkillList.Add(skillNum);
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
				if (skillBtn.skillData.LevelUp())
				{
						skillCompany.ChooseSkill(skillBtn.skillData);

				}
						skillCompany.ActivateSkill(skillBtn.skillData);

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
