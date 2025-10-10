using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.iOS;

public class PlayerController : MonoBehaviour
{
    [SerializeField] InteractableDetector _interactableDetector; //상호작용 감지기
    [SerializeField] InteractionGuideView _interactableGuideView; //상호작용 안내 UI
    [SerializeField] bool _isInteractable; //상호작용 가능 여부

    //public float hAxis;
    //public float vAxis;
    private float moveSpeed = 10;

    private float clickMoveDistance = 10f; //

    private Vector3 moveVec;
    private Vector3 lastAttackVec;

    private bool isAttackCooling = false;
    [SerializeField] private float attackCooldown = 0.5f;
    

    public event Action OnInteractInput;
    void Start()
    {
        OnInteractInput += Interact;
        _interactableDetector.OnDetected += InteractableDetected;
        _interactableDetector.OnMissed += InteractableMissed;
    }

    void Update()
    {

        //hAxis = Input.GetAxisRaw("Horizontal"); //
        //vAxis = Input.GetAxisRaw("Vertical");

        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //transform.position += moveVec * speed * Time.deltaTime;

        //transform.LookAt(transform.position + moveVec);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, 100f))
        //    {
        //        Vector3 targetPos = hit.point;
        //        targetPos.y = transform.position.y; // 캐릭터 높이 유지
        //        transform.LookAt(targetPos);

        //        transform.position += transform.forward * clickMoveDistance; // 바라본 방향으로 앞으로 조금 이동
        //    }
        //}

        bool hasControl = (moveVec != Vector3.zero);
        if (hasControl) 
        {
            transform.rotation = Quaternion.LookRotation(moveVec);              //이동방향으로 전환
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);  //이동방향으로 이동
        }

        //bool hasControl2 = (attackVec != Vector3.zero);
        //if (hasControl2)
        //{
        //    transform.rotation = Quaternion.LookRotation(attackVec);
        //}
        

    }

    void OnMove(InputValue value)   //Move
    {
        Vector2 input = value.Get<Vector2>();
        if (input != null)
        {
            moveVec = new Vector3(input.x, 0f, input.y);    //이동방향
            //Debug.Log($"SEND_MESSAGE : {input.magnitude}"); //받아오는값 출력
        }
    }
    void OnAttack(InputValue value) //Attack
    {
        Vector2 attackInput;
        attackInput = value.Get<Vector2>();
        Vector3 attackVec = new Vector3(attackInput.x, 0f, attackInput.y);

        // 스틱을 일정 세기 이상 밀고 있고 쿨타임이 아닐 때
        if (attackVec.magnitude > 0.1f && !isAttackCooling)
        {
            lastAttackVec = attackVec.normalized;
            StartCoroutine(Attack());
        }
    }
    void OnDash(InputValue value)   //Dash
    {
        transform.position += transform.forward * clickMoveDistance; // 바라본 방향으로 앞으로 조금 이동
        //Debug.Log("Dash");
    }
    IEnumerator Attack()
    {
        isAttackCooling = true;

        transform.rotation = Quaternion.LookRotation(lastAttackVec);
        transform.position += transform.forward * clickMoveDistance; // 바라본 방향으로 앞으로 조금 이동

        //Debug.Log("쿨타임 시작");

        yield return new WaitForSeconds(attackCooldown);

        isAttackCooling = false;
        //Debug.Log("쿨타임 종료");
    }

    void OnInteract(InputValue value)   //Interact
    {
        OnInteractInput?.Invoke();
        Debug.Log("상호작용!");
    }

    void Interact()
    {
        if (!_isInteractable) return;
        _interactableDetector.ExecuteInteraction();
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
