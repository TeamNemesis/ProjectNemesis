using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미니맵 카메라의 위치를 조정하는 클래스
/// </summary>
public class MiniMapCameraController : MonoBehaviour
{
    [SerializeField] Camera _miniMapCamera; // 미니맵 카메라 컴포넌트
    
    public void FollowTarget(Transform target)
    {
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}
