using System;
using UnityEngine;

/// <summary>
/// 플레이어의 영구강화를 상호작용하기 위한 클래스
/// </summary>
public class ReinforceInteractor : InteractableObject
{
    public override InteractableType InteractableType => InteractableType.Reinforce;

    public override event Action<IInteractable> OnInteracted;

    public override void GetInteractionMessage(out string title, out string instruction)
    {
        title = "ReinforceTitle";
        instruction = "ReinforceScript";

    }

    public override bool TryInteract(Transform subject)
    {
        GameManager.Instance.UIManager.OnUpgradePanel();
        return true;
    }
}
