using System;
using UnityEngine;

public class TechUpgradePackInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;

    public override Vector3 GuidePoint => _guidePoint.position;

    public override InteractableType InteractableType => InteractableType.TechUpgradePack;

    public override event Action<IInteractable> OnInteracted;

    public override void StartInteract(Transform subject)
    {
        Debug.Log("업그레이드 팩과 상호작용 함");
    }
}
