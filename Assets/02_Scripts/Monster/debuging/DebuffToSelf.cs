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
            DebuffHandler.DebuffData stun = DebuffHandler.DebuffData.CreateBinding();
            if (stun != null)
            {
                handler.ApplyDebuff(stun);
            }
        }
    }
}
