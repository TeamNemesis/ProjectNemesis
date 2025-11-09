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

    [Header("----- 모바일 입력 -----")]
    [SerializeField] Joystick _joystick;            // 조이스틱 참조
    bool _isMoblile = false;

    public event Action<Vector3> OnMoveInput;        // 이동 입력 이벤트
    
    public event Action OnDashInput;                 // 대시 입력 이벤트
    public event Action OnInteractInput;             // 상호작용 입력 이벤트

    public event Action OnNormalAttackInput;         // 일반공격 입력 이벤트
    public event Action<Vector3> OnGrenadeAttackInput;        // 유탄공격 입력 이벤트
    public event Action OnGrenadeAttackInputEnded;   // 유탄공격 입력 종료 이벤트
    public event Action OnSpecialAttackInput;        // 특수공격 입력 이벤트
    public event Action OnSpecialAttackInputCanceled;   // 특수공격 입력 종료 이벤트

    Vector3 _moveDir;  // 이동 입력을 저장할 변수
    Coroutine _holdAttackRoutine; // 일반공격 입력 코루틴 참조

    //private void Awake()
    //{
    //    mainCam = Camera.main;
    //    // 현재 액션 맵 가져오기
    //    var actionMap = _playerInput.actions.FindActionMap("Player");
    //    var normal = actionMap["NormalAttack"];
    //    var special = actionMap["SpecialAttack"];

    //    // 각 액션에 대한 콜백 함수 등록
    //    actionMap["Move"].performed += OnMove;
    //    actionMap["Move"].canceled += OnMove;
    //    actionMap["Dash"].started += OnDash;
    //    actionMap["Interact"].started += OnInteract;
    //    normal.started += OnNormalAttackStarted;
    //    normal.canceled += OnNormalAttackCanceled;
    //    actionMap["GrenadeAttack"].started += OnGrenadeAttack;
    //    actionMap["GrenadeAttack"].canceled += OnGrenadeAttack;
    //    special.started += OnSpecialAttack;
    //    special.canceled += OnSpecialAttack;
    //    if (EventBus.IsRewardSelecting)
    //    {
    //        Debug.Log("보상 선택 중 입력 비활성화");
    //    }
    //}

    private void Awake()
    {
        // 임시
        _isMoblile = true;
        // 임시
        mainCam = Camera.main;

        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput is NOT assigned on PlayerInputHandler!");
            return;
        }

        var actionMap = _playerInput.actions.FindActionMap("Player");
        if (actionMap == null)
        {
            Debug.LogError("ActionMap 'Player' not found!");
            return;
        }

        // 안전하게 액션 참조
        var normal = actionMap.FindAction("NormalAttack");
        if (normal == null)
        {
            Debug.LogError("NormalAttack action not found in 'Player' map!");
        }
        else
        {
        }

        // 기존 등록 (원래대로)
        actionMap["Move"].performed += OnMove;
        actionMap["Move"].canceled += OnMove;
        actionMap["Dash"].started += OnDash;
        actionMap["Interact"].started += OnInteract;
        if (normal != null)
        {
            normal.started += OnNormalAttackStarted;
            normal.canceled += OnNormalAttackCanceled;
        }
        actionMap["GrenadeAttack"].started += OnGrenadeAttack;
        actionMap["GrenadeAttack"].canceled += OnGrenadeAttack;
        actionMap["SpecialAttack"].started += OnSpecialAttack;
        actionMap["SpecialAttack"].canceled += OnSpecialAttack;
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
        //if(Application.isMobilePlatform)
        if(_isMoblile)
        {
            // 모바일 플랫폼인 경우 조이스틱 입력 처리
            _moveDir = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);
        }
        // 일반공격 입력중에는 이동 입력 벡터를 0으로
        if (_holdAttackRoutine != null)
        {
            OnMoveInput?.Invoke(Vector3.zero);
            return;
        }
        OnMoveInput?.Invoke(_moveDir);

#if !UNITY_ANDROID
    OnMoveInput?.Invoke(_moveDir);
