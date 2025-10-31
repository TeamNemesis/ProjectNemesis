using System.Collections;
using UnityEngine;

public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;

    [Header("Grapple Settings")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private LayerMask grappleMask; // 벽, 기둥 등
    [SerializeField] private LayerMask enemyMask;   // 적 레이어
    [SerializeField] private float stopDistance = 1.5f;

    private LineRenderer line;
    private Vector3 targetPoint;
    private Coroutine grappleRoutine;
    private bool isMoving = false;

    private void Awake()
    {
        // 런타임 자동 생성 (없어도 문제 안 생기게)
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Unlit/Color"));
            line.startColor = line.endColor = Color.white;
            line.startWidth = line.endWidth = 0.05f;
            line.useWorldSpace = true;
            line.enabled = false;
        }
    }

    public override void StartCharge()
    {
        base.StartCharge();

        // 누르는 순간 바로 발사하도록 변경
        Fire();
    }

    protected override void Fire()
    {
        if (isMoving) return;

        Vector3 origin = _player.transform.position + Vector3.up * 1f;
        Vector3 dir = _player.transform.forward;

        RaycastHit hit;

        // 1️ 적 판정
        if (Physics.Raycast(origin, dir, out hit, maxDistance, enemyMask))
        {
            Debug.Log("적에게 명중!");
            ShowLine(origin, hit.point);
            //EventBus.MonsterHit(WeaponType.Blade, ATTACKTYPE.SPECIALATTACK, hit.transform, _player.transform);
            hit.transform.GetComponentInParent<IDamageable>()?.TakeDamage(50, null);
            _player.StartCoroutine(FadeOutLine());
            EndSpecial();
            return;
        }

        // 2️ 벽/기둥 등 구조물 판정
        if (Physics.Raycast(origin, dir, out hit, maxDistance, grappleMask))
        {
            Debug.Log($"갈고리 명중: {hit.collider.name}");
            targetPoint = hit.point;
            ShowLine(origin, targetPoint);
            grappleRoutine = _player.StartCoroutine(GrappleMove());
        }
        else
        {
            Debug.Log("갈고리 명중 실패");
            ShowLine(origin, origin + dir * maxDistance);
            _player.StartCoroutine(FadeOutLine());
            EndSpecial();
        }
    }

    IEnumerator GrappleMove()
    {
        isMoving = true;

        while (true)
        {
            Vector3 pos = _player.transform.position;
            Vector3 toTarget = targetPoint - pos;
            float dist = toTarget.magnitude;

            if (dist < stopDistance)
                break;

            _player.transform.position += toTarget.normalized * moveSpeed * Time.deltaTime;

            // 줄 길이 갱신
            if (line != null)
            {
                line.SetPosition(0, _player.transform.position + Vector3.up * 1f);
                line.SetPosition(1, targetPoint);
            }

            yield return null;
        }

        Debug.Log("도착 완료");
        yield return _player.StartCoroutine(FadeOutLine());
        isMoving = false;
        EndSpecial();
    }

    private void ShowLine(Vector3 start, Vector3 end)
    {
        if (line == null) return;
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    private IEnumerator FadeOutLine()
    {
        if (line == null) yield break;

        float t = 0f;
        Color startColor = line.startColor;
        Color endColor = startColor;
        endColor.a = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            Color c = Color.Lerp(startColor, endColor, t);
            line.startColor = line.endColor = c;
            yield return null;
        }

        line.enabled = false;
    }
}
