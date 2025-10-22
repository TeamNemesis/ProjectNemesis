using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 예시: 레이저형 스페셜(차지 → 단타 발사, Wall에서 끊김, 적 관통)
/// - 차지 타이머는 Player(또는 이 컴포넌트)가 owner.StartCoroutine으로 관리
/// - Initialize(owner) 반드시 호출
/// </summary>
public class PlayerRifleSpecialAttacker : PlayerSpecialAttacker
{
    [Header("Charge")]
    public float maxChargeTime = 3f;
    public float minDamage = 20f;
    public float maxDamage = 150f;
    public float minWidth = 0.05f;
    public float maxWidth = 0.6f;

    [Header("Laser")]
    public GameObject laserPrefab; // LineRenderer 포함된 프리팹
    public float maxDistance = 100f;
    public LayerMask wallMask;
    public LayerMask enemyMask;
    public float visualLifetime = 0.25f;

    Coroutine _chargeRoutine;
    float _chargeTimer;

    public override event Action<float> OnSpecialChargeUpdated;
    public override event Action OnSpecialFired;

    public override WeaponType WeaponType => WeaponType.Rifle; // 예시

    public override void Initialize(Player owner)
    {
        base.Initialize(owner);
    }

    public override void StartCharge()
    {
        base.StartCharge();
        // owner에서 코루틴을 시작하여 차지 업데이트
        if (_chargeRoutine != null) _owner.StopCoroutine(_chargeRoutine);
        _chargeRoutine = _owner.StartCoroutine(ChargeRoutine());
    }

    public override void StopChargeAndFire()
    {
        if (_chargeRoutine != null) { _owner.StopCoroutine(_chargeRoutine); _chargeRoutine = null; }
        float ratio = Mathf.Clamp01(_chargeTimer / maxChargeTime);
        _chargeTimer = 0f;
        // 실제 발사
        FireWithCharge(ratio);
        base.StopChargeAndFire(); // triggers events & EndSpecial
    }

    public override void CancelCharge()
    {
        if (_chargeRoutine != null) { _owner.StopCoroutine(_chargeRoutine); _chargeRoutine = null; }
        _chargeTimer = 0f;
        base.CancelCharge();
    }

    IEnumerator ChargeRoutine()
    {
        _chargeTimer = 0f;
        while (_chargeTimer < maxChargeTime)
        {
            _chargeTimer += Time.deltaTime;
            float ratio = Mathf.Clamp01(_chargeTimer / maxChargeTime);
            OnSpecialChargeUpdated?.Invoke(ratio);
            yield return null;
        }
        // 자동 풀차지 시 자동 발사(선택)
        _chargeRoutine = null;
        float fullRatio = 1f;
        FireWithCharge(fullRatio);
        OnSpecialFired?.Invoke();
        EndSpecial();
    }

    protected override void Fire()
    {
        // 파생 클래스 외부에서 직접 호출하지 않도록 비워둠 or 기본 동작 구현
        FireWithCharge(0f);
    }

    void FireWithCharge(float ratio)
    {
        float damage = Mathf.Lerp(minDamage, maxDamage, ratio);
        float width = Mathf.Lerp(minWidth, maxWidth, ratio);

        Vector3 origin = _owner.transform.position + Vector3.up * 1.0f; // 보정
        Vector3 dir = _owner.transform.forward;

        if (laserPrefab != null)
        {
            GameObject go = Instantiate(laserPrefab);
            var laser = go.GetComponent<LaserBeam>();
            laser.Initialize(origin, dir, maxDistance, damage, width, wallMask, enemyMask, _owner.gameObject);
            laser.lifeTime = visualLifetime;
            laser.Fire();
        }
        else
        {
            // 프리팹 없으면 직접 판정만
            float endDist = maxDistance;
            if (Physics.Raycast(origin, dir, out RaycastHit wh, maxDistance, wallMask, QueryTriggerInteraction.Ignore))
            {
                endDist = wh.distance;
            }
            RaycastHit[] hits = Physics.RaycastAll(origin, dir, endDist, enemyMask, QueryTriggerInteraction.Collide);
            foreach (var h in hits)
            {
                var dmg = h.collider.GetComponentInParent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(damage);
            }
        }
    }
}