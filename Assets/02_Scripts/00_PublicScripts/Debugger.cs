using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class EventBusEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("µπø¨∫Ø¿Ã 1 »πµÊ"))
        {
            EventBus.HasMutant1 = true;
        }
        if (GUILayout.Button("µπø¨∫Ø¿Ã 2 »πµÊ"))
        {
            EventBus.HasMutant2 = true;
        }
        if (GUILayout.Button("µπø¨∫Ø¿Ã 3 »πµÊ"))
        {
            EventBus.HasMutant3 = true;
        }
        if(GUILayout.Button("µπø¨∫Ø¿Ã 4 »πµÊ"))
        {
            EventBus.HasMutant4 = true;
        }
        if (GUILayout.Button("µπø¨∫Ø¿Ã ∏µŒ ¡¶∞≈"))
        {
            EventBus.HasMutant1 = false;
            EventBus.HasMutant2 = false;
            EventBus.HasMutant3 = false;
            EventBus.HasMutant4 = false;
        }
    }
}