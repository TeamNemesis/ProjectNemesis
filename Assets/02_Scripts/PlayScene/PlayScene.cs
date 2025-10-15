using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Player _player;                               // 플레이어
    [SerializeField] PlayerInputHandler _inputHandler;             // 플레이어 입력 핸들러
    [SerializeField] InteractionController _interactionController; // 상호작용 컨트롤러
    [SerializeField] NextDoorsDecider _nextDoorDecider;            // 다음 방 선택지 결정자

    private void Awake()
    {
        _inputHandler.OnMoveInput += _player.Move;
        _inputHandler.OnDashInput += _player.Dash;
        _inputHandler.OnNomralAttackInput += _player.NormalAttack;
        // 여기에 모바일용 유탄공격 시작 이벤트를 추가하여 UI를 생성하는 메서드를 연결할 수 있습니다.
        _inputHandler.OnGrenadeAttackInputEnded += _player.GrenadeAttack;
        _inputHandler.OnSpecialAttackInput += _player.SpecialAttack;
        _inputHandler.OnInteractInput += _player.ExecuteInteraction;

        _interactionController.OnWeaponInteract += _player.OnWeaponInteracted;
    }

    private void Start()
    {
        _player.Initialize();
        _interactionController.Initialize();
        _nextDoorDecider.Initialize();
    }
}
