using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioSource 풀: 위치 기반(3D) 또는 2D SFX 재생에 사용
/// PlayOneShot 대신 개별 AudioSource로 재생하여 개별 제어 가능
/// 개선: 재생을 중간에 멈추거나 특정 시간만 재생, 페이드 아웃 등 제어 가능
/// </summary>
public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private int initialSize = 12;
    [SerializeField] private AudioMixerGroup outputMixerGroup = null;

    private Queue<AudioSource> _pool = new Queue<AudioSource>();
    private HashSet<AudioSource> _inUse = new HashSet<AudioSource>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
            _pool.Enqueue(CreateNewSource());
    }

    private AudioSource CreateNewSource()
    {
        var go = new GameObject("PooledAudioSource");
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        if (outputMixerGroup != null) src.outputAudioMixerGroup = outputMixerGroup;
        return src;
    }

    /// <summary>
    /// 풀에서 AudioSource를 가져오고 "in use"로 표시합니다.
    /// </summary>
    public AudioSource Get()
    {
        AudioSource src;
        if (_pool.Count == 0) src = CreateNewSource();
        else src = _pool.Dequeue();

        _inUse.Add(src);
        src.transform.SetParent(null); // 외부에서 위치를 설정하기 쉬움
        return src;
    }

    /// <summary>
    /// AudioSource를 풀로 반환합니다. 여러번 호출되어도 안전하도록 _inUse 체크를 합니다.
    /// </summary>
    public void Return(AudioSource src)
    {
        if (src == null) return;
        if (!_inUse.Contains(src)) return; // 이미 반환된 경우 무시

        _inUse.Remove(src);

        src.clip = null;
        src.loop = false;
        src.spatialBlend = 0f;
        src.volume = 1f;
        src.pitch = 1f;
        src.transform.SetParent(transform);
        src.Stop();

        _pool.Enqueue(src);
    }

    /// <summary>
    /// 기본 단발 재생. 반환되는 AudioSource를 통해 나중에 중단하거나 시간 이동 가능.
    /// </summary>
    public AudioSource PlayOneShotAt(AudioClip clip, Vector3 position, bool isLoop = false, float volume = 1f, float pitch = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return null;
        var src = Get();
        src.transform.position = position;
        src.spatialBlend = Mathf.Clamp01(spatialBlend); // 0 = 2D, 1 = 3D
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Clamp(pitch, -3f, 3f);
        src.clip = clip;
        src.loop = isLoop;
        src.Play();
        StartCoroutine(ReturnWhenFinished(src, clip.length / Mathf.Abs(src.pitch)));
        return src;
    }

    /// <summary>
    /// 클립을 재생하되 특정 시간(seconds) 후에 자동으로 중단하고 반환합니다.
    /// pitch에 관계없이 seconds로 제어됩니다.
    /// </summary>
    public AudioSource PlayForSecondsAt(AudioClip clip, Vector3 position, float seconds, bool isLoop = false, float volume = 1f, float pitch = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return null;
        var src = Get();
        src.transform.position = position;
        src.spatialBlend = Mathf.Clamp01(spatialBlend);
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Clamp(pitch, -3f, 3f);
        src.clip = clip;
        src.loop = isLoop;
        src.Play();
        StartCoroutine(StopAfterSeconds(src, seconds));
        return src;
    }

    /// <summary>
    /// 외부에서 재생 중인 AudioSource를 즉시 정지하고 풀로 반환합니다.
    /// 안전하게 여러 번 호출되어도 됩니다.
    /// </summary>
    public void StopAndReturn(AudioSource src)
    {
        if (src == null) return;
        if (!_inUse.Contains(src)) return;
        src.Stop();
        Return(src);
    }

    /// <summary>
    /// 부드럽게 페이드아웃 한 뒤 반환합니다.
    /// </summary>
    public void FadeOutAndReturn(AudioSource src, float fadeDuration)
    {
        if (src == null) return;
        if (!_inUse.Contains(src)) return;
        StartCoroutine(FadeOutCoroutine(src, fadeDuration));
    }

    private IEnumerator ReturnWhenFinished(AudioSource src, float delay)
    {
        // 안전 마진 추가
        yield return new WaitForSeconds(Mathf.Max(0f, delay) + 0.05f);
        // 이미 수동으로 반환되었을 수 있으니 체크
        if (_inUse.Contains(src))
            Return(src);
    }

    private IEnumerator StopAfterSeconds(AudioSource src, float seconds)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, seconds));
        if (_inUse.Contains(src))
        {
            src.Stop();
            Return(src);
        }
    }

    private IEnumerator FadeOutCoroutine(AudioSource src, float duration)
    {
        if (duration <= 0f)
        {
            // 즉시 반환
            StopAndReturn(src);
            yield break;
        }

        float startVolume = src.volume;
        float t = 0f;
        while (t < duration)
        {
            if (!_inUse.Contains(src))
                yield break; // 이미 반환되었으면 중단

            t += Time.deltaTime;
            src.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        // 마지막으로 멈추고 반환
        if (_inUse.Contains(src))
        {
            src.Stop();
            Return(src);
        }
    }
}