using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라를 조작하는 공통 기능을 가진 추상 클래스
/// </summary>
public abstract class CameraController : MonoBehaviour
{
    /// <summary>
    /// 카메라를 회전시키는 함수
    /// </summary>
    /// <param name="rotInput"></param>
    public abstract void Rotate(Vector2 rotInput);

    /// <summary>
    /// 카메라를 줌하는 함수
    /// </summary>
    /// <param name="zoomInput"></param>
    public abstract void Zoom(float zoomInput);
}
