using System;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
/// <summary>
/// 플레이어의 유탄공격을 담당하는 클래스
/// 무기타입에 상관없이 공통으로 사용
/// </summary>
public class PlayerGrenadeAttacker : MonoBehaviour, IAttacker
{
    public WeaponType WeaponType => throw new NotImplementedException();

    public bool IsAttacking => throw new NotImplementedException();

    public event Action OnAttackStarted;
    public event Action OnAttackEnded;

    //
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject explodeCircle;
    [SerializeField] private float travelTime = 1.0f;     // 유탄이 도착하는 시간
    [SerializeField] private float travelSpeed = 30.0f;
    [SerializeField] private float explosionRadius = 3f;  // 폭발 반경
    [SerializeField] private float explosionDamage = 30f; // 폭발 데미지
    [SerializeField] private LayerMask enemyLayer;        // 적 탐지용

    Vector3 _mousePos;

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

    private Camera mainCam;

    
    
    public void EndAttack()
    {
        throw new NotImplementedException();
    }

    public void GrenadeAttack(Vector3 mousePos)
    {
        Debug.Log("유탄 공격 실행");
        LaunchGrenade(mousePos);
    }

    public void OnAnimationEnd()
    {
        throw new NotImplementedException();
    }

    public void OnAnimationFire()
    {
        throw new NotImplementedException();
    }

    public bool RequestAttack()
    {
        GrenadeAttack(_mousePos);
        return true;
        throw new NotImplementedException();
    }

    public void SetMousePos(Vector3 mousePos)
    {
        _mousePos = mousePos;
        RequestAttack();
    }

    private void LaunchGrenade(Vector3 targetPos)
    {
        GameObject grenade = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Bullet/Grenade", transform.position + Vector3.up * 1.0f, Quaternion.identity);
        GameObject explodeRange = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Effect/ExplodeCircle", targetPos + Vector3.up * 0.1f, Quaternion.Euler(90f, 0f, 0f));
        StartCoroutine(ParabolaMove(grenade, targetPos));
        Destroy(explodeRange, travelTime);
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

        EventBus.GrenadeBomb(grenade.transform.position);
        Explode(grenade.transform.position,grenade.transform);
        Destroy(grenade);
    }

    private void Explode(Vector3 position,Transform grenadeTransform)
    {
        Collider[] hits = Physics.OverlapSphere(position, explosionRadius, enemyLayer);
        foreach (Collider hit in hits)
        {
            IDamageable enemy = hit.GetComponent<IDamageable>();
            if (enemy != null)
            {
                Transform monster = hit.transform;
                EventBus.MonsterHit(WeaponType.None, ATTACKTYPE.GRENADE, monster, grenadeTransform);
            }
        }

        Debug.Log("폭발 위치: " + position);
    }
}