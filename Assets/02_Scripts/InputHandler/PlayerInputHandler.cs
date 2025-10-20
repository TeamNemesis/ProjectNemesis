using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// NewInput을 통해 플레이어의 여러가지 입력을 받아오고 
/// 각 입력에 대한 이벤트를 제공하는 클래스
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;       // PlayerInput 컴포넌트 참조

    public event Action<Vector2> OnMoveInput;        // 이동 입력 이벤트
    
    public event Action OnDashInput;                 // 대시 입력 이벤트
    public event Action OnInteractInput;             // 상호작용 입력 이벤트

    public event Action OnNomralAttackInput;         // 일반공격 입력 이벤트
    public event Action OnGrenadeAttackInput;        // 유탄공격 입력 이벤트
    public event Action OnGrenadeAttackInputEnded;   // 유탄공격 입력 종료 이벤트
    public event Action OnSpecialAttackInput;        // 특수공격 입력 이벤트

    Vector2 _moveDir;  // 이동 입력을 저장할 변수

    private void Awake()
    {
        // 현재 액션 맵 가져오기
        var actionMap = _playerInput.actions.FindActionMap("Player");

        // 각 액션에 대한 콜백 함수 등록
        actionMap["Move"].performed += OnMove;
        actionMap["Move"].canceled += OnMove;
        actionMap["Dash"].started += OnDash;
        actionMap["Interact"].started += OnInteract;
        actionMap["NormalAttack"].started += OnNormalAttack;
        actionMap["GrenadeAttack"].started += OnGrenadeAttack;
        actionMap["GrenadeAttack"].canceled += OnGrenadeAttack;
        actionMap["SpecialAttack"].started += OnSpecialAttack;
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
            _moveDir = value.ReadValue<Vector2>();
            //Debug.Log($"이동 입력받음: {_moveDir}");
        }
        else if (value.canceled)
        {
            _moveDir = Vector2.zero;
            //Debug.Log("이동 입력 멈춤");
        }
    }

    /// <summary>
    /// 일반 공격 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnNormalAttack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("일반공격 입력받음");
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
            Debug.Log("유탄공격 입력받음");
            OnGrenadeAttackInput?.Invoke();
        }
        else if (value.canceled)
        {
            Debug.Log("유탄공격 입력끝남");
            OnGrenadeAttackInputEnded?.Invoke();
        }
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
    }
}