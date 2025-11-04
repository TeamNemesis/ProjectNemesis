using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
		

		public void ChangeLanguage(string localeCode)
		{
				StartCoroutine(SetLocale(localeCode));
		}

		IEnumerator SetLocale(string localeCode)
		{
				yield return LocalizationSettings.InitializationOperation;
				var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
				if (selectedLocale != null)
				{
						LocalizationSettings.SelectedLocale = selectedLocale;

						// 스킬 리스트 버튼 갱신
						GameManager.Instance.UIManager.RefreshCurrentSkillUI();

						// 버튼 텍스트 갱신
						GameManager.Instance.UIManager.RefreshAllChooseButtons();
				}
		}
}
