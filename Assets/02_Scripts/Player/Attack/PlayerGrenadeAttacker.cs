using System;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
/// <summary>
/// 플레이어의 유탄공격을 담당하는 클래스
/// 무기타입에 상관없이 공통으로 사용
/// </summary>
public class PlayerGrenadeAttacker : MonoBehaviour
{
    //
    [SerializeField] string _grenadePath = "Prefabs/Bullet/Grenade";
    [SerializeField] string _explodeCirclePath = "Prefabs/Effect/ExplodeCircle";

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

    public void GrenadeAttack(Vector3 mousePos)
    {
        Debug.Log("유탄 공격 실행");
        LaunchGrenade(mousePos);
    }

    public bool RequestAttack()
    {
        GrenadeAttack(_mousePos);
        return true;
    }

    public void SetMousePos(Vector3 mousePos)
    {
        _mousePos = mousePos;
    }

    /// <summary>
    /// 유탄을 발사하는 함수
    /// </summary>
    /// <param name="targetPos"></param>
    private void LaunchGrenade(Vector3 targetPos)
    {
        GameObject grenade = GameManager.Instance.PoolManager.GetFromPool(_grenadePath, transform.position + Vector3.up * 1.0f, Quaternion.identity);
        GameObject explodeCircle = GameManager.Instance.PoolManager.GetFromPool(_explodeCirclePath, targetPos + Vector3.up * 0.1f, Quaternion.Euler(90f, 0f, 0f));

        PlayerGrenadeBullet bullet = grenade.GetComponent<PlayerGrenadeBullet>();
        if(bullet == null)
        {
            Debug.Log("PlayerGrenadeBullet 컴포넌트가 없습니다.");
            return;
        }
        bullet.Initialize(transform, targetPos);

        Destroy(explodeCircle, travelTime);
    }
}