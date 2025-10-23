using System;
using UnityEngine;

public enum TechSelectPackType
{
    Company1,
    Company2,
    Company3,
    Company4,
    Company5,
}

public class TechSelectPack : InteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] TechItem _techItem;
    [SerializeField] TechSelectPackType _packType;

    public TechSelectPackType PackType => _packType;
    public override InteractableType InteractableType => InteractableType.TechSelectPack;
    public override Vector3 GuidePoint => _guidePoint.position;

    public override event Action<IInteractable> OnInteracted;

    public void Initialize(TechSelectPackType packType)
    {
        _packType = packType;
    }

    public override void StartInteract(Transform subject)
    {
        _techItem.GetSkill(_packType);
    }
}
