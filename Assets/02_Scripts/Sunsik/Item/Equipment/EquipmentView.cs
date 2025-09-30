using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 장비 슬롯 뷰.
/// 툴팁, 우클릭 해제 기능
/// </summary>
public class EquipmentView : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    InventoryPresenter _presenter;

    Equipment _equipment;

    [SerializeField] EquipSlotType _slotType;
    [SerializeField] Image _iconImage;

    public void Initialize(InventoryPresenter presenter)
    {
        _presenter = presenter;
    }

    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;

        if (_equipment != null)
        {
            _iconImage.sprite = _equipment.ItemModel.Config.IconSprite;
            _iconImage.gameObject.SetActive(true);
        }
        else
        {
            _iconImage.gameObject.SetActive(false);
        }
    }

    public void Hide(bool isHidden)
    {
        _iconImage.enabled = !isHidden;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;

        _presenter.UnequipEquipment(_slotType);
        SetEquipment(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _presenter.SetTooltipPosition(transform.position);
        _presenter.ShowTooltip(_slotType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _presenter.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (_equipment == null) return;

        _presenter.BeginDrag(_equipment.ItemModel);

        Hide(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
            
        _presenter.Dragging(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _presenter.EndDrag();

        Hide(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _presenter.DropOnEquipmentView(_slotType);
    }
}
