using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class ResolutionDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private string tableName;
    [SerializeField] private string[] stringKeys;


    private void OnEnable()
    {
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;

        UpdateDropdown(LocalizationSettings.SelectedLocale);

        // 저장된 설정값 불러오기
        if (PlayerPrefs.HasKey(Constants.RESOLUTION_PREF_KEY))
        {
            int savedIndex = PlayerPrefs.GetInt(Constants.RESOLUTION_PREF_KEY);
            resolutionDropdown.value = savedIndex;
            resolutionDropdown.RefreshShownValue();
            ChangeResolution(savedIndex);
        }
        else
        {
            // 기본값 설정 (예: 첫 번째 항목)
            resolutionDropdown.value = 0;
            resolutionDropdown.RefreshShownValue();
            ChangeResolution(0);
        }

    }
   

    private void OnDisable()
    {
        resolutionDropdown.onValueChanged.RemoveListener(ChangeResolution);
        LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
    }

    private void UpdateDropdown(Locale locale)
    {
        LocalizationSettings.StringDatabase.GetTableAsync(tableName).Completed += handle =>
        {
            var table = handle.Result;
            resolutionDropdown.options.Clear();

            foreach (var key in stringKeys)
            {
                var entry = table.GetEntry(key);
                string localizedText = entry?.GetLocalizedString() ?? key;
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(localizedText));
            }

            resolutionDropdown.RefreshShownValue();
        };
    }

    private void ChangeResolution(int index)
    {
        // 설정값 저장
        PlayerPrefs.SetInt(Constants.RESOLUTION_PREF_KEY, index);
        PlayerPrefs.Save();

        switch (index)
        {
            case 0: // PC, default
#if UNITY_STANDALONE_WIN
                QualitySettings.SetQualityLevel(0); // 예: PC
#elif UNITY_ANDROID
                QualitySettings.SetQualityLevel(0); // 예: Mobile
#endif
                break;
            case 1: // High
                QualitySettings.SetQualityLevel(1);
                break;
            case 2: // Middle
                QualitySettings.SetQualityLevel(2);
                break;
            case 3: // Low
                QualitySettings.SetQualityLevel(3);
                break;
            default:
                Debug.LogError("해당 사항 없음");
                break;
        }

    }
}
