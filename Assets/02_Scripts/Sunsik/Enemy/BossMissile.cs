using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [Header("----- Missile Settings -----")]
    [SerializeField] float _moveSpeed; // 미사일 이동 속도
    [SerializeField] float _rotSpeed; // 미사일 회전 속도
    [SerializeField] float _lifeTime; // 미사일 생존 시간
    [SerializeField] float _trackingTime; // 미사일의 최대 추적 시간

    [Header("ReadOnly")]
    [SerializeField] Transform _target; // 미사일 추적 대상
    [SerializeField] Vector3 _startPos; // 미사일 시작 위치
    [SerializeField] float _currentTrackingTimer; // 현재 추적 시간
    [SerializeField] Vector3 _moveDir; // 미사일 이동 방향

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="target"></param>
    public void Initialize(Transform target)
    {
        _target = target;
        _startPos = transform.position; // 시작 위치 설정
        _currentTrackingTimer = 0f;

        Destroy(gameObject, _lifeTime); // 일정 시간 후 미사일 파괴
    }

    private void FixedUpdate()
    {
        if (_target == null)
        {
            Debug.Log("목표를 포착하지 못했다...");
            return;
        }
        
        _currentTrackingTimer += Time.fixedDeltaTime;
        MoveToTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero"))
        {
            Debug.Log("미사일이 히어로와 충돌했다!");
            Destroy(gameObject); // 히어로와 충돌 시 미사일 파괴
        }
    }

    public void MoveToTarget()
    {
        Vector3 endPos = _target.position;
        float arcHeight = 10f;

        // 0 ~ 1 보간값
        float t = Mathf.Clamp01(_currentTrackingTimer / _trackingTime);

        // 기본 선형 보간
        Vector3 currentPos = Vector3.Lerp(_startPos, endPos, t);

        // 포물선 보정
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * t);

        transform.position = currentPos;

        // 타겟을 바라보도록 회전
        Vector3 lookDir = (endPos - transform.position).normalized;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotSpeed * Time.fixedDeltaTime);
        }
    }
}