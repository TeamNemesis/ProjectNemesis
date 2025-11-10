using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 설정창에서 볼륨 조절 값을 받아 오디오에 적용하는 클래스
/// </summary>
public class VolumeChanger : MonoBehaviour
{
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _bgmVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;

    public void OnMasterVolumeChanged(float value)
    {
        Debug.Log(value);
        GameManager.Instance.SoundManager.SetMasterVolume(value);
    }
    
    public void OnBGMVolumeChanged(float value)
    {
        GameManager.Instance.SoundManager.SetBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        GameManager.Instance.SoundManager.SetSFXVolume(value);
    }
}
