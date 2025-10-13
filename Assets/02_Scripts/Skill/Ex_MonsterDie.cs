using System;
using UnityEngine;

public class Ex_MonsterDie : MonoBehaviour
{
    public static event Action MonsterDie;

    public void OnDie()
    {
        MonsterDie.Invoke();
        Destroy(gameObject);
    }
}




public class Ex_SkillActive : MonoBehaviour
{
    public void SkillActive()
    {
        Ex_MonsterDie.MonsterDie += Explode;
    }

    public void Explode()
    {

    }

}

