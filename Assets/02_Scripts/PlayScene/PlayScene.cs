using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Player _player;               // 플레이어
    [SerializeField] PlayerInput _playerInput;     // 플레이어 입력 컴포넌트

    private void Awake()
    {
        _playerInput.OnMoveInput += _player.Move;
        _playerInput.OnDashInput += _player.Dash;
        _playerInput.OnNomralAttackInput += _player.NormalAttack;
        // 여기에 모바일용 유탄공격 시작 이벤트를 추가하여 UI를 생성하는 메서드를 연결할 수 있습니다.
        _playerInput.OnGrenadeAttackInputEnded += _player.GrenadeAttack;
        _playerInput.OnSpecialAttackInput += _player.SpecialAttack;
        _playerInput.OnInteractInput += _player.ExecuteInteraction;
    }

    private void Start()
    {
        _player.Initialize();
    }
}
