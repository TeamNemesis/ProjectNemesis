//using System;
//using UnityEngine;
//using UnityEngine.InputSystem;

///// <summary>
///// 플레이어의 입력을 받아오고 이벤트를 통해
///// 다른 컴포넌트에 전달하는 역할을 합니다.
///// </summary>
//public class PlayerInput : MonoBehaviour
//{
//    Vector2 _moveInput; // 이동 입력 벡터

//    public event Action<Vector2> OnMoveInput; // 이동 입력 이벤트
//    public event Action OnAttackInput; // 공격 입력 이벤트
//    public event Action OnDashInput; // 대시 입력 이벤트
//    public event Action OnInteractInput; // 상호작용 입력 이벤트
//    public event Action OnGrenadeInput; // 유탄 입력 이벤트
//    public event Action OnGrenadeMInput; // 유탄 마우스 입력 이벤트
//    public event Action 
//    public event Action OnSpecialInput; // 특수공격 입력 이벤트

//    public void OnMove(InputAction.CallbackContext value)   //Move
//    {
//        Vector2 input = value.ReadValue<Vector2>();
//        if (input != null)
//        {
//            moveVec = new Vector3(input.x, 0f, input.y);    //이동방향
//            Debug.Log($"SEND_MESSAGE : {input.magnitude}"); //받아오는값 출력
//        }
//    }

//    public void OnAttackM(InputAction.CallbackContext value)
//    {
//        attackInput = value.ReadValue<Vector2>();
//        Debug.Log("AttackM");
//    }

//    public void OnAttack(InputAction.CallbackContext value)
//    {
//        Debug.Log("Attack 입력");
//        Attack1();
//    }

//    public void OnDash(InputAction.CallbackContext value) //invoke unity event확인용
//    {
//        if (value.started)
//        {
//            transform.position += transform.forward * dashDistance; // 바라본 방향으로 앞으로 조금 이동
//            Debug.Log("Dash");
//        }
//    }

//    void OnInteract(InputAction.CallbackContext value)   //Interact
//    {
//        OnInteractInput?.Invoke();
//        //Debug.Log("상호작용!");
//    }

//    public void OnGrenade(InputAction.CallbackContext value)
//    {
//        if (value.started)
//        {
//            Debug.Log("유탄");
//        }
//        //방향 회전
//        //유탄 발사

//    }

//    public void OnGrenade_M(InputAction.CallbackContext value)
//    {
//        if (value.started)  //클릭
//        {
//            if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()),
//                                out RaycastHit hit,
//                                100f,
//                                groundMask))
//            {
//                currentRange = Instantiate(rangePrefab, hit.point, Quaternion.identity);
//                isDragging = true;
//            }
//        }
//        else if (value.canceled)    //드랍
//        {
//            if (currentRange != null)
//                Destroy(currentRange);

//            isDragging = false;
//        }
//    }

//    public void OnSpecial(InputAction.CallbackContext value)
//    {
//        if (value.started)
//        {
//            Debug.Log("특수공격");
//        }
//    }
//}