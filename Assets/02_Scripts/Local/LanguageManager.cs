using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LanguageManager : MonoBehaviour

{
    public string GetLocalizedText(string key)
    {
        var table = LocalizationSettings.StringDatabase.GetTable(Constants.LOCAL_TABLE);
        if (table == null)
        {
            Debug.LogWarning($"테이블 '{Constants.LOCAL_TABLE}'을 찾을 수 없습니다.");
            return $"[MissingTable:{Constants.LOCAL_TABLE}]";
        }

        var entry = table.GetEntry(key);
        if (entry == null || string.IsNullOrEmpty(entry.LocalizedValue))
        {
            Debug.LogWarning($" 키 '{key}'를 테이블 '{Constants.LOCAL_TABLE}'에서 찾을 수 없습니다.");
            return $"[MissingKey:{key}]";
        }

        return entry.LocalizedValue;
    }



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
