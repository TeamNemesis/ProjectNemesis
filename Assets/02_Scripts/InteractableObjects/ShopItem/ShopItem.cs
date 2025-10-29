using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ShopItem : MonoBehaviour, IInteractable
{
    [Header("Shop")]
    [SerializeField] int _price = 100;
    [Tooltip("구매 시 실행할 보상 컴포넌트 (같은 GameObject 또는 자식에 붙여두세요)")]
    [SerializeField] RewardInteractableObject _rewardInteractable;
    [SerializeField] Vector3 _guideOffset = Vector3.up * 1.2f;
    [SerializeField] bool _destroyOnPurchase = true;

    // 외부 콜백
    public event Action<IInteractable> OnInteracted; // IInteractable 인터페이스 요구

    CurrencyManager _currencyManager;

    // IInteractable 구현
    public Vector3 GuidePoint => transform.position + _guideOffset;
    public InteractableType InteractableType => InteractableType.ShopItem;

    public void Initialize()
    {
        _currencyManager = GameManager.Instance.CurrencyManager;
    }

    // 프롬프트/UI용 메시지
    public void GetInteractionMessage(out string title, out string instruction)
    {
        title = $"{_rewardInteractable.RewardTitle} ({_price}G)";
        instruction = "E: 구매";
    }

    // Detector/Interact 시스템이 호출하는 진입점
    // 반환값: true => 상호작용(구매) 시작/성공, false => 거부(잔액부족 등)
    public bool TryInteract(Transform subject)
    {
        // 기본 유효성 검사
        if (_rewardInteractable == null)
        {
            Debug.LogWarning($"ShopItem({name}): 연결된 RewardInteractableObject가 없습니다.");
            return false;
        }

        if (_currencyManager == null)
        {
            Debug.LogWarning("_currencyManager is null.");
            return false;
        }

        if (!_currencyManager.TrySpendCredit(_price))
        {
            EventBus.FailBuy(_price);
            return false;
        }

        // 결제 시도
        bool spent = _currencyManager.TrySpendCredit(_price);
        if (!spent)
        {
            EventBus.FailBuy(_price);
            return false;
        }

        // 결제 성공: 보상 컴포넌트의 TryInteract 호출
        bool started = _rewardInteractable.TryInteract(subject);
        if (!started)
        {
            Debug.Log("ShopItem: RewardInteractableObject의 TryInteract가 실패했습니다.");
            return false;
        }

        // 성공
        OnInteracted?.Invoke(this);

        return true;
    }
}