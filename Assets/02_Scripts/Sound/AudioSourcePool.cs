using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioSource 풀: 위치 기반(3D) 또는 2D SFX 재생에 사용
/// PlayOneShot 대신 개별 AudioSource로 재생하여 개별 제어 가능
/// </summary>
public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private int initialSize = 12;
    [SerializeField] private AudioMixerGroup outputMixerGroup = null;

    private Queue<AudioSource> _pool = new Queue<AudioSource>();

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

    public AudioSource Get()
    {
        if (_pool.Count == 0) return CreateNewSource();
        return _pool.Dequeue();
    }

    public void Return(AudioSource src)
    {
        if (src == null) return;
        src.clip = null;
        src.loop = false;
        src.spatialBlend = 0f;
        src.volume = 1f;
        src.pitch = 1f;
        src.transform.SetParent(transform);
        src.Stop();
        _pool.Enqueue(src);
    }

    public void PlayOneShotAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return;
        var src = Get();
        src.transform.position = position;
        src.spatialBlend = Mathf.Clamp01(spatialBlend); // 0 = 2D, 1 = 3D
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Clamp(pitch, -3f, 3f);
        src.clip = clip;
        src.loop = false;
        src.Play();
        StartCoroutine(ReturnWhenFinished(src, clip.length / Mathf.Abs(src.pitch)));
    }

    private IEnumerator ReturnWhenFinished(AudioSource src, float delay)
    {
        // 안전 마진 추가
        yield return new WaitForSeconds(delay + 0.05f);
        Return(src);
    }
}