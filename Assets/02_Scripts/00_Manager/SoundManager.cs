using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 모든 사운드를 관리하는 매니저
/// BGM과 효과음(SFX)을 재생, 정지, 볼륨 조절 등을 담당
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] AudioSource _bgmSource;      // BGM 전용 AudioSource

    [Header("SFX")]
    [SerializeField] AudioSourcePool _sfxSourcePool; // SFX 재생용 AudioSource 풀
    [SerializeField] AudioSource _fallbackSfxSource; // 풀 없을 때 사용할 폴백 AudioSource (PlayOneShot 용)

    [Header("Volume Settings")]
    [Range(0f, 1f)][SerializeField] float _masterVolume = 1f; // 마스터 볼륨 (0.0 ~ 1.0)    
    [Range(0f, 1f)][SerializeField] float _bgmVolume = 1f;   // BGM 볼륨 (0.0 ~ 1.0)
    [Range(0f, 1f)][SerializeField] float _sfxVolume = 1f;   // 효과음 볼륨 (0.0 ~ 1.0)

    [Header("Audio Clips")]
    Dictionary<string, AudioClip> _bgmClips = new Dictionary<string, AudioClip>();  // BGM 클립 딕셔너리
    Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();  // 효과음 클립 딕셔너리

    /// <summary>
    /// 리소스 매니저로부터 클립을 받아 초기화합니다.
    /// 호출 전에 ResourceManager가 준비되어 있어야 합니다.
    /// </summary>
    public void Initialize(ResourceManager resourceManager)
    {
        if (resourceManager == null)
        {
            Debug.LogWarning("SoundManager.Initialize: resourceManager is null");
            return;
        }

        // BGM AudioSource가 없으면 자동 생성
        if (_bgmSource == null)
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
        }

        // 폴백 SFX AudioSource가 없으면 생성
        if (_fallbackSfxSource == null)
        {
            var go = new GameObject("FallbackSfxSource");
            go.transform.SetParent(transform);
            _fallbackSfxSource = go.AddComponent<AudioSource>();
            _fallbackSfxSource.playOnAwake = false;
        }

        // SFX 풀이 없으면 자식에서 찾아봄(선택적)
        if (_sfxSourcePool == null)
        {
            _sfxSourcePool = GetComponentInChildren<AudioSourcePool>();
        }

        // 딕셔너리 초기화
        _bgmClips.Clear();
        _sfxClips.Clear();

        if (resourceManager.Bgm != null)
        {
            foreach (var clip in resourceManager.Bgm)
            {
                if (clip == null) continue;
                _bgmClips[clip.name] = clip;
            }
        }

        if (resourceManager.Sfx != null)
        {
            foreach (var clip in resourceManager.Sfx)
            {
                if (clip == null) continue;
                _sfxClips[clip.name] = clip;
            }
        }

        // 볼륨 적용
        ApplyBgmVolume();
        ApplySfxVolume();
    }

    public void PlayBGM(string bgmName)
    {
        if (_bgmSource == null)
        {
            Debug.LogWarning("PlayBGM: _bgmSource is null");
            return;
        }

        if (_bgmClips.TryGetValue(bgmName, out var clip))
        {
            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            ApplyBgmVolume();
            _bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{bgmName}' not found!");
        }
    }

    /// <summary>
    /// UI용 2D SFX 재생 (위치 없음)
    /// </summary>
    public void PlaySfx(string sfxName, float volume = 1f, float pitch = 1f)
    {
        if (!_sfxClips.TryGetValue(sfxName, out var clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' not found");
            return;
        }

        float finalVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        float finalPitch = Mathf.Clamp(pitch, -3f, 3f);

        if (_sfxSourcePool != null)
        {
            _sfxSourcePool.PlayOneShotAt(clip, Vector3.zero, finalVolume, finalPitch, 0f);
        }
        else if (_fallbackSfxSource != null)
        {
            _fallbackSfxSource.pitch = finalPitch;
            _fallbackSfxSource.PlayOneShot(clip, finalVolume);
        }
    }

    /// <summary>
    /// 월드 위치 기반 SFX 재생
    /// </summary>
    public void PlaySfxAt(string sfxName, Vector3 pos, float volume = 1f, float pitch = 1f)
    {
        if (!_sfxClips.TryGetValue(sfxName, out var clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' not found");
            return;
        }

        float finalVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        float finalPitch = Mathf.Clamp(pitch, -3f, 3f);

        if (_sfxSourcePool != null)
        {
            _sfxSourcePool.PlayOneShotAt(clip, pos, finalVolume, finalPitch, 1f);
        }
        else if (_fallbackSfxSource != null)
        {
            // fallback는 2D이므로 위치 기반 3D 재생을 지원하지 않음 — 로그로 경고하고 2D로 재생
            Debug.LogWarning("PlaySfxAt: _sfxSourcePool is null, playing fallback 2D sound.");
            _fallbackSfxSource.pitch = finalPitch;
            _fallbackSfxSource.PlayOneShot(clip, finalVolume);
        }
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        ApplyBgmVolume();
        ApplySfxVolume();
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        ApplyBgmVolume();
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        ApplySfxVolume();
    }

    // 내부용: 실제 AudioSource에 적용
    void ApplyBgmVolume()
    {
        if (_bgmSource != null)
            _bgmSource.volume = _masterVolume * _bgmVolume;
    }

    void ApplySfxVolume()
    {
        // 풀의 재생은 PlayOneShotAt에서 전달된 volume을 사용하므로,
        // 폴백 소스의 기본 볼륨만 설정해둡니다.
        if (_fallbackSfxSource != null)
            _fallbackSfxSource.volume = _masterVolume * _sfxVolume;
        // (AudioSourcePool에서 전역 볼륨을 따로 지원한다면 여기서 설정을 전달)
    }
}