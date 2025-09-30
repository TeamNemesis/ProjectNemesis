using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상호작용 가능한 인터페이스
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
