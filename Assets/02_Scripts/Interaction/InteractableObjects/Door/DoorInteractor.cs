using UnityEngine;

/// <summary>
/// Door의 상호작용을 담당하는 클래스
/// </summary>
public class DoorInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _guidePoint;

    public Vector3 GuidePoint => _guidePoint.position;

    public InteractableType InteractableType => InteractableType.Door;

    public void Interact(Transform subject)
    {
        Debug.Log("문과 상호작용 함");
    }
}
