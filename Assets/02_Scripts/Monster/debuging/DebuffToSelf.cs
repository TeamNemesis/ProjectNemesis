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
            DebuffHandler.DebuffData confuse = DebuffHandler.DebuffData.CreateConfusion();
            if (confuse != null)
            {
                handler.ApplyDebuff(confuse);
            }
        }
    }
}
