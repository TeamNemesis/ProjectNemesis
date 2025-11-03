using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// NewInput을 통해 플레이어의 여러가지 입력을 받아오고 
/// 각 입력에 대한 이벤트를 제공하는 클래스
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] float _normalAttackInterval = 0.2f; // 일반공격 입력 반복 간격

    [SerializeField] PlayerInput _playerInput;       // PlayerInput 컴포넌트 참조
    [SerializeField] Camera mainCam;               // 메인 카메라 참조
    [SerializeField] LayerMask _groundLayer;        // Ground 레이어 마스크

    public event Action<Vector3> OnMoveInput;        // 이동 입력 이벤트
    
    public event Action OnDashInput;                 // 대시 입력 이벤트
    public event Action OnInteractInput;             // 상호작용 입력 이벤트

    public event Action OnNomralAttackInput;         // 일반공격 입력 이벤트
    public event Action<Vector3> OnGrenadeAttackInput;        // 유탄공격 입력 이벤트
    public event Action OnGrenadeAttackInputEnded;   // 유탄공격 입력 종료 이벤트
    public event Action OnSpecialAttackInput;        // 특수공격 입력 이벤트
    public event Action OnSpecialAttackInputCanceled;   // 특수공격 입력 종료 이벤트

    Vector3 _moveDir;  // 이동 입력을 저장할 변수
    Coroutine _holdAttackRoutine; // 일반공격 입력 코루틴 참조

    private void Awake()
    {
        mainCam = Camera.main;
        // 현재 액션 맵 가져오기
        var actionMap = _playerInput.actions.FindActionMap("Player");
        var normal = actionMap["NormalAttack"];
        var special = actionMap["SpecialAttack"];

        // 각 액션에 대한 콜백 함수 등록
        actionMap["Move"].performed += OnMove;
        actionMap["Move"].canceled += OnMove;
        actionMap["Dash"].started += OnDash;
        actionMap["Interact"].started += OnInteract;
        normal.started += OnNormalAttackStarted;
        normal.canceled += OnNormalAttackCanceled;
        actionMap["GrenadeAttack"].started += OnGrenadeAttack;
        actionMap["GrenadeAttack"].canceled += OnGrenadeAttack;
        special.started += OnSpecialAttack;
        special.canceled += OnSpecialAttack;
    }

    private void OnDestroy()
    {
        // 콜백 함수 해제
        var actionMap = _playerInput.actions.FindActionMap("Player");
        var normal = actionMap["NormalAttack"];
        actionMap["Move"].performed -= OnMove;
        actionMap["Move"].canceled -= OnMove;
        actionMap["Dash"].started -= OnDash;
        actionMap["Interact"].started -= OnInteract;
        normal.started -= OnNormalAttackStarted;
        normal.canceled -= OnNormalAttackCanceled;
        actionMap["GrenadeAttack"].started -= OnGrenadeAttack;
        actionMap["GrenadeAttack"].canceled -= OnGrenadeAttack;
        actionMap["SpecialAttack"].started -= OnSpecialAttack;
        actionMap["SpecialAttack"].canceled -= OnSpecialAttack;
    }

    private void Update()
    {
        OnMoveInput?.Invoke(_moveDir);
    }

    /// <summary>
    /// 이동 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputAction.CallbackContext value)
    { 
        if(value.performed)
        {
            Vector2 moveDir = value.ReadValue<Vector2>();
            _moveDir = new Vector3(moveDir.x, 0, moveDir.y);
            //Debug.Log($"이동 입력받음: {_moveDir}");
        }
        else if (value.canceled)
        {
            _moveDir = Vector3.zero;
            //Debug.Log("이동 입력 멈춤");
        }
    }

    void OnNormalAttackStarted(InputAction.CallbackContext ctx)
    {
        // 누르기 시작: 코루틴 시작
        if (_holdAttackRoutine == null)
            _holdAttackRoutine = StartCoroutine(HoldAttackRoutine());
    }

    void OnNormalAttackCanceled(InputAction.CallbackContext ctx)
    {
        // 누름 끝: 코루틴 중지
        if (_holdAttackRoutine != null)
        {
            StopCoroutine(_holdAttackRoutine);
            _holdAttackRoutine = null;
        }
    }

    IEnumerator HoldAttackRoutine()
    {
        // 즉시 한 번 공격 실행하고, interval마다 반복
        OnNomralAttackInput?.Invoke();

        while (true)
        {
            yield return new WaitForSeconds(_normalAttackInterval);
            OnNomralAttackInput?.Invoke();
        }
    }

    /// <summary>
    /// 대시 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnDash(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("대시 입력받음");
            OnDashInput?.Invoke();
        }
    }

    /// <summary>
    /// 상호 작용 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnInteract(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("상호작용 입력받음");
            OnInteractInput?.Invoke();
        }
    }

    /// <summary>
    /// 유탄 공격 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnGrenadeAttack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            // 우클릭 시작 시
            Vector3? target = GetMouseGroundPoint();
            if (target.HasValue)
            {
                
                OnGrenadeAttackInput?.Invoke(target.Value);
            }
        }
        else if (value.canceled)
        {
            
            OnGrenadeAttackInputEnded?.Invoke();
        }
    }

    /// <summary>
    /// 마우스로 Ground 지점 감지
    /// </summary>
    private Vector3? GetMouseGroundPoint()
    {
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, _groundLayer ))
        {
            return hit.point;
        }

        return null; // 맞은게 없으면 null 반환
    }



    /// <summary>
    /// 특수공격 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnSpecialAttack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("특수공격 입력받음");
            OnSpecialAttackInput?.Invoke();
        }
        if(value.canceled)
        {
            Debug.Log("특수공격 입력끝남");
            OnSpecialAttackInputCanceled?.Invoke();
        }
    }
}