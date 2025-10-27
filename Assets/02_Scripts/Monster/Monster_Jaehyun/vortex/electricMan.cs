using UnityEngine;

public class electricMan : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.tag == "Monster")
    //    {
    //        Debug.Log("醱給 馬雖脾");
    //    }
    //}
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //}
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Monster")
        {
            Debug.Log("醱給 馬雖脾");
        }
    }

}
