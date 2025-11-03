using System.Collections;
using UnityEngine;

public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;

    [Header("Grapple Settings")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private LayerMask grappleMask; // 벽, 기둥 등

    [SerializeField] private LineRenderer line; // 프리팹

    private Vector3 targetPoint;
    private Coroutine grappleRoutine;
    private bool isMoving = false;
    private bool hasHit = false;

    public override void StartCharge()
    {
        base.StartCharge();
    }

    protected override void Fire()
    {
        if (isMoving || hasHit) return; // 중복 방지

        Vector3 origin = _player.transform.position + Vector3.up * 1f;
        Vector3 dir = _player.transform.forward;
        RaycastHit hit;

        hasHit = true; // 한 번만 공격 허용


        // 명중
        if (Physics.Raycast(origin, dir, out hit, maxDistance, grappleMask))
        {
            Debug.Log($"갈고리 명중: {hit.collider.name}");
            targetPoint = hit.point;
            ShowLine(origin, targetPoint);
            var dmgTarget = hit.transform.GetComponent<IDamageable>();
            if (dmgTarget != null)
                dmgTarget.TakeDamage(50, null);     //데미지 적용

            grappleRoutine = _player.StartCoroutine(GrappleMove());
        }
        else
        {
            Debug.Log("갈고리 명중 실패");
            ShowLine(origin, origin + dir * maxDistance);
            _player.StartCoroutine(InstantFadeOut());
            EndSpecial();
        }
    }

    IEnumerator GrappleMove()
    {
        isMoving = true;
        _player.SetCanMove(false);
        _player.SetIsSpecialAttacking(true);

        Vector3 start = _player.transform.position;

        while (true)
        {
            Vector3 pos = _player.transform.position;
            Vector3 toTarget = targetPoint - pos;
            float dist = toTarget.magnitude;

            if (dist < stopDistance)
                break;

            // 이동 (지면 높이 유지)
            Vector3 next = pos + toTarget.normalized * moveSpeed * Time.deltaTime;
            next.y = GetGroundY(next); // 떠오름 방지

            _player.transform.position = next;

            // 라인 갱신
            if (line != null)
            {
                line.SetPosition(0, _player.transform.position + Vector3.up * 1f);
                line.SetPosition(1, targetPoint);
            }

            yield return null;
        }

        Debug.Log(" 도착 완료");

        _player.StartCoroutine(InstantFadeOut());
        isMoving = false;

        _player.SetCanMove(true);
        _player.SetIsSpecialAttacking(false);

        EndSpecial();
    }

    private float GetGroundY(Vector3 position)
    {
        //  도착지점에서 지면 높이 탐지 (떠오름 방지)
        if (Physics.Raycast(position + Vector3.up * 2f, Vector3.down, out RaycastHit groundHit, 10f, LayerMask.GetMask("Ground")))
        {
            return groundHit.point.y;
        }
        return position.y; // 없으면 그대로
    }

    private void ShowLine(Vector3 start, Vector3 end)
    {
        if (line == null) return;
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startColor = line.endColor = Color.red;
    }

    private IEnumerator InstantFadeOut()
    {
        yield return new WaitForSeconds(0.1f);
        if (line != null)
            line.enabled = false;
        hasHit = false; // 다음 공격 가능하게 초기화
    }
}
