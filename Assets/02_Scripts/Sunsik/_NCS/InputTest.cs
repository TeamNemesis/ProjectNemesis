using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    Vector3 _moveInput;

    public event Action<Vector3> OnMoveInput;

    void OnMove(InputValue inputValue)
    {
        Vector2 moveInput = inputValue.Get<Vector2>();
        _moveInput = new Vector3(moveInput.x, moveInput.y, 0);
    }

    private void FixedUpdate()
    {
        OnMoveInput?.Invoke(_moveInput);
    }
}
