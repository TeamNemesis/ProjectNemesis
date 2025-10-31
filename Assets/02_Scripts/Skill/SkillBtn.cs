using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : PoolableObject
{
		private SkillData _skillData;
		public SkillData skillData => _skillData;

		[SerializeField] private Image _skillImage;
		[SerializeField] private Text _skillScirpt;
		[SerializeField] private Text _skillIDX;
		[SerializeField] private Text _skillLevel;

		public void SetSkillData(SkillData setSkillData)
		{
				_skillData = setSkillData;
		}

		public void SetSkillInfo(SkillData choosedSkill)
		{
				_skillData = choosedSkill;

				// 언어 번역
				RefreshLanguage(); 

				if (_skillImage != null)
						_skillImage.sprite = choosedSkill.skillImagePath;

				if (_skillIDX != null)
						_skillIDX.text = choosedSkill.skillIdx.ToString();

				if (_skillLevel != null)
						_skillLevel.text = $"{choosedSkill.skillLevel} / {choosedSkill.skillMaxLevel}";
		}

		/// <summary>
		/// 언어 변경 시 텍스트만 갱신
		/// </summary>
		public void RefreshLanguage()
		{
				if (_skillScirpt != null && _skillData != null)
				{
						string locale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
						_skillScirpt.text = locale == "ko" ? _skillData.skillScript : _skillData.skillScriptEn;
				}
		}

		public GameObject GetGameObject() => gameObject;

		public void ReleaseObject()
		{
				if (_skillScirpt != null) _skillScirpt.text = null;
				if (_skillImage != null) _skillImage.sprite = null;
				if (_skillIDX != null) _skillIDX.text = null;
				if (_skillLevel != null) _skillLevel.text = null;

				_skillData = null;
				GetComponent<Button>().onClick.RemoveAllListeners();
		}
}
