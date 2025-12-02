using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 검기 관련 로직을 제거하고 "가장 가까운 적에게 순간이동" 기능만 남긴 구현.
/// - 차지 시간에 따라 목표 근처로 더 가깝게 순간이동(0..1 -> 멀리..가까이)
/// - 가장 가까운 적이 없으면 아무 동작도 하지 않음 (검기 발사 없음, 이벤트 OnSpecialFired 미발생)
/// </summary>
public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    [Header("Charge Settings")]
    [SerializeField] float maxChargeTime = 2.0f;        // 완전 차지까지 시간(초)

    // Teleport-specific tuning: 차지 비율 0 -> minTeleportDistance, 1 -> maxTeleportDistance (작을수록 적에 더 근접)
    [Header("Teleport Tuning")]
    [SerializeField] float minTeleportDistanceFromMonster = 1.5f; // 차지 0일 때 몬스터로부터의 거리
    [SerializeField] float maxTeleportDistanceFromMonster = 0.4f; // 완전 차지일 때 몬스터로부터의 거리
    [SerializeField] float teleportHeightOffset = 0.2f; // 순간이동 시 높이 보정

    Coroutine _chargeCoroutine;
    float _currentChargeRatio = 0f; // 0..1

    // 이 사용 중에 실제로 순간이동이 일어났는지 추적 (이벤트 발생 결정용)
    bool _didTeleportThisUse = false;

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
        _didTeleportThisUse = false;
        OnSpecialStarted?.Invoke();

        // 시작하면 기존 코루틴 정리 후 새로 시작
        if (_chargeCoroutine != null) StopCoroutine(_chargeCoroutine);
        _chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    public override void StopChargeAndFire()
    {
        // 버튼 해제 시 수동 발사
        if (!IsCharging) return;

        // 정지하고 현재 ratio로 시도
        IsCharging = false;
        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;
        }

        // Fire는 몬스터가 없으면 아무 동작도 하지 않음
        Fire();

        //// 실제로 순간이동이 발생했을 때만 OnSpecialFired를 호출
        //if (_didTeleportThisUse)
        //{
        //    OnSpecialFired?.Invoke();
        //}
        OnSpecialFired?.Invoke();

        // 항상 상태를 정리하여 일관된 상태를 유지
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
        // _didTeleportThisUse는 StartCharge에서 초기화됨
        if (_player == null)
        {
            Debug.LogWarning("PlayerBladeSpecialAttacker: player is null. Call Initialize first.");
            return;
        }

        // 가장 가까운 몬스터 찾기 (EventBus에 구현되어 있는 헬퍼 사용)
        Transform nearest = EventBus.GetNearestMonsterFromMe(_player.transform);

        if (nearest == null)
        {
            // 몬스터가 없으면 아무 동작도 하지 않음 (검기 관련 로직은 제거됨)
            // 로그를 남기고 조용히 반환
            // Debug.Log("PlayerBladeSpecialAttacker: No monster found. Doing nothing.");
            _didTeleportThisUse = false;
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

        // 순간이동이 실제로 발생했음을 표시
        _didTeleportThisUse = true;
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

            // 자동 발사: 최대 차지에 도달하면 자동으로 시도
            if (elapsed >= maxChargeTime)
            {
                IsCharging = false;
                _chargeCoroutine = null;

                Fire();

                if (_didTeleportThisUse)
                {
                    OnSpecialFired?.Invoke();
                }

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
        _didTeleportThisUse = false;
        RaiseChargeUpdated(_currentChargeRatio);
        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;
        }

        OnSpecialEnded?.Invoke();
    }
}