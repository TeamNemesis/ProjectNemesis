using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 모든 사운드를 관리하는 매니저
/// BGM과 효과음(SFX)을 재생, 정지, 볼륨 조절 등을 담당
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;      // BGM 전용 AudioSource
    [SerializeField] private AudioSource[] sfxSources;   // 효과음 전용 AudioSource 배열

    [Header("Settings")]
    [SerializeField] private int sfxSourceCount = 10;    // 동시에 재생 가능한 효과음 개수

    private Coroutine currentFadeCoroutine;

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.7f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
    private int currentSfxIndex = 0;  // 다음에 사용할 효과음 소스 인덱스

    /// <summary>
    /// SoundManager 초기화
    /// </summary>
    public void Initialize()
    {
        // BGM AudioSource 생성
        if (bgmSource == null)
        {
            GameObject bgmObj = new GameObject("BGM_Source");
            bgmObj.transform.SetParent(transform);
            bgmSource = bgmObj.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        //효과음 AudioSource들 생성 (null 체크 추가)
        if (sfxSources == null || sfxSources.Length == 0)
        {
            sfxSources = new AudioSource[sfxSourceCount];
            for (int i = 0; i < sfxSourceCount; i++)
            {
                GameObject sfxObj = new GameObject($"SFX_Source_{i}");
                sfxObj.transform.SetParent(transform);
                sfxSources[i] = sfxObj.AddComponent<AudioSource>();
                sfxSources[i].playOnAwake = false;
            }
        }

        UpdateVolumes();
    }

    #region BGM 관련 메서드

    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="clipName">재생할 오디오 클립 이름 (Resources 폴더 내 경로)</param>
    /// <param name="loop">반복 재생 여부</param>
    public void PlayBGM(string clipName, bool loop = true)
    {
        AudioClip clip = LoadAudioClip(clipName);
        if (clip != null)
        {
            PlayBGM(clip, loop);
        }
        else
        {
            Debug.LogWarning($"BGM을 찾을 수 없습니다: {clipName}");
        }
    }

    /// <summary>
    /// BGM 재생 (AudioClip 직접 전달)
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("재생할 BGM AudioClip이 null입니다.");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// BGM 일시정지
    /// </summary>
    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    /// <summary>
    /// BGM 재개
    /// </summary>
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    /// <summary>
    /// BGM 페이드 아웃
    /// </summary>
    public void FadeOutBGM(float duration = 1f)
    {
        // 이전 페이드 중단
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    /// <summary>
    /// BGM 페이드 인
    /// </summary>
    public void FadeInBGM(string clipName, float duration = 1f, bool loop = true)
    {
        AudioClip clip = LoadAudioClip(clipName);
        if (clip != null)
        {
            // 이전 페이드 중단
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }
            currentFadeCoroutine = StartCoroutine(FadeInCoroutine(clip, duration, loop));
        }
    }

    #endregion

    #region 효과음(SFX) 관련 메서드

    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="clipName">재생할 오디오 클립 이름</param>
    /// <param name="volume">볼륨 (0~1, 기본값은 설정된 sfxVolume 사용)</param>
    public void PlaySFX(string clipName, float volume = -1f)
    {
        AudioClip clip = LoadAudioClip(clipName);
        if (clip != null)
        {
            PlaySFX(clip, volume);
        }
        else
        {
            Debug.LogWarning($"효과음을 찾을 수 없습니다: {clipName}");
        }
    }

    /// <summary>
    /// 효과음 재생 (AudioClip 직접 전달)
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = -1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("재생할 SFX AudioClip이 null입니다.");
            return;
        }

        // 사용할 AudioSource 찾기
        AudioSource source = GetAvailableSFXSource();

        // 볼륨 설정 (커스텀 볼륨이 지정되지 않으면 기본 sfxVolume 사용)
        float playVolume = volume < 0 ? sfxVolume * masterVolume : volume * masterVolume;

        source.PlayOneShot(clip, playVolume);
    }

    /// <summary>
    /// 특정 위치에서 3D 효과음 재생
    /// </summary>
    public void PlaySFX3D(string clipName, Vector3 position, float volume = -1f)
    {
        AudioClip clip = LoadAudioClip(clipName);
        if (clip != null)
        {
            float playVolume = volume < 0 ? sfxVolume * masterVolume : volume * masterVolume;
            AudioSource.PlayClipAtPoint(clip, position, playVolume);
        }
    }

    /// <summary>
    /// 모든 효과음 정지
    /// </summary>
    public void StopAllSFX()
    {
        foreach (AudioSource source in sfxSources)
        {
            source.Stop();
        }
    }

    #endregion

    #region 볼륨 관련 메서드

    /// <summary>
    /// 마스터 볼륨 설정
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume * masterVolume;
    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 모든 AudioSource의 볼륨 업데이트
    /// </summary>
    private void UpdateVolumes()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
    }

    public float GetMasterVolume() => masterVolume;
    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;

    #endregion

    #region 유틸리티 메서드

    /// <summary>
    /// AudioClip 로드 (캐싱 사용)
    /// </summary>
    private AudioClip LoadAudioClip(string clipName)
    {
        // 캐시에 있으면 반환
        if (audioClipCache.ContainsKey(clipName))
        {
            return audioClipCache[clipName];
        }

        // Resources 폴더에서 로드
        AudioClip clip = Resources.Load<AudioClip>(clipName);

        if (clip != null)
        {
            audioClipCache.Add(clipName, clip);
        }

        return clip;
    }

    /// <summary>
    /// 사용 가능한 효과음 AudioSource 가져오기
    /// </summary>
    private AudioSource GetAvailableSFXSource()
    {
        // 현재 재생중이지 않은 소스 찾기
        for (int i = 0; i < sfxSources.Length; i++)
        {
            int index = (currentSfxIndex + i) % sfxSources.Length;
            if (!sfxSources[index].isPlaying)
            {
                currentSfxIndex = (index + 1) % sfxSources.Length;
                return sfxSources[index];
            }
        }

        // 모두 재생중이면 순환하며 사용
        AudioSource source = sfxSources[currentSfxIndex];
        currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Length;
        return source;
    }

    /// <summary>
    /// 오디오 클립 캐시 삭제
    /// </summary>
    public void ClearCache()
    {
        audioClipCache.Clear();
    }

    #endregion

    #region 코루틴

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();
        bgmSource.volume = bgmVolume * masterVolume;

        currentFadeCoroutine = null;
    }

    private System.Collections.IEnumerator FadeInCoroutine(AudioClip clip, float duration, bool loop)
    {
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float targetVolume = bgmVolume * masterVolume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        bgmSource.volume = targetVolume;

        currentFadeCoroutine = null;
    }

    #endregion


    private void OnDisable()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }
    }
}