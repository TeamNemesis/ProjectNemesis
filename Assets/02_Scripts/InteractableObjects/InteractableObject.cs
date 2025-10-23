using System;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{

    public abstract InteractableType InteractableType { get; }

    public abstract Vector3 GuidePoint { get; }

    public abstract event Action<IInteractable> OnInteracted;

    public abstract void StartInteract(Transform subject);
}