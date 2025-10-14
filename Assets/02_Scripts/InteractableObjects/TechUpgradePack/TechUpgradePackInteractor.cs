using UnityEngine;

public class TechUpgradePackInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _guidePoint;

    public Vector3 GuidePoint => _guidePoint.position;

    public InteractableType InteractableType => InteractableType.TechUpgradePack;

    public void Interact(Transform subject)
    {
        Debug.Log("업그레이드 팩과 상호작용 함");
    }
}
