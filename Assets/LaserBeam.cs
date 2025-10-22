using System.Collections;
using UnityEngine;

/// <summary>
/// LineRenderer 기반의 레이저 비주얼 + 타격 처리.
/// Initialize(...) 후 Fire()로 발동.
/// - 자동으로 Destroy(혹은 Pool으로 반환)
/// - 지속시간 동안 매 tick마다 RaycastAll 하여 데미지 적용(원하면 한 번만 적용)
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    LineRenderer _lr;
    Vector3 _origin;
    Vector3 _dir;
    float _maxDist;
    float _width;
    float _damage;
    float _duration;
    LayerMask _hitMask;
    GameObject _owner;
    float _tickInterval = 0.1f;
    float _elapsed = 0f;
    bool _isFiring = false;

    public void Initialize(Vector3 origin, Vector3 dir, float maxDist, float width, float damage, float duration, LayerMask mask, GameObject owner)
    {
        _lr = GetComponent<LineRenderer>();
        _origin = origin;
        _dir = dir.normalized;
        _maxDist = maxDist;
        _width = width;
        _damage = damage;
        _duration = duration;
        _hitMask = mask;
        _owner = owner;

        _lr.positionCount = 2;
        _lr.startWidth = _width;
        _lr.endWidth = _width;
        // set material properties (예: intensity) if shader supports it
    }

    public void Fire()
    {
        _isFiring = true;
        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        _elapsed = 0f;
        while (_elapsed < _duration)
        {
            float len = _maxDist;
            // 장애물에 의해 멈춰야 하면 Raycast to hit first obstacle:
            RaycastHit hit;
            if (Physics.Raycast(_origin, _dir, out hit, _maxDist, ~0, QueryTriggerInteraction.Ignore))
            {
                // 예: 벽 레이어를 무시하려면 mask 조정
                // hit가 enemy면 관통까지 진행(계속 RaycastAll). 여기선 시각만 줄이려면 len = hit.distance;
                len = hit.distance;
            }

            Vector3 end = _origin + _dir * len;
            _lr.SetPosition(0, _origin);
            _lr.SetPosition(1, end);

            // 피격처리: BoxCast 또는 RaycastAll 사용(두께 고려)
            ApplyDamageAlongBeam(_origin, _dir, len);

            _elapsed += _tickInterval;
            yield return new WaitForSeconds(_tickInterval);
        }

        // (옵션) 풀에 반환
        Destroy(gameObject);
    }

    void ApplyDamageAlongBeam(Vector3 origin, Vector3 dir, float length)
    {
        // 간단: RaycastAll
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, length, _hitMask, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.collider.gameObject == _owner) continue; // 자기자신 무시
            var dmg = h.collider.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(_damage * _tickInterval); // 지속시간 배분 데미지
            }
        }
    }
}