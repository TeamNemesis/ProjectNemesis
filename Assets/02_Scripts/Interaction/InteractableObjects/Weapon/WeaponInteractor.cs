using UnityEngine;

public class WeaponInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _guidePoint;

    public Vector3 GuidePoint => _guidePoint.position;

    public InteractableType InteractableType => InteractableType.Weapon;

    public void Interact(Transform subject)
    {
        Debug.Log("무기와 상호작용 함");
    }
}