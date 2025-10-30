using UnityEngine;

public class invincibility : MonoBehaviour
{
    CharacterController controller;
    CapsuleCollider capsuleCollider;
    void Start()
    {
        OnInvincibile();
    }

    private void OnInvincibile()
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        controller.radius = 0f;
        controller.height = 0f;
        capsuleCollider.enabled = false;
        //controller.enabled = false;   //이동 못함
        //Physics.IgnoreCollision(capsuleCollider, GameObject.FindWithTag("Enemy").GetComponent<Collider>(), true);

    }
}
