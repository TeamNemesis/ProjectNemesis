using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템 설명 표시 뷰
/// 인벤토리에서 아이템 슬롯 위에 포인터를 올렸을 때 자동으로 표시되는 툴팁.
/// </summary>
public class ItemDescView : MonoBehaviour
{
    const string _priceTextFormat = "가격: {0} 골드";

    [Header("----- 컴폰너트 참조 -----")]
    [SerializeField] TextMeshProUGUI _nameText;  // 아이템 이름 텍스트
    [SerializeField] TextMeshProUGUI _descText; // 아이템 개수 텍스트
    [SerializeField] TextMeshProUGUI _priceText; // 아이템 개수 텍스트

    public void SetItemModel(ItemModel model)
    {
        _nameText.text = model.Config.ItemName;       // 아이템 이름 텍스트 설정
        _descText.text = model.Config.Description; // 아이템 설명 텍스트 설정
        _priceText.text = string.Format(_priceTextFormat,model.Config.Price); // 아이템 가격 텍스트 설정
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
