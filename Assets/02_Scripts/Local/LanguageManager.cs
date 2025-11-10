using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{

		public event Action OnLanguageChanged;


		public void ChangeLanguage(string localeCode)
		{
				StartCoroutine(SetLocale(localeCode));
		}

		private IEnumerator SetLocale(string localeCode)
		{
				yield return LocalizationSettings.InitializationOperation;

				var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
				if (selectedLocale != null)
				{
						LocalizationSettings.SelectedLocale = selectedLocale;

						// UI 갱신
						GameManager.Instance.UIManager?.RefreshCurrentSkillUI();
						GameManager.Instance.UIManager?.RefreshAllChooseButtons();

						// 이벤트 브로드캐스트
						OnLanguageChanged?.Invoke();
				}
		}

		public string GetLocalizedText(string key)
		{
				var table = LocalizationSettings.StringDatabase.GetTable(Constants.LOCAL_TABLE);
				if (table == null)
						return $"[MissingTable:{Constants.LOCAL_TABLE}]";

				var entry = table.GetEntry(key);
				if (entry == null || string.IsNullOrEmpty(entry.LocalizedValue))
						return $"[MissingKey:{key}]";

				return entry.LocalizedValue;
		}

}
