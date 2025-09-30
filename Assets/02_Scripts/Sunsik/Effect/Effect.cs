using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    ParticleSystem _ps;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();    
    }

    /// <summary>
    /// 이펙트를 재생하는 함수
    /// </summary>
    public void Play()
    {
        _ps.Play(true);
    }

    private void OnParticleSystemStopped()
    {
        gameObject.DestroyORReturnToPool();
    }
}
