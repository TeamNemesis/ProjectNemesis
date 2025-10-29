using System;
using UnityEngine;

public class ShopItem : InteractableObject
{
    public override InteractableType InteractableType => InteractableType.ShopItem;

    public override event Action<IInteractable> OnInteracted;

    public override void GetInteractionMessage(out string title, out string instruction)
    {
        throw new NotImplementedException();
    }

    public override bool TryInteract(Transform subject)
    {
        throw new NotImplementedException();
    }
}