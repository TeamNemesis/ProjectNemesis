using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] Vector3 _offset;
    [SerializeField] float _cameraSpeed;

    [Header("카메라 제한 영역")]
    [SerializeField] float _minX;
    [SerializeField] float _maxX;
    [SerializeField] float _minZ;
    [SerializeField] float _maxZ;

    [Header("투명화 설정")]
    [SerializeField] float _transparentAlpha = 0.3f;
    [SerializeField] LayerMask _obstacleMask; // 투명화 대상 레이어 설정

    [Header("----- 읽기 전용 -----")]
    [SerializeField] Transform _target;

    // 상태 저장
    Dictionary<MeshRenderer, Color> _originalColorMap = new Dictionary<MeshRenderer, Color>();
    HashSet<MeshRenderer> _currentOccluders = new HashSet<MeshRenderer>();
    MaterialPropertyBlock _mpb;

    public void Initialize(Player player)
    {
        _target = player.transform;
        _mpb = new MaterialPropertyBlock();
        if (_target == null)
            Debug.LogError("CameraMover.Initialize: target이 null입니다!");
    }

    void Update()
    {
        if (_target == null) return;

        // 카메라 이동 (제한 포함)
        Vector3 desiredPos = _target.position + _offset;
        float clampX = Mathf.Clamp(desiredPos.x, _minZ, _maxZ);
        float clampZ = Mathf.Clamp(desiredPos.z, _minZ, _maxZ);
        transform.position = Vector3.Lerp(transform.position, new Vector3(clampX, desiredPos.y, clampZ), _cameraSpeed * Time.deltaTime);

        // 이번 프레임에 투명화할 렌더러들 계산
        _currentOccluders.Clear();

        Vector3 dir = _target.position - transform.position;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return;

        Ray ray = new Ray(transform.position, dir.normalized);
        // 모든 히트 검사 (레이가 관통하는 모든 오브젝트)
        RaycastHit[] hits = Physics.RaycastAll(ray, dist, _obstacleMask.value); // 레이어 마스크 적용

        if (hits != null && hits.Length > 0)
        {
            // 거리가 가까운 순으로 정렬 (선택적)
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.isTrigger) continue;

                if (hit.collider.TryGetComponent<MeshRenderer>(out MeshRenderer rend))
                {
                    // 렌더러가 유효하고 투명화 가능하면 처리
                    SetRendererAlpha(rend, _transparentAlpha);
                    _currentOccluders.Add(rend);
                }
            }
        }

        // 이전에 투명화했지만 이번 프레임에 포함되지 않은 것들은 복원
        // originalColors 키들 중 currentOccluders에 없는 것들을 복원
        var toRestore = new List<MeshRenderer>();
        foreach (var kv in _originalColorMap)
        {
            if (!_currentOccluders.Contains(kv.Key))
                toRestore.Add(kv.Key);
        }
        foreach (var rend in toRestore)
        {
            RestoreRenderer(rend);
        }
    }

    void SetRendererAlpha(MeshRenderer rend, float alpha)
    {
        if (rend == null) return;

        // property 이름 검사
        var shared = rend.sharedMaterial;
        if (shared == null) return;

        string colorProp = null;
        if (shared.HasProperty("_BaseColor")) colorProp = "_BaseColor";
        else if (shared.HasProperty("_Color")) colorProp = "_Color";

        if (colorProp == null)
        {
            // 해당 쉐이더가 컬러 프로퍼티를 제공하지 않음 -> 로그 한 번만 남김
            // Debug.LogWarning($"Shader '{shared.shader.name}'에 _BaseColor/_Color가 없음.");
            return;
        }

        // 원래 색 저장 (처음 만날 때만)
        if (!_originalColorMap.ContainsKey(rend))
        {
            Color orig = shared.GetColor(colorProp);
            _originalColorMap[rend] = orig;
        }

        // 기존 PropertyBlock 가져오고 색 변경
        rend.GetPropertyBlock(_mpb);
        Color cur = _originalColorMap[rend];
        cur.a = alpha;
        _mpb.SetColor(colorProp, cur);
        rend.SetPropertyBlock(_mpb);

        // 주의: 쉐이더가 Transparent/Fade 모드를 지원해야 알파가 실제로 보입니다.
    }

    void RestoreRenderer(MeshRenderer rend)
    {
        if (rend == null) return;
        if (!_originalColorMap.TryGetValue(rend, out Color orig)) return;

        // 복원
        rend.GetPropertyBlock(_mpb);
        var shared = rend.sharedMaterial;
        if (shared == null) return;

        string colorProp = null;
        if (shared.HasProperty("_BaseColor")) colorProp = "_BaseColor";
        else if (shared.HasProperty("_Color")) colorProp = "_Color";

        if (colorProp != null)
            _mpb.SetColor(colorProp, orig);

        rend.SetPropertyBlock(_mpb);

        _originalColorMap.Remove(rend);
    }

    private void OnDisable()
    {
        // 비활성화 시 모든 복원
        var keys = new List<MeshRenderer>(_originalColorMap.Keys);
        foreach (var rend in keys) RestoreRenderer(rend);
    }
}