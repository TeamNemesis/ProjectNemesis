using UnityEngine;

public class DebuffToSelf : MonoBehaviour
{
    [SerializeField]
    DebuffHandler handler;

    private void Start()
    {
        handler = GetComponent<DebuffHandler>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DebuffHandler.DebuffData poison = DebuffHandler.DebuffData.CreatePoison();
            if (poison != null)
            {
                handler.ApplyDebuff(poison);
            }
            DebuffHandler.DebuffData overloard = DebuffHandler.DebuffData.CreateOverload();
            if (overloard != null)
            {
                handler.ApplyDebuff(overloard);
            }
        }
    }
}
