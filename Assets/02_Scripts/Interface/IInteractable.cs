using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상호작용 가능한 인터페이스
/// 이 인터페이스를 구현하는 오브젝트는 플레이어와 상호작용할 수 있습니다.
/// Interactable 오브젝트는 플레이어가 접근했을 때 상호작용 가이드를 표시하고,
/// InteractableDetector 스크립트를 통해 상호작용을 처리합니다.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 상호작용 가이드를 표시할 월드 좌표
    /// </summary>
    Vector3 GuidePoint { get; }

    /// <summary>
    /// 상호작용을 실행하는 함수
    /// </summary>
    void Interact(Transform subject);
}
