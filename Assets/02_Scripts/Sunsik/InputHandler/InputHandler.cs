using System;
using UnityEngine;

/// <summary>
/// 입력을 받는 클래스
/// </summary>
public abstract class InputHandler : MonoBehaviour
{
    public abstract event Action<Vector3> OnMoveInput; // 이동 이벤트
    public abstract event Action<Vector2> OnCameraRotInput; // 회전 이벤트
    public abstract event Action OnAttackInput;    // 공격 이벤트
    public abstract event Action OnInteractInput;   // 상호작용 이벤트
}
