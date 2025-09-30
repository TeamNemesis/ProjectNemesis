using UnityEngine;

/// <summary>
/// 상호작용 가능한 물체를 구현하기 위한 클래스
/// </summary>
public class RedBall : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _guidePoint;

    public Vector3 GuidePoint => _guidePoint.position;

    public void Interact(Transform subject)
    {
        
    }
}
