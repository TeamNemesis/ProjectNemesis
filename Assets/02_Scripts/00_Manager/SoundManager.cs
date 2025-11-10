using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 모든 사운드를 관리하는 매니저
/// BGM과 효과음(SFX)을 재생, 정지, 볼륨 조절 등을 담당
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource _bgmSources;      // BGM 전용 AudioSource
    [SerializeField] AudioSource[] _sfxSources;   // 효과음 전용 AudioSource 배열

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] float _bgmVolume = 1f;   // BGM 볼륨 (0.0 ~ 1.0)
    [Range(0f, 1f)] [SerializeField] float _sfxVolume = 1f;   // 효과음 볼륨 (0.0 ~ 1.0)

    [Header("Audio Clips")]
    Dictionary<string, AudioClip> _bgmClips = new Dictionary<string, AudioClip>();  // BGM 클립 딕셔너리
    Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();  // 효과음 클립 딕셔너리

    public void Initialize(ResourceManager resourceManager)
    {
        // BGM 컴포넌트 찾아오기
        if (_bgmSources == null)
        {
            _bgmSources = gameObject.AddComponent<AudioSource>();
            _bgmSources.loop = true; // BGM은 기본적으로 루프
        }
        if (_sfxSources == null)
        {
            _sfxSources = new AudioSource[5]; // 기본적으로 5개의 SFX 소스 생성
            for (int i = 0; i < _sfxSources.Length; i++)
            {
                _sfxSources[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        // 오디오 클립 로드 및 딕셔너리 초기화
        _bgmClips.Clear();
        _sfxClips.Clear();

        foreach (var clip in resourceManager.Bgm)
        {
            _bgmClips[clip.name] = clip;
        }

        foreach(var clip in resourceManager.Sfx)
        {
            _sfxClips[clip.name] = clip;
        }

        // 볼륨 세팅
        SetBGMVolume(_bgmVolume);
        SetSFXVolume(_sfxVolume);
    }

    public void PlayBGM(string bgmName)
    {
        // 현재 재생 중인 BGM이 있다면 중지
        if (_bgmSources.isPlaying)
        {
            _bgmSources.Stop();
        }

        if (_bgmClips.TryGetValue(bgmName, out var clip))
        {
            _bgmSources.clip = clip;
            _bgmSources.volume = _bgmVolume;
            _bgmSources.loop = true;
            _bgmSources.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{bgmName}' not found!");
        }
    }

    public void PlayOneShot(string sfxName)
    {
        if (_sfxClips.TryGetValue(sfxName, out var clip))
        {
            // 사용 가능한 AudioSource 찾기
            foreach (var sfxSource in _sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    sfxSource.clip = clip;
                    sfxSource.volume = _sfxVolume;
                    sfxSource.Play();
                    return;
                }
            }
            // 모든 AudioSource가 사용 중일 경우, 첫 번째 소스를 사용
            _sfxSources[0].clip = clip;
            _sfxSources[0].volume = _sfxVolume;
            _sfxSources[0].Play();
        }
        else
        {
            Debug.LogWarning($"SFX '{sfxName}' not found!");
        }
    }

    public void SetMasterVolume(float volume)
    {
        SetBGMVolume(volume);
        SetSFXVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        _bgmSources.volume = _bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        foreach (var sfxSource in _sfxSources)
        {
            sfxSource.volume = _sfxVolume;
        }
    }
}