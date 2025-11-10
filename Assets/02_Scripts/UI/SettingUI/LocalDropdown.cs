using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocalDropdown : MonoBehaviour
{
		[SerializeField] private TMP_Dropdown languageDropdown;
		[SerializeField] private string[] stringKeys;

		private LanguageManager languageManager;

		private void OnEnable()
		{
				if (GameManager.Instance == null || GameManager.Instance.languageManager == null)
				{
						Debug.LogError("❌ GameManager 또는 LanguageManager가 null입니다.");
						return;
				}

				languageManager = GameManager.Instance.languageManager;
				languageDropdown.onValueChanged.AddListener(OnDropdownChanged);
				LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;

				UpdateDropdown(LocalizationSettings.SelectedLocale);

				int savedIndex = PlayerPrefs.GetInt(Constants.LOCAL_PREF_KEY, 0);
				languageDropdown.value = savedIndex;
				languageDropdown.RefreshShownValue();
		}

		private void OnDisable()
		{
				languageDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
				LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
		}

		private void UpdateDropdown(Locale locale)
		{
				LocalizationSettings.StringDatabase.GetTableAsync(Constants.LOCAL_TABLE).Completed += handle =>
				{
						if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
						{
								Debug.LogError($"❌ 테이블 '{Constants.LOCAL_TABLE}' 로드 실패: {handle.OperationException}");
								return;
						}

						var table = handle.Result;
						languageDropdown.options.Clear();

						foreach (var key in stringKeys)
						{
								var entry = table.GetEntry(key);
								string localizedText = entry?.GetLocalizedString() ?? $"⚠️ {key}";
								languageDropdown.options.Add(new TMP_Dropdown.OptionData(localizedText));
						}

						languageDropdown.RefreshShownValue();
				};
		}

		private void OnDropdownChanged(int index)
		{
				if (index >= 0 && index < LocalizationSettings.AvailableLocales.Locales.Count)
				{
						var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
						string localeCode = selectedLocale.Identifier.Code;

						languageManager.ChangeLanguage(localeCode);

						PlayerPrefs.SetInt(Constants.LOCAL_PREF_KEY, index);
						PlayerPrefs.Save();
				}
		}
}
