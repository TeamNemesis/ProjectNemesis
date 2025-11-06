using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalDropdown : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    void Start()
    {
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);

        // 현재 시스템 언어 자동 설정
        var systemLang = Application.systemLanguage.ToString();
        for (int i = 0; i < languageDropdown.options.Count; i++)
        {
            if (languageDropdown.options[i].text == systemLang)
            {
                languageDropdown.value = i;
                ChangeLanguage(i);
                break;
            }
        }
    }

    void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
