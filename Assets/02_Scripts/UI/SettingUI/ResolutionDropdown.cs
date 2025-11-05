using TMPro;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    void Start()
    {
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            Debug.Log($"Index: {i}, Name: {QualitySettings.names[i]}");
        }
    }

    void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0: // PC, default
#if UNITY_STANDALONE_WIN
                QualitySettings.SetQualityLevel(1); // 예: PC
#endif
#if UNITY_ANDROID
                QualitySettings.SetQualityLevel(0); // 예: Mobile
#endif
                break;
            case 1: // High
                QualitySettings.SetQualityLevel(2); // 예: High
                break;
            case 2: // Middle
                QualitySettings.SetQualityLevel(3); // 예: Middle
                break;
            case 3: //Low
                QualitySettings.SetQualityLevel(4); // 예: Low
                break;

            default:
                Debug.LogError("해당 사항 없음");
                break;
        }
    }
}
