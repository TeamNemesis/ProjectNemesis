using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerAttacker _playerAttacker; //플레이어 공격 컴포넌트
    [SerializeField] PlayerAnimator _playerAnimator; //플레이어 애니메이터 컴포넌트

    [SerializeField] InteractableDetector _interactableDetector; //상호작용 감지기
    [SerializeField] InteractionGuideView _interactableGuideView; //상호작용 안내 UI

    [SerializeField] bool _isInteractable; //상호작용 가능 여부

    //public float hAxis;
    //public float vAxis;
    private float moveSpeed = 10;

    private float attackDistance = 2f; 

    private float dashDistance = 10f; //

    Vector2 attackInput;

    private Vector3 moveVec;
    private Vector3 lastAttackVec;

    private bool isAttackCooling = false;
    [SerializeField] private float attackCooldown = 0.5f;

    [SerializeField] private GameObject rangePrefab;
    [SerializeField] private Camera mainCamera;
    private GameObject currentRange;
    private bool isDragging = false;
    [SerializeField] private LayerMask groundMask;

    public event Action OnInteractInput;
    public event Action<Vector2> OnMoveInput;
    void Start()
    {
        OnInteractInput += Interact;
        _interactableDetector.OnDetected += InteractableDetected;
        _interactableDetector.OnMissed += InteractableMissed;

        _interactableGuideView.Initialize();
    }


    

    public void OnMove(InputAction.CallbackContext value)   //Move
    {
        Vector2 input = value.ReadValue<Vector2>();
        if (input != null)
        {
            moveVec = new Vector3(input.x, 0f, input.y);    //이동방향
            Debug.Log($"SEND_MESSAGE : {input.magnitude}"); //받아오는값 출력
        }
    }

    public void OnAttackM(InputAction.CallbackContext value)    
    {
        attackInput = value.ReadValue<Vector2>();
        Debug.Log("AttackM");
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        Debug.Log("Attack 입력");
        Attack1();
    }
    
    public void OnDash(InputAction.CallbackContext value) //invoke unity event확인용
    {
        if (value.started)
        {
            transform.position += transform.forward * dashDistance; // 바라본 방향으로 앞으로 조금 이동
            Debug.Log("Dash");
        }
    }
    
    void Attack1()
    {
        _playerAnimator.OnAttack(); //공격 애니메이션 재생
        _playerAttacker.Attack(); //공격 실행
    }

    IEnumerator Attack()
    {
        isAttackCooling = true;

        transform.rotation = Quaternion.LookRotation(lastAttackVec);
        transform.position += transform.forward * attackDistance; // 바라본 방향으로 앞으로 조금 이동

        //Debug.Log("쿨타임 시작");

        yield return new WaitForSeconds(attackCooldown);

        isAttackCooling = false;
        //Debug.Log("쿨타임 종료");
    }

    void OnInteract(InputValue value)   //Interact
    {
        OnInteractInput?.Invoke();
        //Debug.Log("상호작용!");
    }

    public void OnGrenade(InputAction.CallbackContext value)    
    {
        if (value.started)
        {
            Debug.Log("유탄");
        }
        //방향 회전
        //유탄 발사
        
    }
    
    public void OnGrenade_M(InputAction.CallbackContext value)
    {
        if (value.started)  //클릭
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()),
                                out RaycastHit hit,
                                100f,
                                groundMask))
            {
                currentRange = Instantiate(rangePrefab, hit.point, Quaternion.identity);
                isDragging = true;
            }
        }
        else if (value.canceled)    //드랍
        {
            if (currentRange != null)
                Destroy(currentRange);

            isDragging = false;
        }
    }

    public void OnSpecial(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            Debug.Log("특수공격");
        }
    }
    void Interact()
    {
        if (!_isInteractable) return;
        _interactableDetector.ExecuteInteraction();
        InteractableMissed(); //상호작용 한번 하면 UI 사라지게
    }

    void InteractableDetected(IInteractable interactable)
    {
        _isInteractable = true;
        _interactableGuideView.ShowUI(interactable);
    }

    void InteractableMissed()
    {
        _isInteractable = false;
        _interactableGuideView.HideUI();
    }
}
