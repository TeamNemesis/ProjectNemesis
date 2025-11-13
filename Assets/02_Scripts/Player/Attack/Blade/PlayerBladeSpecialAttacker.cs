using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 검기(Blade) 특수공격 구현
/// - 차지 시간에 따라 크기(스케일)와 대미지 비례.
/// - StartCharge / StopChargeAndFire / CancelCharge 를 오버라이드하여 코루틴으로 차지 처리.
/// </summary>
public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    [Header("Blade Settings")]
    [SerializeField] GameObject bladePrefab;            // 발사할 검기 프리팹 (BladeProjectile 컴포넌트 포함 권장)
    [SerializeField] Transform spawnPoint;              // 발사 위치(없으면 this.transform 사용)
    [SerializeField] float maxChargeTime = 2.0f;        // 완전 차지까지 시간(초)
    [SerializeField] float minScale = 0.6f;             // 최소 스케일
    [SerializeField] float maxScale = 2.0f;             // 최대 스케일
    [SerializeField] float baseDamage = 10f;            // 차지 0일 때 대미지
    [SerializeField] float maxDamage = 35f;             // 완전 차지 시 대미지
    [SerializeField] float bladeSpeed = 12f;            // 발사 속도
    [SerializeField] float bladeLifetime = 3f;          // 검기 수명

    Coroutine _chargeCoroutine;
    float _currentChargeRatio = 0f; // 0..1

    // 이벤트 오버라이드
    public override event Action OnSpecialStarted;
    public override event Action<float> OnSpecialChargeUpdated; // ratio 0..1
    public override event Action OnSpecialFired;
    public override event Action OnSpecialEnded;
    public override event Action OnSpecialCancelled;

    public override WeaponType WeaponType => WeaponType.Blade;

    public override void Initialize(Player player)
    {
        base.Initialize(player);
    }

    public override bool RequestSpecial()
    {
        // 예: 쿨타임이나 다른 조건을 추가하고 싶으면 여기서 처리
        return base.RequestSpecial();
    }

    public override void StartCharge()
    {
        if (_player == null)
        {
            Debug.LogWarning("PlayerBladeSpecialAttacker: owner is null. Call Initialize first.");
        }

        if (IsCharging || IsActive) return;

        IsCharging = true;
        OnSpecialStarted?.Invoke();

        // 시작하면 기존 코루틴 정리 후 새로 시작
        if (_chargeCoroutine != null) StopCoroutine(_chargeCoroutine);
        _chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    public override void StopChargeAndFire()
    {
        // 버튼 해제 시 수동 발사
        if (!IsCharging) return;

        // 정지하고 현재 ratio로 발사
        IsCharging = false;
        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;
        }

        Fire();

        OnSpecialFired?.Invoke();
        EndSpecial();
    }

    public override void CancelCharge()
    {
        // 강제 취소 (피격 등)
        if (!IsCharging) return;

        IsCharging = false;
        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;
        }

        OnSpecialCancelled?.Invoke();
        EndSpecial();
    }

    protected override void Fire()
    {
        // 발사 시작
        IsActive = true;

        if (bladePrefab == null)
        {
            Debug.LogWarning("PlayerBladeSpecialAttacker: bladePrefab is not set.");
            IsActive = false;
            return;
        }

        Vector3 spawnPos = (spawnPoint != null) ? spawnPoint.position : transform.position;
        Quaternion spawnRot = (spawnPoint != null) ? spawnPoint.rotation : transform.rotation;

        // 스케일/데미지 계산
        float scale = Mathf.Lerp(minScale, maxScale, _currentChargeRatio);
        float damage = Mathf.Lerp(baseDamage, maxDamage, _currentChargeRatio);

        GameObject bladeObj = Instantiate(bladePrefab, spawnPos, spawnRot);
        // BladeProjectile 스크립트가 있다면 초기화
        var blade = bladeObj.GetComponent<BladeProjectile>();
        if (blade != null)
        {
            //blade.Init(damage, bladeSpeed, bladeLifetime, scale, _player.gameObject);
        }
        else
        {
            // 없으면 기본적으로 스케일 적용하고 forward 방향으로 이동시키는 간단한 처리
            bladeObj.transform.localScale = Vector3.one * scale;
            var rb = bladeObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = bladeObj.transform.forward * bladeSpeed;
            }
            Destroy(bladeObj, bladeLifetime);
        }
    }

    IEnumerator ChargeRoutine()
    {
        float elapsed = 0f;
        _currentChargeRatio = 0f;
        RaiseChargeUpdated(_currentChargeRatio);

        while (IsCharging)
        {
            elapsed += Time.deltaTime;
            _currentChargeRatio = Mathf.Clamp01(elapsed / maxChargeTime);
            RaiseChargeUpdated(_currentChargeRatio);

            // 자동 발사: 최대 차지에 도달하면 자동으로 발사
            if (elapsed >= maxChargeTime)
            {
                IsCharging = false;
                _chargeCoroutine = null;

                Fire();
                OnSpecialFired?.Invoke();
                EndSpecial();
                yield break;
            }

            yield return null;
        }
    }

    protected override void EndSpecial()
    {
        base.EndSpecial();
        // 정리
        _currentChargeRatio = 0f;
        RaiseChargeUpdated(_currentChargeRatio);
        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;
        }
    }
}