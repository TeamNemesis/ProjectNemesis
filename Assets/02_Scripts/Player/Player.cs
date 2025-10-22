using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어의 주요 컴포넌트들을 관리하는 최상위 클래스
/// - 상태머신 전환(EvaluateTransitions)과 입력 소비를 담당
/// - 무기별 일반 공격 동작은 _normalAttacker(추상/베이스)에 위임
/// - 애니메이션 이벤트는 Player에서 받아 해당 attacker의 발사/종료 메서드를 호출
/// </summary>
public class Player : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] PlayerModel _playerModel;
    [SerializeField] CharacterController _characterController; // 플레이어 캐릭터 컨트롤러 컴포넌트
    [SerializeField] PlayerMover _mover;                       // 플레이어 이동 컴포넌트
    [SerializeField] PlayerDasher _dasher;                     // 플레이어 대시 컴포넌트
    [SerializeField] PlayerWeaponController _weaponController; // 플레이어 무기 관리 컴포넌트
    [SerializeField] PlayerAnimator _animator;                 // 플레이어 애니메이터 컴포넌트
    [SerializeField] PlayerAnimationEventForwarder _forwarder; // 플레이어 애니메이션 이벤트 포워더 컴포넌트
    [SerializeField] PlayerGrenadeAttacker _grenadeAttacker;   // 플레이어 유탄 발사 컴포넌트

    [SerializeField] PlayerNormalAttacker[] _normalAttackers;          // 플레이어 일반 공격 컴포넌트 배열 (무기별로 다름)
    [SerializeField] PlayerSpecialAttacker[] _specialAttackers;      // 플레이어 특수 공격 컴포넌트 배열 (무기별로 다름)

    [Header("----- 상호작용 컴포넌트 -----")]
    [SerializeField] InteractionController _interactionController;      // 상호작용 컨트롤러
    [SerializeField] InteractableDetector _interactableDetector;        // 상호작용 감지기
    [SerializeField] InteractionGuideView _interactableGuideView;       // 상호작용 안내 뷰

    [SerializeField] bool _isInteractable; // 상호작용 가능 여부

    [Header("----- 무기가 바뀌면 같이 바뀌는 컴포넌트(읽기 전용) -----")]
    [SerializeField] PlayerNormalAttacker _normalAttacker;     // 플레이어 일반 공격 컴포넌트 (추상/베이스)
    [SerializeField] PlayerSpecialAttacker _specialAttacker;   // 플레이어 특수 공격 컴포넌트

    [Header("----- 읽기 전용 -----")]
    [SerializeField] PlayerWeaponSet _currentWeaponSet;        // 현재 플레이어 무기 세트
    [SerializeField] PlayerStateType _currentStateType;        // 현재 플레이어 상태 타입
    [SerializeField] PlayerStatManager _statManager;           // 플레이어 스탯 매니저

    [Header("----- 입력 상태 플래그 -----")]
    [SerializeField] Vector2 _moveInput;                  // 플레이어 이동 입력 벡터
    [SerializeField] bool _dashPressed;                    // 플레이어 대시 입력 여부 (단발)
    [SerializeField] bool _normalAttackPressed;            // 플레이어 일반 공격 입력 여부 (단발)
    [SerializeField] bool _grenadeAttackPressed;           // 플레이어 유탄 공격 입력 여부 (단발)
    [SerializeField] bool _specialAttackPressed;           // 플레이어 특수 공격 입력 여부 (단발)

    [Header("----- 상태 플래그 (Player가 소유) -----")]
    [SerializeField] bool _canMove;                        // 플레이어 이동 가능 여부
    [SerializeField] bool _isDashing;                      // 플레이어 대시 중 여부
    [SerializeField] bool _isNormalAttacking;              // 플레이어 일반공격 중 여부
    [SerializeField] bool _isSpecialAttacking;             // 플레이어 특수공격 중 여부

    // 읽기 전용 프로퍼티로 외부 접근 제공
    public PlayerNormalAttacker NormalAttacker => _normalAttacker;
    public PlayerSpecialAttacker SpecialAttacker => _specialAttacker;
    public PlayerAnimator Animator => _animator;
    public PlayerModel playerModel => _playerModel;

    public Vector2 MoveInput => _moveInput;
    public bool DashPressed => _dashPressed;
    public bool NormalAttackPressed => _normalAttackPressed;
    public bool GrenadeAttackPressed => _grenadeAttackPressed;
    public bool SpecialAttackPressed => _specialAttackPressed;

    public bool CanMove => _canMove;
    public bool IsDashing => _isDashing;
    public bool IsNormalAttacking => _isNormalAttacking;
    public bool IsSpecialAttacking => _isSpecialAttacking;

    #region 이벤트
    public event Action OnNormalAttackStarted;
    public event Action OnSpecialAttackStarted;
    public event Action OnDashStarted;
    #endregion

    [Header("----- 상태 캐시 -----")]
    StateMachine _stateMachine;                              // 플레이어 상태 머신

    Dictionary<WeaponType, PlayerNormalAttacker> _normalAttackerMap = new(); // 무기 타입별 일반 공격 컴포넌트 맵
    Dictionary<WeaponType, PlayerSpecialAttacker> _specialAttackerMap = new(); // 무기 타입별 특수 공격 컴포넌트 맵

    private void Update()
    {
        // 입력에 따른 상태 전환 판단 (우선순위: NormalAttack > Dash > Move > Idle)
        EvaluateTransitions();

        // 단발 입력은 한 프레임 소비 패턴: 프레임 끝에 리셋
        _dashPressed = false;
        _normalAttackPressed = false;
        _grenadeAttackPressed = false;
        _specialAttackPressed = false;

        // 상태 업데이트 호출
        _stateMachine?.Update();
        _currentStateType = _stateMachine.CurrentType;
    }

    /// <summary>
    /// Player 초기화 함수
    /// - 컴포넌트 초기화, 상태 맵 구성, 초기 상태 설정
    /// </summary>
    public void Initialize()
    {
        _statManager = GameManager.Instance.PlayerStatManager;

        _weaponController.OnWeaponChanged += OnWeaponChanged;

        _interactionController.OnWeaponInteract += OnWeaponInteracted;
        _interactionController.OnDoorInteract += OnDoorInteracted;
        _interactableDetector.OnDetected += InteractableDetected;
        _interactableDetector.OnMissed += InteractableMissed;

        // Attacker 타입별로 매핑
        foreach (var attacker in _normalAttackers)
        {
            _normalAttackerMap[attacker.WeaponType] = attacker;
        }
        foreach (var attacker in _specialAttackers)
        {
            _specialAttackerMap[attacker.WeaponType] = attacker;
        }
        _playerModel.Initialize();
        _mover.Initialize(_characterController);
        _dasher.Initialize(_characterController);
        _forwarder.Initialize(this);
        _weaponController.Initialize();

        _interactionController.Initialize();
        _interactableGuideView.Initialize();

        // 상태 맵 초기화 및 상태머신 설정
        _stateMachine = new StateMachine(this);
    }

    #region 입력 플래그 Setters (InputHandler -> Player)
    public void SetMoveInput(Vector2 moveInput) => _moveInput = moveInput;
    public void SetDashPressed(bool isPressed) => _dashPressed = isPressed;
    public void SetNormalAttackPressed(bool isPressed) => _normalAttackPressed = isPressed;
    public void SetGrenadeAttackPressed(bool isPressed) => _grenadeAttackPressed = isPressed;
    public void SetSpecialAttackPressed(bool isPressed) => _specialAttackPressed = isPressed;
    #endregion

    #region 상태 플래그 Setters (상태 Enter/Exit에서 호출)
    // 상태(Enter/Exit)에서 호출하여 Player가 소유한 플래그를 변경
    public void SetCanMove(bool canMove) => _canMove = canMove;
    public void SetIsDashing(bool isDashing) => _isDashing = isDashing;
    public void SetIsNormalAttacking(bool isNormalAttacking) => _isNormalAttacking = isNormalAttacking;
    public void SetIsSpecialAttacking(bool isSpecialAttacking) => _isSpecialAttacking = isSpecialAttacking;
    #endregion

    #region 상태 전환 평가 및 처리
    /// <summary>
    /// EvaluateTransitions: 입력 소비 + 우선순위 검사
    /// - 무기별로 RequestAttack()을 내부에서 처리하도록 _normalAttacker에 위임 가능
    /// </summary>
    void EvaluateTransitions()
    {
        // 1) 일반 공격 입력 처리(우선순위 최상)
        if (_normalAttackPressed && TryConsumeNormalAttack())
        {
            Debug.Log("일반공격 입력 들어옴");
            if (_normalAttacker != null)
            {
                //// reflection 제거: RequestAttack()이 있으면 직접 호출
                //bool accepted = false;
                //try
                //{
                //    accepted = _normalAttacker.RequestAttack();
                //}
                //catch (Exception ex)
                //{
                //    Debug.LogWarning($"Player.EvaluateTransitions: RequestAttack 호출 중 예외: {ex}");
                //    accepted = false;
                //}

                //Debug.Log("일반공격 시도: " + accepted);

                // 공격 시작이 허용되고 Player가 바쁘지 않다면 상태 전환
                //if (accepted && !IsDashing && !IsNormalAttacking && !IsSpecialAttacking)
                if(!_isDashing && !_isNormalAttacking && !_isSpecialAttacking)
                {
                    // StateMachine을 이용한 전환
                    _stateMachine.ChangeState(PlayerStateType.NormalAttack);
                    return;
                }
            }
        }

        // 2) 대시
        if (_dashPressed && TryConsumeDash())
        {
            if (!IsNormalAttacking && !IsSpecialAttacking && !IsDashing)
            {
                _stateMachine.ChangeState(PlayerStateType.Dash);
                return;
            }
        }

        // 3) 이동
        if (_moveInput.sqrMagnitude > 0.01f && !IsNormalAttacking && !IsSpecialAttacking && !IsDashing)
        {
            if (_stateMachine.CurrentType != PlayerStateType.Move)
            {
                _stateMachine.ChangeState(PlayerStateType.Move);
            }
            return;
        }

        // 4) 기본 Idle
        if (_stateMachine.CurrentType != PlayerStateType.Idle && !IsNormalAttacking && !IsDashing && !IsSpecialAttacking)
        {
            _stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    // 단발 입력 소비 함수 (변경 없음)
    bool TryConsumeNormalAttack()
    {
        if (!_normalAttackPressed) return false;
        _normalAttackPressed = false;
        return true;
    }

    bool TryConsumeSpecialAttack()
{
    if (!_specialAttackPressed) return false;
    _specialAttackPressed = false;
    return true;
}

    bool TryConsumeDash()
    {
        if (!_dashPressed) return false;
        _dashPressed = false;
        return true;
    }
    #endregion

    #region 이동 관련 공개 메서드 (기존 로직 유지)
    /// <summary>
    /// 각 상태가 끝났을 때 Idle 상태로 복귀할 때 호출
    /// </summary>
    public void SetToIdle()
    {
        _stateMachine.ChangeState(PlayerStateType.Idle);
    }
    
    public void Move(Vector2 moveDir)
    {
        _mover.Move(moveDir);
        _animator.OnMove(moveDir.magnitude);
    }

    public void StopMove()
    {
        _mover.Move(Vector2.zero);
        _animator.OnMove(0f);
    }

    public void Dash()
    {
        _dasher.Dash(transform.forward, _statManager.playerDashDistance, 0.5f);
        OnDashStarted?.Invoke();
        _animator.OnDash();
    }
    #endregion

    #region 일반공격 애니메이션 이벤트 라우팅
    // 애니메이션 이벤트에서 Player의 메서드를 호출하도록 설정하세요.
    // 예: 애니메이션 클립의 이벤트에서 "OnAttackFireEvent" 와 "OnAttackEndEvent"를 호출.

    /// <summary>
    /// 애니메이션의 발사(히트) 프레임에서 호출.
    /// 라우팅하여 현재 장착된 attacker의 FireNow() 등 실동작을 실행한다.
    /// </summary>
    public void OnAttackFireEvent()
    {
        if (_normalAttacker == null) return;

        Debug.Log(" OnAttackFireEvent 호출됨");

        // 라이플 계열은 FireNow() 호출
        if (_normalAttacker is PlayerRifleNormalAttacker rifle)
        {
            rifle.FireNow();
            OnNormalAttackStarted?.Invoke();
            return;
        }

        // 근접 콤보 타입은 그 컴포넌트에 맞게 애니메이션 이벤트를 연결하거나
        // Player에서 melee-specific 메서드를 호출하도록 구성하세요.
        // 예: if (_normalAttacker is MeleeComboAttacker melee) melee.OnComboHit();
    }

    /// <summary>
    /// 애니메이션의 공격 종료 프레임에서 호출.
    /// </summary>
    public void OnAttackEndEvent()
    {
        if (_normalAttacker == null) return;

        Debug.Log(" OnAttackEndEvent 호출됨");

        // 라이플일 경우 애니메이션 종료 콜백
        if (_normalAttacker is PlayerRifleNormalAttacker rifle)
        {
            Debug.Log(" Rifle OnAnimationAttackEnd 호출됨");
            rifle.OnAnimationAttackEnd();
            return;
        }

        // 기본적으로 EndAttack 호출
        _normalAttacker.EndAttack();
    }
    #endregion

    #region 특수공격 처리
    public void HandleSpecialStarted()
    {
        // 우선권/상태 검사: 이미 다른 동작 중이면 무시
        if (_isDashing || _isNormalAttacking || _isSpecialAttacking) return;

        if (_specialAttacker != null && _specialAttacker.RequestSpecial())
        {
            // StateMachine으로 전환하면 State에서 StartCharge를 호출하도록 하는 방식(권장)
            _stateMachine.ChangeState(PlayerStateType.SpecialAttack);
            OnSpecialAttackStarted?.Invoke();
        }
    }

    public void HandleSpecialCanceled()
    {
        // 만약 StateMachine에서 취급한다면 State에 전달하거나 직접 호출
        if (_specialAttacker != null && _specialAttacker.IsCharging)
        {
            _specialAttacker.StopChargeAndFire();
        }

        // State 전환(Idle 등)은 _specialAttacker 이벤트 또는 State에서 하도록 설계
    }
    #endregion

    #region 상호작용 처리 (기존 로직)
    public void OnWeaponInteracted(WeaponType newWeaponType)
    {
        _weaponController.OnWeaponInteracted(newWeaponType);
    }

    public void OnDoorInteracted(RoomInfo roomInfo)
    {
        if(roomInfo == null)
        {
            Debug.LogError("Player.OnDoorInteracted 호출 시 roomInfo가 null입니다! 호출자 스택을 확인하세요.");
            return;
        }
        Debug.Log(roomInfo.RoomType + " 방으로 가는 문과 상호작용 함");
        // OnDoorInteract?.Invoke(roomInfo);
    }

    /// <summary>
    /// 플레이어가 상호작용가능한 물체를 감지했고,
    /// 상호작용 입력을 받아 상호작용을 시도할 때 호출되는 함수
    /// </summary>
    public void ExecuteInteraction()
    {
        // 상호작용 불가능 상태라면 함수 종료
        if (!_isInteractable) return;

        // 상호작용 감지기가 감지한 IInteractable과 상호작용 수행
        _interactableDetector.ExecuteInteraction();

        InteractableMissed(); //상호작용 한번 하면 UI 사라지게
    }
    
    /// <summary>
    /// 플레이어가 상호작용가능한 물체를 감지했을 때 호출되는 함수
    /// </summary>
    /// <param name="interactable"></param>
    public void InteractableDetected(IInteractable interactable)
    {
        _isInteractable = true;
        _interactableGuideView.ShowUI(interactable);
    }

    /// <summary>
    /// 플레이어가 상호작용가능한 물체를 
    /// </summary>
    public void InteractableMissed()
    {
        _isInteractable = false;
        _interactableGuideView.HideUI();
    }

    /// <summary>
    /// 무기가 변경되었을 때 호출되는 함수
    /// 컴포넌트들을 무기에 맞게 변경한다.
    /// </summary>
    /// <param name="weaponType"></param>
    void OnWeaponChanged(Weapon weapon)
    {
        _currentWeaponSet = GameManager.Instance.DataManager.WeaponSetMap[weapon.WeaponType];

        _normalAttacker = _normalAttackerMap[weapon.WeaponType];
        _specialAttacker = _specialAttackerMap[weapon.WeaponType];

        if (weapon.WeaponType == WeaponType.Rifle)
        {
            PlayerRifleNormalAttacker playerRifleNormalAttacker = _normalAttacker as PlayerRifleNormalAttacker;
            PlayerRifleSpecialAttacker playerRifleSpecialAttacker = _specialAttacker as PlayerRifleSpecialAttacker;
            RifleWeapon rifleWeapon = weapon as RifleWeapon;

            playerRifleNormalAttacker.Initialize(this, rifleWeapon.FirePos);
            playerRifleSpecialAttacker.Initialize(this);
        }
        //_grenadeAttacker = _currentWeaponSet.GrenadeAttacker;

        _animator.SetAnimator(_currentWeaponSet.AnimController);
        // 만약 이펙트가 바뀌거나 사운드 등 효과가 필요해서 이벤트가 필요하다면
        // 무기변경 이벤트를 만들어서 여기에 추가
    }
    #endregion
}