using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
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

    [Header("----- 방 로딩 패널 -----")]
    [SerializeField] GameObject _roomLoadingPanel;
    [SerializeField] TextMeshProUGUI _roomLoadingText;

    [Header("----- 상호작용 패널 -----")]
    [SerializeField] GameObject _interactionPanel;
    [SerializeField] TextMeshProUGUI _interactionTitleText;
    [SerializeField] TextMeshProUGUI _interactionDescriptionText;

    [Header("----- 게임 승리 및 오버 패널 -----")]
    [SerializeField] GameObject _gameOverPanel;
    [SerializeField] GameObject _gameClearPanel;

    [Header("----- 게임 타이머 -----")]
    [SerializeField] TextMeshProUGUI _gameTimerText;

    [Header("----- 로컬라이즈 컴포넌트 -----")]

    // 테이블 이름은 Localization Table에서 사용한 이름(또는 Collection 이름)
    const string TABLE_COLLECTION_NAME = "InteractionView";

    LocalizedString _localizedTitle;
    LocalizedString _localizedDesc;

    Dictionary<int, string> _roomLoadingTextMap = new Dictionary<int, string>()
{
    { 0, "방의 종류에는 실험실, 일반 전투방, 암시장, 콜로세움, 보스 전투방이 있습니다." },
    { 1, "돌연변이 정수는 네뷸라사의 비밀 기술로, 2차 기업전쟁을 일으키게 된 원인입니다." },
    { 2, "기업은 5개가 존재하고, 기업마다 고유한 컨셉의 기술을 제공합니다." },
    { 3, "기본공격, 대쉬, 특수공격, 유탄을 직접적으로 강화하는 기술은 다른 기술 습득시 대체됩니다." },
    { 4, "주인공은 네뷸라사에 맹렬한 증오를 가지고 있습니다." }
};
    Coroutine _roomLoadingRoutine;

    public event Action<DoorInteractor> OnRoomLoadingComplete;

    public void Initialize()
    {
        EventBus.OnBossDead += ShowGameClearPanel;

        GameManager.Instance.CurrencyManager.GetCurrentCurrency();
        HideInteractionUI();

        // 처음 시작하면 유탄 슬라이더를 꽉 채우기
        UpdateGrenadeCoolTime(1.0f, 1.0f);
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

    #region 상호작용 UI 바인딩 (로컬라이즈된 문자열)
    // IInteractable에서 키를 받아 바인딩합니다.
    public void ShowInteractionUI(IInteractable interactable)
    {
        if (interactable == null) return;

        // 인터랙터로부터 로컬라이제이션 키를 받는다
        interactable.ReturnInteractionViewKey(out string titleKey, out string instructionKey);

        BindInteractionLocalizedStrings(titleKey, instructionKey);
        _interactionPanel?.SetActive(true);
    }

    public void HideInteractionUI()
    {
        _interactionPanel?.SetActive(false);
        UnbindInteractionLocalizedStrings();
    }

    // 바인딩: 기존 구독 해제 후 새 LocalizedString 생성 및 구독
    void BindInteractionLocalizedStrings(string titleKey, string descriptionKey)
    {
        UnbindInteractionLocalizedStrings();

        if (!string.IsNullOrEmpty(titleKey))
        {
            _localizedTitle = new LocalizedString { TableReference = TABLE_COLLECTION_NAME, TableEntryReference = titleKey };
            _localizedTitle.StringChanged += OnTitleChanged;
            _localizedTitle.RefreshString();
        }

        if (!string.IsNullOrEmpty(descriptionKey))
        {
            _localizedDesc = new LocalizedString { TableReference = TABLE_COLLECTION_NAME, TableEntryReference = descriptionKey };
            _localizedDesc.StringChanged += OnDescChanged;
            _localizedDesc.RefreshString();
        }
    }

    void UnbindInteractionLocalizedStrings()
    {
        if (_localizedTitle != null) _localizedTitle.StringChanged -= OnTitleChanged;
        if (_localizedDesc != null) _localizedDesc.StringChanged -= OnDescChanged;
        _localizedTitle = null;
        _localizedDesc = null;
    }

    void OnTitleChanged(string localized)
    {
        if (_interactionTitleText != null) _interactionTitleText.text = localized;
    }

    void OnDescChanged(string localized)
    {
        if (_interactionDescriptionText != null) _interactionDescriptionText.text = localized;
    }
    #endregion

    public void UpdateGrenadeCoolTime(float currentCooltime, float maxCooltime)
    {
        _grenadeCooltimeSlider.maxValue = maxCooltime;
        _grenadeCooltimeSlider.value = currentCooltime;
    }

    public void UpdateGrenadeCount(int currentCount, int maxCount)
    {
        _grenadeCountText.text = $"{currentCount} / {maxCount}";
    }

    public void RoomLoading(DoorInteractor doorInteractor)
    {
        _roomLoadingRoutine = StartCoroutine(RoomLoadingRoutine(doorInteractor));
    }

    /// <summary>
    /// 룸 로딩 시 실행할 코루틴
    /// 랜덤으로 로딩 텍스트를 보여줌
    /// </summary>
    /// <returns></returns>
    IEnumerator RoomLoadingRoutine(DoorInteractor doorInteractor)
    {
        EventBus.SetCanTimeRun(false);
        int rand = UnityEngine.Random.Range(0, _roomLoadingTextMap.Count);
        switch (rand)
        {
            case 0:
                _roomLoadingText.text = _roomLoadingTextMap[0];
                break;
            case 1:
                _roomLoadingText.text = _roomLoadingTextMap[1];
                break;
            case 2:
                _roomLoadingText.text = _roomLoadingTextMap[2];
                break;
            case 3:
                _roomLoadingText.text = _roomLoadingTextMap[3];
                break;
            case 4:
                _roomLoadingText.text = _roomLoadingTextMap[4];
                break;
            default:
                break;
        }
        _roomLoadingPanel.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        OnRoomLoadingComplete?.Invoke(doorInteractor);

        // 잠시 대기 후 패널 비활성화(플레이어 카메라 변화때문에)
        yield return new WaitForSeconds(0.5f);
        _roomLoadingPanel.SetActive(false);
        EventBus.SetCanTimeRun(true);
        _roomLoadingRoutine = null;
    }

    public void ShowGameOverPanel()
    {
        _gameOverPanel.SetActive(true);
    }

    public void ShowGameClearPanel()
    {
        _gameClearPanel.SetActive(true);
    }

    public void UpdateTimer(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        _gameTimerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
}