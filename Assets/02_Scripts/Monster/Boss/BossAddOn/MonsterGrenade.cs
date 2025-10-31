using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UIElements;


public class MonsterGrenade : PoolableObject
{

    public bool IsAttacking;

    public event Action AttackStarted;
    public event Action AttackEnded;


    [SerializeField] private float travelTime = 3.0f;     // 유탄이 도착하는 시간
    [SerializeField] private float explosionRadius = 3f;  // 폭발 반경
    [SerializeField] private float explosionDamage = 30f; // 폭발 데미지
    [SerializeField] private bool isEcploed = false;

    [SerializeField] private PoolableObject grenadeObject;
    [SerializeField] private PoolableObject explodeCircleObject;

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

    // --- 작은 유탄 관련 파라미터 ---
    [Header("Small Grenade Settings")]
    [SerializeField, Range(0.1f, 1f)] private float smallGrenadeScale = 0.5f;  // 작은 유탄 크기 비율
    [SerializeField] private float smallGrenadeTravelTime = 1.5f;
    [SerializeField] private float smallGrenadeDistance = 3f;  // 작은 유탄이 날아갈 거리
    [SerializeField] private float smallGrenadeHeight = 5f;    // 작은 유탄 포물선 높이
    [SerializeField] private float smallGrenadeExplosionRadius = 2f;
    [SerializeField] private float smallGrenadeExplosionDamage = 15f;


    private void Start()
    {
        FireGrenade(transform.position + transform.forward * 5f);
    }
    public void FireGrenade(Vector3 targetPosition)
    {
        StartCoroutine(LaunchGrenade(targetPosition));
    }

    private IEnumerator LaunchGrenade(Vector3 targetPos)
    {
        GameObject grenade = GameManager.Instance.PoolManager.GetFromPool(grenadeObject, transform.position + Vector3.up * 1.0f, Quaternion.identity);

        grenade.transform.localScale = Vector3.one;

        GameObject explodeRange = GameManager.Instance.PoolManager.GetFromPool(explodeCircleObject, targetPos + Vector3.up * 0.1f, Quaternion.Euler(90f, 0f, 0f));
        AttackDecalEffect effect = explodeRange.GetComponent<AttackDecalEffect>();
        if (effect != null)
        {
            effect.Play(travelTime, explosionRadius);
        }
        StartCoroutine(ParabolaMove(grenade, targetPos));
        yield return new WaitForSeconds(travelTime);
    }

    private IEnumerator ParabolaMove(GameObject grenade, Vector3 target)
    {
        Vector3 start = grenade.transform.position;
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

            grenade.transform.position = flatPos;
            yield return null;
        }

        Vector3 explosionPos = grenade.transform.position;
        Explode(explosionPos);

        // 작은 유탄 5개 발사
        SpawnSmallGrenades(explosionPos);

        GameManager.Instance.PoolManager.ReleaseToPool(grenade);
    }

    private void SpawnSmallGrenades(Vector3 centerPos)
    {
        int grenadeCount = 5;
        float angleStep = 360f / grenadeCount; // 72도

        for (int i = 0; i < grenadeCount; i++)
        {
            float angle = angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            // 목표 위치 계산 (Y축 회전)
            Vector3 direction = new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            Vector3 targetPos = centerPos + direction * smallGrenadeDistance;

            // 작은 유탄 생성 및 발사
            StartCoroutine(LaunchSmallGrenade(centerPos, targetPos));
        }
    }

    private IEnumerator LaunchSmallGrenade(Vector3 startPos, Vector3 targetPos)
    {
        GameObject smallGrenade = GameManager.Instance.PoolManager.GetFromPool(
            grenadeObject,
            startPos,
            Quaternion.identity
        );

        if (smallGrenade == null) yield break;

        smallGrenade.transform.localScale = Vector3.one * smallGrenadeScale;

        GameObject smallExplodeRange = GameManager.Instance.PoolManager.GetFromPool(
            explodeCircleObject,
            targetPos + Vector3.up * 0.1f,
            Quaternion.Euler(90f, 0f, 0f)
        );

        if (smallExplodeRange != null)
        {
            AttackDecalEffect effect = smallExplodeRange.GetComponent<AttackDecalEffect>();
            if (effect != null)
            {
                effect.Play(smallGrenadeTravelTime, smallGrenadeExplosionRadius);
            }
        }

        float elapsed = 0f;

        while (elapsed < smallGrenadeTravelTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / smallGrenadeTravelTime);

            Vector3 flatPos = Vector3.Lerp(startPos, targetPos, t);
            float parabola = 4f * smallGrenadeHeight * (t - t * t);
            flatPos.y += parabola;

            smallGrenade.transform.position = flatPos;
            yield return null;
        }

        SmallGrenadeExplode(smallGrenade.transform.position);

        smallGrenade.transform.localScale = Vector3.one;
        GameManager.Instance.PoolManager.ReleaseToPool(smallGrenade);
    }

    private void Explode(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(Constants.TAG_PLAYER))
            {
                IDamageable player = hit.GetComponent<IDamageable>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage);
                }
            }
        }
    }

    private void SmallGrenadeExplode(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, smallGrenadeExplosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(Constants.TAG_PLAYER))
            {
                IDamageable player = hit.GetComponent<IDamageable>();
                if (player != null)
                {
                    player.TakeDamage(smallGrenadeExplosionDamage);
                }
            }
        }
    }
}