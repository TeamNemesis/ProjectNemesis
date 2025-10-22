using System;
using UnityEngine;

public class SkillItem : MonoBehaviour,IInteractable
{
    public Vector3 GuidePoint => throw new NotImplementedException();

    public InteractableType InteractableType => throw new NotImplementedException();

    public event Action<IInteractable> OnInteracted;
    public event Action<IInteractable> OnInteractionCompleted;

    public void StartInteract()
    {
        SkillChoose skillchoose = GetComponent<SkillChoose>();
        skillchoose.SetSkillCompany(GameManager.Instance.skillManager.skill_Four);
        skillchoose.SetBtn();
    }

    public void OnTriggerEnter(Collider other)
    {
        StartInteract();
    }

    public void StartInteract(Transform subject)
    {
        throw new NotImplementedException();
    }
}
