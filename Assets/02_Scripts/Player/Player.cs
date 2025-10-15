using UnityEngine;

/// <summary>
/// 플레이어의 주요 컴포넌트들을 관리하는 최상위 클래스
/// </summary>
public class Player : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] PlayerModel _model;                       // 플레이어 모델 컴포넌트
    [SerializeField] PlayerMover _mover;                       // 플레이어 이동 컴포넌트
    [SerializeField] PlayerDasher _dasher;                     // 플레이어 대시 컴포넌트
    [SerializeField] PlayerWeaponController _weaponController; // 플레이어 무기 관리 컴포넌트
    [SerializeField] PlayerAnimator _animator;                 // 플레이어 애니메이터 컴포넌트
    [SerializeField] PlayerGrenadeAttacker _grenadeAttacker;   // 플레이어 유탄 발사 컴포넌트

    [Header("----- UI 컴포넌트 -----")]
    // [SerializeField] PlayerView _view;                         // 플레이어 UI 컴포넌트

    [Header("----- 상호작용 컴포넌트 -----")]
    [SerializeField] InteractableDetector _interactableDetector;        //상호작용 감지기
    [SerializeField] InteractionGuideView _interactableGuideView;       //상호작용 안내 뷰

    [SerializeField] bool _isInteractable; //상호작용 가능 여부

    [Header("----- 무기가 바뀌면 같이 바뀌는 컴포넌트(읽기 전용) -----")]
    [SerializeField] PlayerNormalAttacker _normalAttacker;     // 플레이어 일반 공격 컴포넌트
    //[SerializeField] PlayerGrenadeAttacker _grenadeAttacker;   // 플레이어 유탄 발사 컴포넌트(얘는 일단 안바뀜)
    [SerializeField] PlayerSpecialAttacker _specialAttacker;   // 플레이어 특수 공격 컴포넌트

    [Header("----- 읽기 전용 -----")]
    [SerializeField] PlayerWeaponSet _currentWeaponSet;        // 현재 플레이어 무기 세트

    /// <summary>
    /// Player 초기화 함수
    /// </summary>
    public void Initialize()
    {


        _weaponController.OnWeaponChanged += OnWeaponChanged;

        _interactableDetector.OnDetected += InteractableDetected;
        _interactableDetector.OnMissed += InteractableMissed;

        _interactableGuideView.Initialize();

        _model.Initialize();
        _weaponController.Initialize();
    }

    public void OnWeaponInteracted(WeaponType newWeaponType)
    {
        _weaponController.OnWeaponInteracted(newWeaponType);
    }

    /// <summary>
    /// 무기가 변경되었을 때 호출되는 함수
    /// 컴포넌트들을 무기에 맞게 변경한다.
    /// </summary>
    /// <param name="weaponType"></param>
    void OnWeaponChanged(WeaponType weaponType)
    {
        _currentWeaponSet = GameManager.Instance.ResourceManager.PlayerWeaponSetMap[weaponType];

        _normalAttacker = _currentWeaponSet.NormalAttacker;
        //_grenadeAttacker = _currentWeaponSet.GrenadeAttacker;
        _specialAttacker = _currentWeaponSet.SpecialAttacker;

        _animator.SetAnimator(_currentWeaponSet.AnimController);
        // 만약 이펙트가 바뀌거나 사운드 등 효과가 필요해서 이벤트가 필요하다면
        // 무기변경 이벤트를 만들어서 여기에 추가
    }

    /// <summary>
    /// 플레이어가 이동 입력을 받았을 때 호출되는 함수
    /// </summary>
    /// <param name="moveDir"></param>
    public void Move(Vector2 moveDir)
    {
        _mover.Move(moveDir);
        _animator.OnMove(moveDir.magnitude);
    }

    /// <summary>
    /// 플레이어가 대시 입력을 받았을 때 호출되는 함수
    /// </summary>
    public void Dash()
    {
        _dasher.Dash();
    }

    /// <summary>
    /// 플레이어가 일반 공격 입력을 받았을 때 호출되는 함수
    /// </summary>
    public void NormalAttack()
    {
        _normalAttacker.Attack();
        _animator.OnNormalAttack();
    }

    /// <summary>
    /// 플레이어가 유탄 공격 입력을 받았을 때 호출되는 함수
    /// </summary>
    public void GrenadeAttack()
    {
        _grenadeAttacker.GrenadeAttack();
    }

    /// <summary>
    /// 플레이어가 특수 공격 입력을 받았을 때 호출되는 함수
    /// </summary>
    public void SpecialAttack()
    {
        _specialAttacker.SpecialAttack();
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
}