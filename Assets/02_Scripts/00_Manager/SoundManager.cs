using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 모든 사운드를 관리하는 매니저
/// BGM과 효과음(SFX)을 재생, 정지, 볼륨 조절 등을 담당
/// 추가된 기능:
/// - SFX 재생 시 AudioSource 핸들 반환(풀 사용 시) — 중간 정지, 시간 제한 재생, 페이드아웃 가능
/// - PlayForSeconds (지정 시간만 재생)
/// - StopSfx / FadeOutSfx 유틸 제공
/// 주의:
/// - AudioSourcePool이 없으면 폴백(_fallbackSfxSource)으로 PlayOneShot만 가능. 이 경우 개별 제어(시간 제한/페이드/정지)는 권장되지 않음.
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
            _fallbackSfxSource.spatialBlend = 0f; // 2D
        }

        // SFX 풀이 없으면 생성
        if (_sfxSourcePool == null)
        {
            var go = new GameObject("SfxSourcePool");
            go.transform.SetParent(transform);
            _sfxSourcePool = go.AddComponent<AudioSourcePool>();
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
    /// 반환값: 풀을 사용하면 재생에 사용된 AudioSource(제어 가능)를 반환합니다. 폴백 사용 시 null 반환(개별 제어 불가 또는 공유).
    /// </summary>
    public AudioSource PlaySfx(string sfxName, bool isLoop=false, float volume = 1f, float pitch = 1f)
    {
        if (!_sfxClips.TryGetValue(sfxName, out var clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' not found");
            return null;
        }

        float finalVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        float finalPitch = Mathf.Clamp(pitch, -3f, 3f);

        if (_sfxSourcePool != null)
        {
            // spatialBlend 0 -> 2D
            return _sfxSourcePool.PlayOneShotAt(clip, Vector3.zero, isLoop, finalVolume, finalPitch, 0f);
        }
        else if (_fallbackSfxSource != null)
        {
            // 폴백은 PlayOneShot로 재생 — 공유 오디오소스이므로 개별 제어가 어렵습니다.
            _fallbackSfxSource.pitch = finalPitch;
            _fallbackSfxSource.PlayOneShot(clip, finalVolume);
            Debug.LogWarning("PlaySfx: using fallback source. Returned AudioSource is not controllable per-instance.");
            return null;
        }

        return null;
    }

    /// <summary>
    /// 월드 위치 기반 SFX 재생
    /// 반환값: 풀을 사용하면 재생에 사용된 AudioSource(제어 가능)를 반환합니다. 폴백 사용 시 null 반환.
    /// </summary>
    public AudioSource PlaySfxAt(string sfxName, Vector3 pos, bool isLoop=false, float volume = 1f, float pitch = 1f)
    {
        if (!_sfxClips.TryGetValue(sfxName, out var clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' not found");
            return null;
        }

        float finalVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        float finalPitch = Mathf.Clamp(pitch, -3f, 3f);

        if (_sfxSourcePool != null)
        {
            return _sfxSourcePool.PlayOneShotAt(clip, pos, isLoop, finalVolume, finalPitch, 1f);
        }
        else if (_fallbackSfxSource != null)
        {
            Debug.LogWarning("PlaySfxAt: _sfxSourcePool is null, playing fallback 2D sound.");
            _fallbackSfxSource.pitch = finalPitch;
            _fallbackSfxSource.PlayOneShot(clip, finalVolume);
            return null;
        }

        return null;
    }

    /// <summary>
    /// 클립을 재생하되 지정한 seconds 만큼만 재생하고 자동으로 정지/반환합니다.
    /// 반환값: 풀 사용 시 제어 가능한 AudioSource 반환. 폴백 사용 시 null 반환.
    /// </summary>
    public AudioSource PlaySfxForSeconds(string sfxName, float seconds, bool isLoop=false, float volume = 1f, float pitch = 1f)
    {
        if (!_sfxClips.TryGetValue(sfxName, out var clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' not found");
            return null;
        }

        float finalVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        float finalPitch = Mathf.Clamp(pitch, -3f, 3f);

        if (_sfxSourcePool != null)
        {
            // 2D
            return _sfxSourcePool.PlayForSecondsAt(clip, Vector3.zero, seconds, isLoop, finalVolume, finalPitch, 0f);
        }
        else
        {
            Debug.LogWarning("PlaySfxForSeconds: no AudioSourcePool available — cannot safely play-for-seconds with fallback.");
            return null;
        }
    }

    /// <summary>
    /// 위치 기반으로 seconds 만큼만 재생.
    /// 반환값: 풀 사용 시 제어 가능한 AudioSource 반환. 폴백 사용 시 null 반환.
    /// </summary>
    public AudioSource PlaySfxForSecondsAt(string sfxName, Vector3 pos, float seconds, bool isLoop = false, float volume = 1f, float pitch = 1f)
    {
        if (!_sfxClips.TryGetValue(sfxName, out var clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' not found");
            return null;
        }

        float finalVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        float finalPitch = Mathf.Clamp(pitch, -3f, 3f);

        if (_sfxSourcePool != null)
        {
            return _sfxSourcePool.PlayForSecondsAt(clip, pos, seconds, isLoop, finalVolume, finalPitch, 1f);
        }
        else
        {
            Debug.LogWarning("PlaySfxForSecondsAt: no AudioSourcePool available — cannot safely play-for-seconds with fallback.");
            return null;
        }
    }

    /// <summary>
    /// 특정 AudioSource(핸들)를 즉시 정지하고 반환합니다.
    /// AudioSource가 null이거나 폴백으로 재생된 경우에는 적절히 처리합니다.
    /// </summary>
    public void StopSfx(AudioSource src)
    {
        if (src == null)
        {
            Debug.LogWarning("StopSfx: null AudioSource");
            return;
        }

        if (_sfxSourcePool != null)
        {
            _sfxSourcePool.StopAndReturn(src);
            return;
        }

        // 풀 자체가 없고 src가 폴백 소스라면 정지 가능하지만, 폴백은 공유이므로 다른 사운드도 중단될 수 있음
        if (src == _fallbackSfxSource)
        {
            src.Stop();
            return;
        }

        Debug.LogWarning("StopSfx: AudioSourcePool is null and the provided AudioSource is not the fallback source. Cannot safely stop.");
    }

    /// <summary>
    /// 지정한 AudioSource를 fade out 한 뒤 반환합니다.
    /// </summary>
    public void FadeOutSfx(AudioSource src, float fadeDuration)
    {
        if (src == null)
        {
            Debug.LogWarning("FadeOutSfx: null AudioSource");
            return;
        }

        if (_sfxSourcePool != null)
        {
            _sfxSourcePool.FadeOutAndReturn(src, fadeDuration);
            return;
        }

        // fallback 소스에 대해서는 매니저가 직접 페이드 수행 (주의: 공유 소스)
        if (src == _fallbackSfxSource)
        {
            StartCoroutine(FadeOutFallback(src, fadeDuration));
            return;
        }

        Debug.LogWarning("FadeOutSfx: AudioSourcePool is null and the provided AudioSource is not the fallback source. Cannot safely fade.");
    }

    IEnumerator FadeOutFallback(AudioSource src, float duration)
    {
        if (src == null) yield break;
        float startVolume = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(startVolume, 0f, t / Mathf.Max(0.0001f, duration));
            yield return null;
        }
        src.Stop();
        src.volume = startVolume; // 복구 (다음 재생 시 정상 볼륨 적용)
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
        // 풀로 재생할 때는 Play 호출 시 최종 볼륨을 전달하므로 여기선 폴백의 기본 볼륨만 설정
        if (_fallbackSfxSource != null)
            _fallbackSfxSource.volume = _masterVolume * _sfxVolume;
    }
}