using TMPro;
using UnityEngine;
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

    public void Initialize()
    {
        GameManager.Instance.CurrencyManager.GetCurrentCurrency();
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
}