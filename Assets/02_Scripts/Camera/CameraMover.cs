using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 이동 + 카메라↔타겟 사이의 오클루더(occluder)를 찾아서 MaterialPropertyBlock으로 알파를 조절해 투명화/복원하는 컴포넌트.
/// 개선 내용:
/// - RaycastAll 기반을 OverlapBoxNonAlloc 기반(박스 검사)으로 대체(옵션). 더 넓은 영역(사각형/박스) 검사 지원.
/// - X/Z 클램프 버그 수정(_minX/_maxX 사용).
/// - MaterialPropertyBlock 재사용, 원래 색상 캐시 개선(렌더러 단위 Color 저장).
/// - destroyed/null renderer 안전 처리 및 비활성화 시 복원 보장.
/// - NonAlloc API 사용으로 GC 부담 감소.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] Vector3 _offset;
    [SerializeField] float _cameraSpeed = 10f;

    [Header("카메라 제한 영역")]
    [SerializeField] float _minX = -50f;
    [SerializeField] float _maxX = 50f;
    [SerializeField] float _minZ = -50f;
    [SerializeField] float _maxZ = 50f;

    [Header("투명화 설정")]
    [SerializeField, Range(0f, 1f)] float _transparentAlpha = 0.3f;
    [SerializeField] LayerMask _obstacleMask; // 투명화 대상 레이어 설정

    [Header("박스 검사 옵션")]
    [SerializeField] bool _useBoxOverlap = true;     // false면 RaycastAll 모드(이전 방식)
    [SerializeField] float _paddingFactor = 1.05f;   // 프러스텀 중간 슬라이스에 줄 패딩

    [Header("----- 읽기 전용 -----")]
    [SerializeField] Transform _target;

    // 내부 상태
    Dictionary<Renderer, Color> _originalColorMap = new Dictionary<Renderer, Color>();
    HashSet<Renderer> _currentOccluders = new HashSet<Renderer>();
    MaterialPropertyBlock _mpb;

    // NonAlloc 배열
    Collider[] _overlapResults = new Collider[256];
    RaycastHit[] _rayHits = new RaycastHit[256];

    public void Initialize(Player player)
    {
        _target = player?.transform;
        _mpb = new MaterialPropertyBlock();
        if (_target == null)
            Debug.LogError("CameraMover.Initialize: target이 null입니다!");
    }

    void Update()
    {
        if (_target == null) return;

        // 카메라 목표 위치 계산 및 이동 (제한 포함)
        Vector3 desiredPos = _target.position + _offset;
        float clampX = Mathf.Clamp(desiredPos.x, _minX, _maxX); // _minX/_maxX 로 수정
        float clampZ = Mathf.Clamp(desiredPos.z, _minZ, _maxZ);
        Vector3 targetPos = new Vector3(clampX, desiredPos.y, clampZ);
        transform.position = Vector3.Lerp(transform.position, targetPos, _cameraSpeed * Time.deltaTime);

        // 이번 프레임에 투명화할 렌더러들 계산
        _currentOccluders.Clear();

        Vector3 origin = transform.position;
        Vector3 dir = _target.position - origin;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return;

        if (_useBoxOverlap)
        {
            // 박스 중심: 카메라와 타깃의 중간
            Vector3 boxCenter = origin + dir * 0.5f;

            // 중간 거리에서 카메라 프러스텀의 절반 너비/높이 계산
            float midDist = dist * 0.5f;
            Camera cam = GetComponent<Camera>();
            float halfHeight = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * midDist;
            float halfWidth = halfHeight * cam.aspect;

            // 패딩 및 halfExtents 설정 (z는 거리 절반으로 카메라->타깃 전체를 덮음)
            Vector3 halfExtents = new Vector3(halfWidth * _paddingFactor, halfHeight * _paddingFactor, dist * 0.5f);

            // 박스의 방향: 카메라 -> 타깃
            Quaternion orientation = Quaternion.LookRotation(dir.normalized, cam.transform.up);

            // OverlapBoxNonAlloc 호출
            int hitCount = Physics.OverlapBoxNonAlloc(boxCenter, halfExtents, _overlapResults, orientation, _obstacleMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                var col = _overlapResults[i];
                if (col == null) continue;
                if (col.isTrigger) continue;

                Renderer rend = GetRelevantRenderer(col);
                if (rend == null) continue;
                if (IsTargetRenderer(rend)) continue;

                // 투명화
                EnsureOriginalColor(rend);
                SetRendererAlpha(rend, _transparentAlpha);
                _currentOccluders.Add(rend);
            }
        }
        else
        {
            // 기존 RaycastAll 방식(NonAlloc으로 대체)
            int hitCount = Physics.RaycastNonAlloc(new Ray(origin, dir.normalized), _rayHits, dist, _obstacleMask, QueryTriggerInteraction.Ignore);
            if (hitCount > 0)
            {
                System.Array.Sort(_rayHits, 0, hitCount, new RayHitDistanceComparer());

                for (int i = 0; i < hitCount; i++)
                {
                    var hit = _rayHits[i];
                    var col = hit.collider;
                    if (col == null) continue;
                    if (col.isTrigger) continue;

                    Renderer rend = GetRelevantRenderer(col);
                    if (rend == null) continue;
                    if (IsTargetRenderer(rend)) continue;

                    EnsureOriginalColor(rend);
                    SetRendererAlpha(rend, _transparentAlpha);
                    _currentOccluders.Add(rend);
                }
            }
        }

        // 이전에 투명화했지만 이번 프레임에 포함되지 않은 것들은 복원
        var toRestore = new List<Renderer>();
        foreach (var kv in _originalColorMap)
        {
            var rend = kv.Key;
            // 안전성: renderer가 파괴되었는지 확인
            if (rend == null)
            {
                toRestore.Add(rend); // null은 RestoreRenderer에서 내부적으로 무시됨
                continue;
            }
            if (!_currentOccluders.Contains(rend))
                toRestore.Add(rend);
        }
        foreach (var rend in toRestore)
        {
            RestoreRenderer(rend);
        }
    }

    // Renderer 얻기: Collider에서 Parent/Children으로 안전 탐색
    Renderer GetRelevantRenderer(Collider col)
    {
        if (col == null) return null;
        Renderer rend = col.GetComponent<Renderer>();
        if (rend != null) return rend;
        rend = col.GetComponentInParent<Renderer>();
        if (rend != null) return rend;
        rend = col.GetComponentInChildren<Renderer>();
        return rend;
    }

    // 타깃(플레이어) 내부 렌더러인지 확인
    bool IsTargetRenderer(Renderer r)
    {
        if (r == null || _target == null) return false;
        return r.transform.IsChildOf(_target);
    }

    // 원래 색 저장 (한 렌더러 당 하나의 Color로 단순화: material[0]의 컬러를 기준으로 저장)
    void EnsureOriginalColor(Renderer rend)
    {
        if (rend == null) return;
        if (_originalColorMap.ContainsKey(rend)) return;

        // sharedMaterial로 읽어 원본 색을 확인 (인스턴스 생성을 막기 위해 sharedMaterial 사용)
        var shared = rend.sharedMaterial;
        if (shared == null)
        {
            _originalColorMap[rend] = Color.white;
            return;
        }

        string colorProp = GetColorPropertyName(shared);
        if (colorProp == null)
        {
            _originalColorMap[rend] = Color.white;
            return;
        }

        Color orig = shared.GetColor(colorProp);
        _originalColorMap[rend] = orig;
    }

    // 사전에 검사해서 셰이더가 지원하는 컬러 프로퍼티 이름을 리턴
    string GetColorPropertyName(Material mat)
    {
        if (mat == null) return null;
        if (mat.HasProperty("_BaseColor")) return "_BaseColor";
        if (mat.HasProperty("_Color")) return "_Color";
        return null;
    }

    // MaterialPropertyBlock으로 알파 조절 (렌더러 단위로 적용)
    void SetRendererAlpha(Renderer rend, float alpha)
    {
        if (rend == null) return;
        if (!_originalColorMap.TryGetValue(rend, out Color orig)) EnsureOriginalColor(rend);

        var shared = rend.sharedMaterial;
        if (shared == null) return;

        string colorProp = GetColorPropertyName(shared);
        if (colorProp == null) return;

        // 기존 property block 가져오기
        rend.GetPropertyBlock(_mpb);
        Color newColor = orig;
        newColor.a = alpha;
        _mpb.SetColor(colorProp, newColor);
        rend.SetPropertyBlock(_mpb);
    }

    void RestoreRenderer(Renderer rend)
    {
        if (rend == null)
        {
            // null keys cleaning
            var keysToRemove = new List<Renderer>();
            foreach (var k in _originalColorMap.Keys)
                if (k == null) keysToRemove.Add(k);
            foreach (var k in keysToRemove)
                _originalColorMap.Remove(k);
            return;
        }

        if (!_originalColorMap.TryGetValue(rend, out Color orig)) return;

        var shared = rend.sharedMaterial;
        if (shared == null)
        {
            _originalColorMap.Remove(rend);
            return;
        }

        string colorProp = GetColorPropertyName(shared);
        if (colorProp != null)
        {
            rend.GetPropertyBlock(_mpb);
            _mpb.SetColor(colorProp, orig);
            rend.SetPropertyBlock(_mpb);
        }

        _originalColorMap.Remove(rend);
    }

    private void OnDisable()
    {
        // 비활성화 시 모든 복원
        var keys = new List<Renderer>(_originalColorMap.Keys);
        foreach (var rend in keys)
            RestoreRenderer(rend);
    }

    // 디버그: 박스 그리기
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || _target == null) return;

        Vector3 origin = transform.position;
        Vector3 dir = _target.position - origin;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return;

        Vector3 boxCenter = origin + dir * 0.5f;
        float midDist = dist * 0.5f;
        Camera cam = GetComponent<Camera>();
        float halfHeight = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * midDist * _paddingFactor;
        float halfWidth = halfHeight * cam.aspect * _paddingFactor;
        Vector3 halfExtents = new Vector3(halfWidth, halfHeight, dist * 0.5f);
        Quaternion orientation = Quaternion.LookRotation(dir.normalized, cam.transform.up);

        Gizmos.color = new Color(1f, 0f, 0f, 0.12f);
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, orientation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, halfExtents * 2f);
        Gizmos.matrix = old;
    }

    // RaycastHit 정렬기
    class RayHitDistanceComparer : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit a, RaycastHit b)
        {
            return a.distance.CompareTo(b.distance);
        }
    }
}