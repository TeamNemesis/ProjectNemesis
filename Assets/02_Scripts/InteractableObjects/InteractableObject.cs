using System;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{

    public abstract InteractableType InteractableType { get; }

    public abstract Vector3 GuidePoint { get; }

    public virtual event Action<IInteractable> OnInteracted;
    public virtual event Action<IInteractable> OnInteractionCompleted;

    public abstract void StartInteract(Transform subject);
    protected void NotifyInteractionCompleted() => OnInteractionCompleted?.Invoke(this);
}