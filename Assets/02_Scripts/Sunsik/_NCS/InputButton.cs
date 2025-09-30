using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ButtonType
{
    Left,
    Right,
    Up,
    Down
}

public class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ButtonType _buttonType;
    [SerializeField] bool _isHolding = false;

    public event Action <ButtonType> OnButtonClicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHolding = false;
    }

    void Update()
    {
        if (_isHolding)
        {
            OnButtonClicked?.Invoke(_buttonType);
        }
    }
}   