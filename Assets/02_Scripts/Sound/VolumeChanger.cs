using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 설정창에서 볼륨 조절 값을 받아 오디오에 적용하는 클래스
/// </summary>
public class VolumeChanger : MonoBehaviour
{
    [SerializeField] Scrollbar _masterVolumeScrollbar;
    [SerializeField] Scrollbar _bgmVolumeScrollbar;
    [SerializeField] Scrollbar _sfxVolumeScrollbar;

    private void Start()
    {
        if (_masterVolumeScrollbar != null)
        {
            OnMasterVolumeChanged();
            OnBGMVolumeChanged();
            OnSFXVolumeChanged();
        }
    }

    public void OnMasterVolumeChanged()
    {
        GameManager.Instance.SoundManager.SetMasterVolume(_masterVolumeScrollbar.value);
    }
    
    public void OnBGMVolumeChanged()
    {
        GameManager.Instance.SoundManager.SetBGMVolume(_bgmVolumeScrollbar.value);
    }

    public void OnSFXVolumeChanged()
    {
        GameManager.Instance.SoundManager.SetSFXVolume(_sfxVolumeScrollbar.value);
    }
}
