using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Player _player;                               // 플레이어
    [SerializeField] PlayerInputHandler _inputHandler;             // 플레이어 입력 핸들러
    [SerializeField] MapController _mapController;                 // 맵 컨트롤러
    [SerializeField] PlaySceneView _playSceneView;                 // 플레이 씬 뷰

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

        // PlaySceneView
        GameManager.Instance.CurrencyManager.OnGoldChanged += _playSceneView.UpdateGoldText;
        GameManager.Instance.CurrencyManager.OnChromeChanged += _playSceneView.UpdateChromeText;
    }

    private void Start()
    {
        if (_player == null)
        {
            Debug.LogError("플레이어가 할당되지 않았습니다!");
            return;
        }
        _player.Initialize();
        Debug.Log("플레이어 할당");
        GameManager.Instance.skillManager.SetPlayer(_player);

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
    }

    private void Update()
    {
    }
}
