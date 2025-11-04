using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

/// <summary>
/// 플레이씬의 UI를 관리하는 뷰 클래스입니다.
/// </summary>
public class PlaySceneView : MonoBehaviour
{
    [Header("----- UI 컴포넌트 참조 -----")]
    [SerializeField] Slider _hpBarSlider;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _chromeText;
    [SerializeField] Slider _grenadeCooltimeSlider;
    [SerializeField] TextMeshProUGUI _grenadeCountText;

    [SerializeField] GameObject _interactionPanel;
    [SerializeField] TextMeshProUGUI _interactionTitleText;
    [SerializeField] TextMeshProUGUI _interactionInstructionText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;


		public void Initialize()
    {
        GameManager.Instance.CurrencyManager.GetCurrentCurrency();
        HideInteractionUI();
    }

    public void UpdateHPBar(int currentHp, int maxHp)
    {
        _hpBarSlider.maxValue = maxHp;
        _hpBarSlider.value = currentHp;
        _hpText.text = $"{currentHp} / {maxHp}";
    }

    public void UpdateGoldText(int currentGold)
    {
        _goldText.text = $"{currentGold}";
    }

    public void UpdateChromeText(int currentChrome)
    {
        _chromeText.text = $"{currentChrome}";
    }

    public void ShowInteractionUI(IInteractable interactable)
    {
        UpdateInteractionText(interactable);
        _interactionPanel.SetActive(true);
    }

    public void HideInteractionUI()
    {
        _interactionPanel.SetActive(false);
    }

    void UpdateInteractionText(IInteractable interactable)
    {
        interactable.GetInteractionMessage(out string title, out string instruction);
        _interactionTitleText.text = title;
				localizeStringEvent.StringReference.SetReference("New Table", instruction);
    }

    public void UpdateGrenadeCoolTime(float currentCooltime, float maxCooltime)
    {
        _grenadeCooltimeSlider.maxValue = maxCooltime;
        _grenadeCooltimeSlider.value = currentCooltime;
    }

    public void UpdateGrenadeCount(int currentCount, int maxCount)
    {
        _grenadeCountText.text = $"{currentCount} / {maxCount}";
    }
}