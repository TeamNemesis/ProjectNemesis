using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBtn : PoolableObject, IPointerEnterHandler, IPointerExitHandler
{
		private SkillData _skillData;
		public SkillData skillData => _skillData;

		[SerializeField] private Image _skillImage;
		[SerializeField] private Text _skillScirpt;
		[SerializeField] private Text _skillIDX;
		[SerializeField] private Text _skillLevel;

		private float _currentTime;
		[SerializeField]
		private float _tooltipTime;
		[SerializeField]
		private bool _bIsPointerEnter;

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
						_skillScirpt.text = Constants.STRING_Korean == "ko" ? _skillData.skillScript : _skillData.skillScriptEn;
				}
		}

		public GameObject GetGameObject() => gameObject;

		public void Update()
		{
				if (_bIsPointerEnter)
				{
						_currentTime += Time.deltaTime;
						if (_currentTime > _tooltipTime)
						{
								Debug.LogWarning("툴팁 생성");
								GameManager.Instance.UIManager.skillTooltip.ShowTooltip(skillData);
								Debug.LogWarning(_bIsPointerEnter);
								_bIsPointerEnter = false;
						}
				}
		}

		public void ReleaseObject()
		{
				if (_skillScirpt != null) _skillScirpt.text = null;
				if (_skillImage != null) _skillImage.sprite = null;
				if (_skillIDX != null) _skillIDX.text = null;
				if (_skillLevel != null) _skillLevel.text = null;
				_currentTime = 0f;
				_bIsPointerEnter = false;

				_skillData = null;
				GetComponent<Button>().onClick.RemoveAllListeners();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
				Debug.LogWarning("Enter");
				_bIsPointerEnter = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
				Debug.LogWarning("Out");

				_currentTime = 0f;
				_bIsPointerEnter = false;
				GameManager.Instance.UIManager.skillTooltip.ReleaseCurrentTooltip();
		}
}
