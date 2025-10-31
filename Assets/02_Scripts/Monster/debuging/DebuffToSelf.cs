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
            DebuffHandler.DebuffData posion = DebuffHandler.DebuffData.CreatePoison();
            if (posion != null)
            {
                handler.ApplyDebuff(posion);
            }
        }
    }
}
