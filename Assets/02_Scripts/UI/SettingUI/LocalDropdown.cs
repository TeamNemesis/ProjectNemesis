using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LocalDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private string tableName;
    [SerializeField] private string[] stringKeys;

    private void OnEnable()
    {
        languageDropdown.onValueChanged.AddListener(OnDropdownChanged);
        LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;

        // 드롭다운 옵션 먼저 설정
        UpdateDropdown(LocalizationSettings.SelectedLocale);

        // 저장된 언어 인덱스 기준으로 드롭다운 UI 동기화
        if (PlayerPrefs.HasKey(Constants.PREF_KEY))
        {
            int savedIndex = PlayerPrefs.GetInt(Constants.PREF_KEY);
            languageDropdown.value = savedIndex;
            languageDropdown.RefreshShownValue();
        }
        else
        {
            languageDropdown.value = 0;
            languageDropdown.RefreshShownValue();
        }
    }

    private void OnDisable()
    {
        languageDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
    }

    private void UpdateDropdown(Locale locale)
    {
        LocalizationSettings.StringDatabase.GetTableAsync(tableName).Completed += handle =>
        {
            var table = handle.Result;
            languageDropdown.options.Clear();

            foreach (var key in stringKeys)
            {
                var entry = table.GetEntry(key);
                string localizedText = entry?.GetLocalizedString() ?? key;
                languageDropdown.options.Add(new TMP_Dropdown.OptionData(localizedText));
            }

            languageDropdown.RefreshShownValue();
        };

        // UIManager에 UI 갱신 요청 (있다면)
        if (GameManager.Instance.UIManager != null)
        {
            GameManager.Instance.UIManager.RefreshCurrentSkillUI();
            GameManager.Instance.UIManager.RefreshAllChooseButtons();
        }
    }

    private void OnDropdownChanged(int index)
    {
        if (index >= 0 && index < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

            PlayerPrefs.SetInt(Constants.PREF_KEY, index);
            PlayerPrefs.Save();
        }
    }
}
