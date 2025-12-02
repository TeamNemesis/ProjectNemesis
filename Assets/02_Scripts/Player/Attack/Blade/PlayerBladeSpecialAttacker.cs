using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 검기(Blade) 특수공격을 "가장 가까운 적에게 순간이동" 스킬로 변경한 구현
/// - 차지 시간에 따라 목표 근처로 더 가깝게 순간이동(0..1 -> 멀리..가까이)
/// - 몬스터가 없으면 기존 검기 발사 동작을 폴백으로 사용
/// </summary>
public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    [Header("Blade / Teleport Settings")]
    [SerializeField] GameObject bladePrefab;            // (선택) 발사할 검기 프리팹 (폴백)
    [SerializeField] Transform spawnPoint;              // 발사 위치(없으면 this.transform 사용)
    [SerializeField] float maxChargeTime = 2.0f;        // 완전 차지까지 시간(초)
    [SerializeField] float minScale = 0.6f;             // (기존) 최소 스케일 (폴백 사용 가능)
    [SerializeField] float maxScale = 2.0f;             // (기존) 최대 스케일 (폴백 사용 가능)
    [SerializeField] float baseDamage = 10f;            // (기존) 차지 0일 때 대미지
    [SerializeField] float maxDamage = 35f;             // (기존) 완전 차지 시 대미지

    // Teleport-specific tuning: 차지 비율 0 -> minTeleportDistance, 1 -> maxTeleportDistance (작을수록 적에 더 근접)
    [Header("Teleport Tuning")]
    [SerializeField] float minTeleportDistanceFromMonster = 1.5f; // 차지 0일 때 몬스터로부터의 거리
    [SerializeField] float maxTeleportDistanceFromMonster = 0.4f; // 완전 차지일 때 몬스터로부터의 거리
    [SerializeField] float teleportHeightOffset = 0.2f; // 순간이동 시 높이 보정

    [SerializeField] float bladeSpeed = 12f;            // (폴백) 발사 속도
    [SerializeField] float bladeLifetime = 3f;          // (폴백) 검기 수명

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

        if (_player == null)
        {
            Debug.LogWarning("PlayerBladeSpecialAttacker: player is null. Call Initialize first.");
            IsActive = false;
            return;
        }

        // 가장 가까운 몬스터 찾기 (EventBus에 구현되어 있는 헬퍼 사용)
        Transform nearest = EventBus.GetNearestMonsterFromMe(_player.transform);

        if (nearest == null)
        {
            // 폴백: 몬스터가 없으면 기존 검기 발사 동작 유지
            Debug.Log("PlayerBladeSpecialAttacker: No monster found. Falling back to blade projectile.");

            if (bladePrefab == null)
            {
                Debug.LogWarning("PlayerBladeSpecialAttacker: bladePrefab is not set.");
                IsActive = false;
                return;
            }

            Vector3 spawnPos = (spawnPoint != null) ? spawnPoint.position : transform.position + Vector3.up;
            Quaternion spawnRot = (spawnPoint != null) ? spawnPoint.rotation : transform.rotation;

            float scale = Mathf.Lerp(minScale, maxScale, _currentChargeRatio);
            float damage = Mathf.Lerp(baseDamage, maxDamage, _currentChargeRatio);

            GameObject bladeObj = Instantiate(bladePrefab, spawnPos, spawnRot);
            var blade = bladeObj.GetComponent<BladeProjectile>();
            if (blade != null)
            {
                blade.Initialize(damage, bladeSpeed, bladeLifetime, scale, _player.gameObject);
            }
            else
            {
                bladeObj.transform.localScale = Vector3.one * scale;
                var rb = bladeObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = bladeObj.transform.forward * bladeSpeed;
                }
                Destroy(bladeObj, bladeLifetime);
            }

            IsActive = false;
            return;
        }

        // 몬스터가 있으면 플레이어를 몬스터 근처로 순간이동
        Vector3 playerPos = _player.transform.position;
        Vector3 targetPos = nearest.position;

        Vector3 dirToMonster = (targetPos - playerPos);
        float dist = dirToMonster.magnitude;
        Vector3 approachDir = dist > 0.001f ? dirToMonster.normalized : (nearest.forward != Vector3.zero ? nearest.forward : Vector3.forward);

        // 차지 비율에 따라 몬스터로부터 떨어진 거리를 보간 (차지 높을수록 더 가까이 붙음)
        float desiredDistance = Mathf.Lerp(minTeleportDistanceFromMonster, maxTeleportDistanceFromMonster, _currentChargeRatio);
        Vector3 teleportPos = targetPos - approachDir * desiredDistance;
        teleportPos.y = targetPos.y + teleportHeightOffset; // 약간 높이 오프셋(지면 박힘 방지용)

        // 실제 순간이동: 콜라이더/네비/애니메이션에 따라 추가 처리가 필요할 수 있음.
        // (예: CharacterController나 NavMeshAgent를 사용한다면 해당 컴포넌트의 Move/warp를 사용해야 함)
        CharacterController cc = _player.GetComponent<CharacterController>();
        if (cc != null)
        {
            // CharacterController가 있다면 직접 위치 세팅 전에 enabled 처리가 필요할 수 있음
            cc.enabled = false;
            _player.transform.position = teleportPos;
            cc.enabled = true;
        }
        else
        {
            // NavMeshAgent가 있다면 agent.Warp(teleportPos)를 사용하는 것이 안전
            var agent = _player.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(teleportPos);
            }
            else
            {
                _player.transform.position = teleportPos;
            }
        }

        // 플레이어가 몬스터를 바라보게 회전
        Vector3 lookDir = (targetPos - _player.transform.position);
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            _player.transform.rotation = Quaternion.LookRotation(lookDir);

        // 필요하면 이 시점에 몬스터에 대한 근접 공격을 처리하거나 이벤트를 발생시킬 수 있음.
        // 예: 근접 히트 처리(데미지 전파)는 프로젝트 구조에 따라 다름.
        // float damage = Mathf.Lerp(baseDamage, maxDamage, _currentChargeRatio);
        // nearest.GetComponent<MonsterBase>()?.TakeDamage(damage, _player.gameObject);

        IsActive = false;
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