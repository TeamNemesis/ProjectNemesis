using System;
using UnityEngine;

public class TechSelectPackInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] TechItem _techItem;

    public override InteractableType InteractableType => InteractableType.TechSelectPack;

    public override Vector3 GuidePoint => _guidePoint.position;

    public override event Action<IInteractable> OnInteracted;

    public override void StartInteract(Transform subject)
    {
        _techItem.GetSkill();
    }
}