using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 대시 동작을 관리하는 클래스
/// </summary>
public class PlayerDasher : MonoBehaviour
{
    CharacterController _cc;

    public void Initialize(CharacterController cc)
    {
        _cc = cc;
    }

    /// <summary>
    /// 플레이어가 바라보고 있는 방향으로 짧게 대시하는 함수
    /// </summary>
    public void Dash(Vector3 direction, float distance, float duration)
    {
        direction = direction.normalized;
        StartCoroutine(DashRoutine(direction, distance, duration));
    }

    IEnumerator DashRoutine(Vector3 direction, float distance, float duration)
    {
        float elapsed = 0f;
        float speed = distance / duration;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            float step = speed * dt;
            // 충돌 사전 검사 (원하면)
            float allowed = GetAllowedDistance(direction, step);
            if (allowed <= 0f) yield break;

            var flags = _cc.Move(direction * allowed);
            if ((flags & CollisionFlags.Sides) != 0) yield break;

            elapsed += dt;
            yield return null;
        }
    }

    float GetAllowedDistance(Vector3 dir, float desiredDistance)
    {
        // (위에 있는 CapsuleCast 예제 재사용)
        Vector3 center = _cc.transform.position + _cc.center;
        float halfHeight = Mathf.Max(0, _cc.height * 0.5f - _cc.radius);
        Vector3 top = center + Vector3.up * halfHeight;
        Vector3 bottom = center - Vector3.up * halfHeight;
        float castDistance = desiredDistance + _cc.skinWidth;

        if (Physics.CapsuleCast(bottom, top, _cc.radius, dir, out RaycastHit hit, castDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            float allowed = Mathf.Max(0f, hit.distance - _cc.skinWidth);
            return allowed;
        }
        return desiredDistance;
    }
}