using System;
using UnityEngine;

public class SkillItem : MonoBehaviour
{

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
}
