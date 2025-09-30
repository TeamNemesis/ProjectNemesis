using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [SerializeField] Hero _hero;
    [SerializeField] Enemy _enemy;
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] DialogueSystem _dialogueSystem;

    bool _isPlaying = false;

    //[SerializeField] CameraController _cameraController;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputHandler.OnMoveInput += OnMoveInput;
        _inputHandler.OnAttackInput += OnAttackInput;
        _inputHandler.OnInteractInput += OnInteractInput;
        _dialogueSystem.OnToggled += OnInteractToggled;

        //if (_cameraController == null)
        //{
        //    Debug.LogError("VCameraController is not assigned in PlayScene.");
        //    return;
        //}
        //_inputHandler.OnCameraRotInput += _cameraController.Rotate;

        _hero.Initialize();
        _dialogueSystem.Initialize();

        _enemy.Initialize(_hero.transform);
    }

    void OnMoveInput(Vector3 inputVector)
    {
        if (_isPlaying)
        {
            _hero.Stop();
            return;
        }

        // 카메라의 전방 방향
        Vector3 camForward = Camera.main.transform.forward;
        // 카메라의 우측 방향
        Vector3 camRight = Camera.main.transform.right;

        // y축 방향 제거
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // 실제 이동 방향(카메라 기준으로 변환된 방향)

        Vector3 direction = camForward * inputVector.z + camRight * inputVector.x;

        _hero.Move(direction);
    }

    void OnAttackInput()
    {
        if (_isPlaying) return;
        _hero.OnAttackInput();
    }

    void OnInteractInput()
    {
        _hero.ExecuteInteraction();
        Debug.Log("상호작용 입력받음");
    }

    void OnInteractToggled(bool isPlaying)
    {
        _isPlaying = isPlaying;
        Cursor.lockState = isPlaying ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
