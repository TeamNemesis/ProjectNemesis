using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Player _player;                               // 플레이어
    [SerializeField] PlayerInputHandler _inputHandler;             // 플레이어 입력 핸들러
    [SerializeField] MapController _mapController;                 // 맵 컨트롤러

    public MapController MapController => _mapController;

    private void Awake()
    {
        //_inputHandler.OnDashInput += _player.Dash;
        //_inputHandler.OnNomralAttackInput += _player.NormalAttack;
        //// 여기에 모바일용 유탄공격 시작 이벤트를 추가하여 UI를 생성하는 메서드를 연결할 수 있습니다.
        //_inputHandler.OnGrenadeAttackInputEnded += _player.GrenadeAttack;
        //_inputHandler.OnSpecialAttackInput += _player.SpecialAttack;
        _inputHandler.OnInteractInput += _player.ExecuteInteraction;

        // PlayerInputHandler의 이벤트와 Player 메서드 연결
        _inputHandler.OnMoveInput += _player.SetMoveInput;
        _inputHandler.OnDashInput += () => _player.SetDashPressed(true);
        _inputHandler.OnNomralAttackInput += () => _player.SetNormalAttackPressed(true);
        _inputHandler.OnGrenadeAttackInputEnded += () => _player.SetGrenadeAttackPressed(false);
        _inputHandler.OnSpecialAttackInput += _player.HandleSpecialStarted;
        _inputHandler.OnSpecialAttackInputCanceled += _player.HandleSpecialCanceled;
        _inputHandler.OnInteractInput += _player.ExecuteInteraction;

        // PlayScene.Awake
    }

    private void Start()
    {
        _player.Initialize();
        if (MapController == null)
        {
            return;
        }
        _mapController.Initialize();
    }

    private void Update()
    {
    }
}
