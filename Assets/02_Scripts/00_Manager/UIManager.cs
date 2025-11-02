using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class UIManager : MonoBehaviour
{
		[SerializeField] private SkillBtn _skillBtnPrefab;
		[SerializeField] private GameObject _listPanel;

		[SerializeField] private Image _skillImage;
		[SerializeField] private Text _skillScriptText;
		[SerializeField] private Text _skillValueScriptText;
		[SerializeField] private Text _skillLevelText;
		[SerializeField] private Transform _parentContent;

		[SerializeField] private GameObject _skillBtnPanel;
		[SerializeField] private GameObject _parentPanel;
		[SerializeField] private SkillBtn _skillChooseBtnPrefab;

		public event Action onRewardSelect;
		#region 스킬 리스트
		/// <summary>
		/// 현재 보유 스킬 리스트에서 선택한 버튼 정보
		/// </summary>
		private SkillBtn _currentSelectedSkillBtn; 

		/// <summary>
		/// 현재 활성화 되어있는 스킬 버튼 리스트
		/// </summary>
		private List<SkillBtn> _activeChooseButtons = new List<SkillBtn>();

		[SerializeField]
		private SkillTooltip _skillTooltip;
		public SkillTooltip skillTooltip { get { return _skillTooltip; } }

		public void InitializeManager()
		{
				if (_skillBtnPrefab == null)
						_skillBtnPrefab = Resources.Load<SkillBtn>("Prefabs/Skill/SkillBtnPrefab");

				if (_skillChooseBtnPrefab == null)
						_skillChooseBtnPrefab = Resources.Load<SkillBtn>("Prefabs/Skill/SkillChoosePrefab");

				_skillTooltip.Initialize();
		}

		public void MakeCurrentSkillList()
		{
				_listPanel.SetActive(true);
				List<SkillData> list = GameManager.Instance.skillManager.GetChooseSkillList();
				if (list == null) return;

				foreach (SkillData skill in list)
				{
						MakeSkillBtn(skill, _parentContent);
				}
		}

		public void MakeSkillBtn(SkillData skillData, Transform parentContent)
		{
				SkillBtn skillBtn = GameManager.Instance.PoolManager
						.GetFromPool(_skillBtnPrefab, _skillBtnPrefab.transform.position, _skillBtnPrefab.transform.rotation, parentContent)
						.GetComponent<SkillBtn>();

				skillBtn.SetSkillInfo(skillData);
				skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillListBtn(skillBtn));
		}

		public void OnClick_SkillListBtn(SkillBtn skillBtn)
		{
				// 선택된 버튼 저장
				_currentSelectedSkillBtn = skillBtn; 

				SkillData data = skillBtn.skillData;
				_skillImage.sprite = data.skillImagePath;

				_skillScriptText.text = $"{data.skillIdx}\n" + (Constants.STRING_Korean == "ko" ? data.skillScript : data.skillScriptEn);
				_skillValueScriptText.text = Constants.STRING_Korean == "ko" ? data.skillValueScript : data.skillValueScriptEn;
				_skillLevelText.text = $"{data.skillLevel} / {data.skillMaxLevel}";
		}
		/// <summary>
		/// 언어 변경 시 UI 갱신
		/// </summary>
		public void RefreshCurrentSkillUI() 
		{
				if (_currentSelectedSkillBtn != null)
				{
						OnClick_SkillListBtn(_currentSelectedSkillBtn);
				}
		}

		public void OnClick_ListExitBtn()
		{
				foreach (Transform child in _parentContent)
				{
						PoolableObject childPool = child.GetComponent<PoolableObject>();
						if (childPool != null)
						{
								SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
								if (skillBtn != null) skillBtn.ReleaseObject();
								GameManager.Instance.PoolManager.ReleaseToPoolByInterface(childPool);
						}
				}
		}
		#endregion


		public void SetActiveSkillBtnPanel(bool isActive)
		{
				_skillBtnPanel.SetActive(isActive);
				if (!isActive)
						onRewardSelect?.Invoke();
		}

		#region skill choose
		public SkillBtn MakeSkillBtn()
		{
				SkillBtn skillBtn = GameManager.Instance.PoolManager
						.GetFromPool(_skillChooseBtnPrefab, Vector3.zero, _skillChooseBtnPrefab.transform.rotation, _parentPanel.transform)
						.GetComponent<SkillBtn>();

				_activeChooseButtons.Add(skillBtn);
				return skillBtn;
		}

		/// <summary>
		/// 버튼 텍스트 갱신
		/// </summary>
		public void RefreshAllChooseButtons()
		{
				foreach (SkillBtn button in _activeChooseButtons)
				{
						button.RefreshLanguage(); 
				}
		}


		public void DestroyChildObject(Transform parentObject)
		{
				Transform[] children = new Transform[parentObject.childCount];
				for (int i = 0; i < parentObject.childCount; i++)
						children[i] = parentObject.GetChild(i);

				foreach (Transform child in children)
				{
						PoolableObject childPool = child.GetComponent<PoolableObject>();
						if (childPool != null)
						{
								SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
								if (skillBtn != null) skillBtn.ReleaseObject();
								GameManager.Instance.PoolManager.ReleaseToPoolByInterface(childPool);
						}
				}
		}

		public void OnClickListExitBtn(Transform content)
		{
				Transform[] children = new Transform[content.childCount];
				for (int i = 0; i < content.childCount; i++)
						children[i] = content.GetChild(i);

				foreach (Transform child in children)
				{
						PoolableObject childPool = child.GetComponent<PoolableObject>();
						if (childPool != null)
						{
								SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
								if (skillBtn != null) skillBtn.ReleaseObject();
								GameManager.Instance.PoolManager.ReleaseToPoolByInterface(childPool);
						}
				}

				_listPanel.SetActive(false);
		}
		#endregion

	
}
