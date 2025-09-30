using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 유니티 Input System의 PlayerInput을 활용해 입력을 받아 알리는 역할
/// </summary>
public class PlayerInputHandler : InputHandler
{
    public override event Action<Vector3> OnMoveInput;
    public override event Action<Vector2> OnCameraRotInput;
    public override event Action OnAttackInput;
    public override event Action OnInteractInput;

    Vector3 _moveInput;
    Vector2 _cameraRotInput;

    /// <summary>
    /// Player Input으로부터 이동 입력을 받는 함수
    /// </summary>
    /// <param name="inputValue"></param>
    void OnMove(InputValue inputValue)
    {
        Vector2 moveInput = inputValue.Get<Vector2>();
        _moveInput = new Vector3(moveInput.x, 0, moveInput.y);
    }

    void OnAttack(InputValue inputValue)
    {
        OnAttackInput?.Invoke();
        //Debug.Log("Attack Input Received");
    }

    /// <summary>
    /// Plyaer Input으로부터 마우스 입력을 받는 함수
    /// </summary>
    /// <param name="inputValue"></param>
    void OnLook(InputValue inputValue)
    {
        _cameraRotInput = inputValue.Get<Vector2>();
    }

    /// <summary>
    /// Player Input으로부터 상호작용 키를 입력받는 함수
    /// </summary>
    /// <param name="inputValue"></param>
    void OnInteract(InputValue inputValue)
    {
        OnInteractInput?.Invoke();
        Debug.Log("상호작용 키 눌림!");
    }

    private void FixedUpdate()
    {
        //Debug.Log(_moveInput);
        OnMoveInput?.Invoke(_moveInput);
        //Debug.Log(_cameraRotInput);
        OnCameraRotInput?.Invoke(_cameraRotInput);
    }
}