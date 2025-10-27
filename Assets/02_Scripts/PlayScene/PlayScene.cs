using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Player _player;                               // 플레이어
    [SerializeField] PlayerInputHandler _inputHandler;             // 플레이어 입력 핸들러
    [SerializeField] MapController _mapController;                 // 맵 컨트롤러
    [SerializeField] PlaySceneView _playSceneView;                 // 플레이 씬 뷰
    [SerializeField] CameraMover _cameraMover;                     // 카메라 무버

    public MapController MapController => _mapController;
    public Player player => _player;

    private void Awake()
    {
        _inputHandler.OnInteractInput += _player.ExecuteInteraction;

        // PlayerInputHandler의 이벤트와 Player 메서드 연결
        _inputHandler.OnMoveInput += _player.SetMoveInput;
        _inputHandler.OnDashInput += () => _player.SetDashPressed(true);
        _inputHandler.OnNomralAttackInput += () => _player.SetNormalAttackPressed(true);
        _inputHandler.OnGrenadeAttackInputEnded += () => _player.SetGrenadeAttackPressed(false);
        _inputHandler.OnSpecialAttackInput += _player.HandleSpecialStarted;
        _inputHandler.OnSpecialAttackInputCanceled += _player.HandleSpecialCanceled;
        _inputHandler.OnInteractInput += _player.ExecuteInteraction;

        // PlaySceneView
        if(_playSceneView == null)
        {
            Debug.LogError("PlaySceneView가 할당되지 않았습니다!");
            return;
        }
        GameManager.Instance.CurrencyManager.OnCreditChanged += _playSceneView.UpdateGoldText;
        GameManager.Instance.CurrencyManager.OnChromeChanged += _playSceneView.UpdateChromeText;
        _player.playerModel.OnHpChanged += _playSceneView.UpdateHPBar;
    }

    private void Start()
    {
        if (_player == null)
        {
            Debug.LogError("플레이어가 할당되지 않았습니다!");
            return;
        }
        _player.Initialize();

        if (MapController == null)
        {
            Debug.LogError("맵 컨트롤러가 할당되지 않았습니다!");
            return;
        }
        _mapController.Initialize();

        if( _playSceneView == null)
        {
            Debug.LogError("PlaySceneView가 할당되지 않았습니다!");
            return;
        }
        _playSceneView.Initialize();
        if (_cameraMover == null)
        {
            Debug.LogError("카메라 무버가 할당되지 않았습니다!");
            return;
        }
        _cameraMover.Initialize(_player);

        GameManager.Instance.skillManager.SetPlayScene(this);

    }

    private void Update()
    {
    }
}
