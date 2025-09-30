using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 아이템 뷰
/// 인벤토리에서 각 아이템 슬롯의 기능을 담당.
/// 아이템 아이콘 표시, 드래그앤드롭, 포인터 오버, 클릭 기능.
/// </summary>
public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    InventoryPresenter _presenter;
    int _slotIndex;  // 아이템 뷰의 슬롯 인덱스

    ItemModel _model;                   // 아이템 모델

    Image _iconImage;  // 아이템 아이콘 이미지

    public ItemModel Model => _model;  // 아이템 모델 프로퍼티

    void Awake()
    {
        _iconImage = gameObject.FindChild<Image>("Icon", true);  // 아이콘 이미지 컴포넌트 찾기
    }

    public void Initialize(InventoryPresenter presenter, int slotIndex)
    {
        _presenter = presenter;
        _slotIndex = slotIndex;

        _iconImage = gameObject.FindChild<Image>("Icon", true);  // 아이콘 이미지 컴포넌트 찾기
    }

    /// <summary>
    /// 아이템 모델을 넣어주고 아이콘 이미지를 설정
    /// </summary>
    /// <param name="model"></param>
    public void SetItemModel(ItemModel model)
    {
        _model = model;  // 아이템 모델 설정

        if (_model != null)
        {
            _iconImage.sprite = _model.Config.IconSprite;  // 아이템 아이콘 이미지 설정
            _iconImage.gameObject.SetActive(true);         // 아이콘 이미지 활성화
        }
        else
        {
            _iconImage.gameObject.SetActive(false);        // 아이템 모델이 없으면 아이콘 이미지 비활성화
        }
    }

    /// <summary>
    /// 드래그 시 빈자리 표현을 위한 아이콘 이미지 숨김/표시 함수
    /// </summary>
    /// <param name="isHidden">숨김 여부</param>
    public void Hide(bool isHidden)
    {
        _iconImage.enabled = !isHidden;  // 아이콘 이미지의 활성화 상태를 숨김 여부에 따라 설정
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _presenter.BeginDrag(_model);

        Hide(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _presenter.Dragging(eventData.position);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _presenter.DropOnItemView(_slotIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _presenter.EndDrag();

        Hide(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _presenter.SetTooltipPosition(transform.position);
        _presenter.ShowTooltip(_slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _presenter.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에만 실행
        if (eventData.button != PointerEventData.InputButton.Right) return;

        Debug.Log($"클릭! {gameObject.name}", gameObject);
        _presenter.UseItem(_slotIndex);
    }
}