#endif
		}

		/// <summary>
		/// 이동 입력을 받아오는 함수
		/// </summary>
		/// <param name="value"></param>
		public void OnMove(InputAction.CallbackContext value)
    { 
        if(EventBus.IsRewardSelecting)
            return;
        if (value.performed)
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

    //void OnNormalAttackStarted(InputAction.CallbackContext ctx)
    //{
    //    if (EventBus.IsRewardSelecting)
    //        return;

    //    // 누르기 시작: 코루틴 시작
    //    if (_holdAttackRoutine == null)
    //        _holdAttackRoutine = StartCoroutine(HoldAttackRoutine());
    //}

    //void OnNormalAttackCanceled(InputAction.CallbackContext ctx)
    //{
    //    if (EventBus.IsRewardSelecting)
    //        return;
    //    // 누름 끝: 코루틴 중지
    //    if (_holdAttackRoutine != null)
    //    {
    //        StopCoroutine(_holdAttackRoutine);
    //        _holdAttackRoutine = null;
    //    }
    //}
    void OnNormalAttackStarted(InputAction.CallbackContext ctx)
    {
        if (EventBus.IsRewardSelecting) return;

        if (_holdAttackRoutine == null)
            _holdAttackRoutine = StartCoroutine(HoldAttackRoutine());
    }

    void OnNormalAttackCanceled(InputAction.CallbackContext ctx)
    {
        if (EventBus.IsRewardSelecting) return;
        if (_holdAttackRoutine != null)
        {
            StopCoroutine(_holdAttackRoutine);
            _holdAttackRoutine = null;
        }
    }

    IEnumerator HoldAttackRoutine()
    {
        // 즉시 한 번 공격 실행하고, interval마다 반복
        OnNormalAttackInput?.Invoke();

        while (true)
        {
            yield return new WaitForSeconds(_normalAttackInterval);
            OnNormalAttackInput?.Invoke();
        }
    }

    /// <summary>
    /// 대시 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnDash(InputAction.CallbackContext value)
    {
        if (EventBus.IsRewardSelecting)
            return;
        if (value.started)
        {
            OnDashInput?.Invoke();
        }
    }

    /// <summary>
    /// 상호 작용 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnInteract(InputAction.CallbackContext value)
    {
        if (EventBus.IsRewardSelecting)
            return;
        if (value.started)
        {
            OnInteractInput?.Invoke();
        }
    }

    /// <summary>
    /// 유탄 공격 입력을 받아오는 함수
    /// </summary>
    /// <param name="value"></param>
    public void OnGrenadeAttack(InputAction.CallbackContext value)
    {
        if (EventBus.IsRewardSelecting)
            return; 
        if (value.started)
        {
            if (EventBus.IsColosseumRoom)
            {
                Vector3 monsterPos = EventBus.EliteBoss.transform.position;
                monsterPos.y = 0f;
                OnGrenadeAttackInput?.Invoke(monsterPos);
                return;
            }
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
        if (EventBus.IsRewardSelecting)
            return null;
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
        if (EventBus.IsRewardSelecting)
            return;
        if (value.started)
        {
            OnSpecialAttackInput?.Invoke();
        }
        if(value.canceled)
        {
            OnSpecialAttackInputCanceled?.Invoke();
        }
    }

#if UNITY_ANDROID
    #region mobile

    /// <summary>
    /// 모바일 입력용: 이동 입력 이벤트 호출
    /// </summary>
    public void TriggerMoveInput(Vector3 moveDir)
		{
				_moveDir = moveDir;
				OnMoveInput?.Invoke(moveDir);
		}

		/// <summary>
		/// 모바일 입력용: 대시 입력 이벤트 호출
		/// </summary>
		public void TriggerDashInput()
		{
				OnDashInput?.Invoke();
		}

		/// <summary>
		/// 모바일 입력용: 일반 공격 입력 이벤트 호출
		/// </summary>
		public void TriggerNormalAttackInput()
		{
				OnNomralAttackInput?.Invoke();
		}

		/// <summary>
		/// 모바일 입력용: 특수 공격 시작 이벤트 호출
		/// </summary>
		public void TriggerSpecialAttackInput()
		{
				OnSpecialAttackInput?.Invoke();
		}

		/// <summary>
		/// 모바일 입력용: 특수 공격 취소 이벤트 호출
		/// </summary>
		public void TriggerSpecialAttackCanceled()
		{
				OnSpecialAttackInputCanceled?.Invoke();
		}

		/// <summary>
		/// 모바일 입력용: 유탄 공격 시작 이벤트 호출
		/// </summary>
		public void TriggerGrenadeAttackInput(Vector3 target)
		{
				OnGrenadeAttackInput?.Invoke(target);
		}

		/// <summary>
		/// 모바일 입력용: 유탄 공격 종료 이벤트 호출
		/// </summary>
		public void TriggerGrenadeAttackEnded()
		{
				OnGrenadeAttackInputEnded?.Invoke();
		}


    #endregion
#endif
}