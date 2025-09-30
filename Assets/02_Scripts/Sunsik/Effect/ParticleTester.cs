using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTester : MonoBehaviour
{
    ParticleSystem _ps;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        ParticleSystem.MainModule main = _ps.main;
        main.duration = 2f;

        ParticleSystem.ShapeModule shape = _ps.shape;
        shape.radius = 0.1f;
    }

    private void OnParticleSystemStopped()
    {
        gameObject.DestroyORReturnToPool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 파티클 시스템이 파티클들을 생성하도록 시작
            _ps.Play(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 파티클 시스템이 파티클을 더 생성하지 않게 정지
            _ps.Stop(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 일시 정지
            _ps.Pause(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // 생성된 파티클들을 전부 제거
            _ps.Clear(true);
        }
    }
}
