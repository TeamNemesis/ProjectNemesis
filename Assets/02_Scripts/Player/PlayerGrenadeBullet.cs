using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어가 발사한 유탄을 관리하는 클래스
/// </summary>
public class PlayerGrenadeBullet : MonoBehaviour
{
    [SerializeField] private float travelTime = 1.0f;     // 유탄이 도착하는 시간
    [SerializeField] private float travelSpeed = 30.0f;
    [SerializeField] private float explosionRadius = 3f;  // 폭발 반경
    [SerializeField] private LayerMask enemyLayer;        // 적 탐지용

    [SerializeField] float _mutatntSpeedMultiplier = 1.5f;
    [SerializeField] float _mutantTravelTime = 3f;

    // --- 새로 추가된 파라미터 (튜닝용) ---
    [Header("Parabola Height Tuning")]
    [SerializeField, Tooltip("짧은 거리(가까움)일 때의 최대 포물선 높이")]
    private float maxParabolaHeight = 15f;
    [SerializeField, Tooltip("먼 거리(멀리)일 때의 최소 포물선 높이")]
    private float minParabolaHeight = 10f;
    [SerializeField, Tooltip("이 거리 이하이면 '가깝다'로 간주")]
    private float closeDistance = 2f;
    [SerializeField, Tooltip("이 거리 이상이면 '멀다'로 간주")]
    private float farDistance = 10f;

    public void Initialize(Transform firePoint, Vector3 target)
    {
        if(EventBus.HasMutant2)
        {
            StartCoroutine(DirectMoveRoutine(firePoint, target));
            return;
        }
        StartCoroutine(ParabolaMove(firePoint, target));
    }

    public IEnumerator ParabolaMove(Transform firePoint, Vector3 target)
    {
        Vector3 start = firePoint.position + Vector3.up * 1.5f;
        float elapsed = 0f;

        float distance = Vector3.Distance(new Vector3(start.x, 0f, start.z), new Vector3(target.x, 0f, target.z));

        float tDist = Mathf.InverseLerp(closeDistance, farDistance, distance);
        float smooth = Mathf.SmoothStep(0f, 1f, tDist);
        float parabolaHeight = Mathf.Lerp(maxParabolaHeight, minParabolaHeight, smooth);

        if (distance < 0.2f)
            parabolaHeight = Mathf.Min(parabolaHeight, maxParabolaHeight * 0.8f);

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);

            Vector3 flatPos = Vector3.Lerp(start, target, t);
            float parabola = 4f * parabolaHeight * (t - t * t);
            flatPos.y += parabola;

            transform.position = flatPos;
            yield return null;
        }

        EventBus.GrenadeBomb(transform.position);
    }

    IEnumerator DirectMoveRoutine(Transform firePoint, Vector3 target)
    {
        while(true)
        {
            Vector3 direction = (target - firePoint.position).normalized;
            transform.position += direction * travelSpeed * _mutatntSpeedMultiplier * Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(EventBus.HasMutant2)
        {
            if(other.CompareTag(Constants.TAG_GROUND) || other.CompareTag(Constants.TAG_WALL) || other.CompareTag(Constants.TAG_MONSTER))
            {
                Explode(transform.position, transform);
                Destroy(gameObject);
                return;
            }
        }
        if (other.CompareTag(Constants.TAG_GROUND) || other.CompareTag(Constants.TAG_WALL))
        {
            Explode(transform.position, transform);
            Destroy(gameObject);
        }
    }

    private void Explode(Vector3 position, Transform grenadeTransform)
    {
        // non-alloc array
        Collider[] hits = new Collider[10];
        int hitCount = Physics.OverlapSphereNonAlloc(position, explosionRadius, hits, enemyLayer);

        if (hitCount <= 0)
        {
            Debug.Log("폭발했지만 적을 맞추지 못함");
            return;
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = hits[i];
            if (hit == null)
                continue;

            IDamageable enemy = hit.GetComponent<IDamageable>();
            Debug.Log("IDamageable 컴포넌트 탐색 시도: " + (enemy != null ? "성공" : "실패"));
            if (enemy != null)
            {
                Transform monster = hit.transform;
                EventBus.MonsterHit(WeaponType.None, ATTACKTYPE.GRENADE, monster, grenadeTransform);
                Debug.Log(monster.name + "에게 폭발 데미지 적용");
            }
        }

        Debug.Log("폭발 위치: " + position);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
