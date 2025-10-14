using UnityEngine;

public class DebuffData : MonoBehaviour
{
    public string debuffName;
    public float debuffDuration;
    public float debuffValue = 0;
    public bool isDibuffed = false;


    public DebuffData(string name, float duration, float dps)
    {
        debuffName = name;
        debuffDuration = duration;
        debuffValue = dps;
    }

    public DebuffData(string name , float duration , bool apply)
    {
        debuffName = name;
        debuffDuration = duration;
        isDibuffed = apply;
    }
}
