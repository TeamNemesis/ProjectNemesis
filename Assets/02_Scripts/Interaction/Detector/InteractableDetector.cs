using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 지점에서 RayCast로 IInteractable 객체를 감지하는 클래스
/// FixedUpdate 주기로 자동 감지한다.
/// </summary>
public class InteractableDetector : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Transform _detectPoint;            // RayCast 시작점

    [Header("----- 설정 데이터 -----")]
    [SerializeField] float _distance;                   // RayCast 거리
    [SerializeField] LayerMask _targetLayerMask;        // 감지할 레이어마스크

    IInteractable _detectedInteractable;                // 현재 감지한 Interactable 객체

    /// <summary>
    /// 감지 이벤트
    /// </summary>
    public event Action<IInteractable> OnDetected;

    /// <summary>
    /// 감지 실패 이벤트
    /// </summary>
    public event Action OnMissed;

    private void FixedUpdate()
    {
        Detect();
    }

    /// <summary>
    /// 감지하는 함수
    /// </summary>
    public void Detect()
    {
        // Raycast에 감지된 콜라이더가 있으면 
        if (Physics.Raycast(
            _detectPoint.position,
            _detectPoint.forward,
            out RaycastHit hit,
            _distance,
            _targetLayerMask) == true)
        {
            // 감지된 콜라이더 게임오브젝트의 부모 게임오브젝트에서
            // IInteractable 컴포넌트를 찾아온다.
            _detectedInteractable = hit.collider.GetComponentInParent<IInteractable>();

            // IInteractable이 있었으면
            if (_detectedInteractable != null)
            {
                // 감지 이벤트 발행
                OnDetected?.Invoke(_detectedInteractable);
                return;
            }
        }

        // 감지된 콜라이더에 IInteractable이 없었거나
        // 감지 자체가 실패한 경우
        _detectedInteractable = null;
        OnMissed?.Invoke();
    }

    /// <summary>
    /// 감지한 IInteractable과 상호작용을 수행하는 함수
    /// </summary>
    public void ExecuteInteraction()
    {
        if (_detectedInteractable == null) return;

        _detectedInteractable.Interact(transform);
        Debug.Log("상호작용 가능한 대상의 Interact 함수 호출");
    }
}
