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
    float _maxDistance;
    float _width;
    LayerMask _wallMask;
    LayerMask _enemyMask;
    GameObject _owner;

    public float lifeTime = 0.25f; // 비주얼 지속 시간
    public GameObject impactEffectPrefab; // 벽 충돌 이펙트(선택)

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        // 기본 세팅(필요하면 Inspector에서 덮어쓰기)
        _lr.positionCount = 2;
        _lr.useWorldSpace = true;
    }

    public void Initialize(Vector3 origin, Vector3 dir, float maxDist, float width,
                           LayerMask wallMask, LayerMask enemyMask, GameObject owner)
    {
        _origin = origin;
        _dir = dir.normalized;
        _maxDistance = maxDist;
        
        _width = width;
        _wallMask = wallMask;
        _enemyMask = enemyMask;
        _owner = owner;

        _lr.startWidth = _width;
        _lr.endWidth = _width;
    }

    public void Fire()
    {
        // 1) wallMask로 가장 가까운 wall 충돌 지점 찾기 (없으면 maxDistance)
        float endDist = _maxDistance;
        RaycastHit wallHit;
        if (Physics.Raycast(_origin, _dir, out wallHit, _maxDistance, _wallMask, QueryTriggerInteraction.Ignore))
        {
            endDist = wallHit.distance;
            // 임팩트 이펙트 생성 (선택)
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, wallHit.point, Quaternion.LookRotation(wallHit.normal));
            }
        }

        Vector3 endPos = _origin + _dir * endDist;

        // 2) LineRenderer로 시각화
        _lr.SetPosition(0, _origin);
        _lr.SetPosition(1, endPos);

        // 3) 적에게 단타 데미지 적용: enemyMask로 RaycastAll(범위: endDist)
        //    적들이 관통되길 원하므로 여기서는 All hits을 탐색
        RaycastHit[] hits = Physics.RaycastAll(_origin, _dir, endDist, _enemyMask, QueryTriggerInteraction.Collide);
        // Optional: 정렬 (거리에 따라) - 보통 필요없음
        // System.Array.Sort(hits, (a,b) => a.distance.CompareTo(b.distance));
        foreach (var h in hits)
        {
            if (h.collider == null) continue;
            // 자기 자신 무시
            if (h.collider.gameObject == _owner) continue;

            // IDamageable 인터페이스 사용 권장
            var dmg = h.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                EventBus.MonsterHit(WeaponType.Rifle, ATTACKTYPE.SPECIALATTACK, h.collider.gameObject.transform, _owner.transform);
            }
            else
            {
                // 디버그용: 다른 방법으로 처리할 수 있음
                var rb = h.collider.attachedRigidbody;
                if (rb != null) rb.AddForce(_dir * 50f, ForceMode.Impulse);
            }
        }

        // 4) 자동 제거(또는 풀에 반환)
        StartCoroutine(DestroyAfterSeconds(lifeTime));
    }

    IEnumerator DestroyAfterSeconds(float t)
    {
        yield return new WaitForSeconds(t);
        // 풀 사용 시에는 반환 로직으로 대체
        Destroy(gameObject);
    }
}