using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 주인공 캐릭터 UI 담당
/// </summary>
public class HeroStatusView : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] RectTransform _canvasRt;           // 캔버스 렉트 트랜스폼
    [SerializeField] Image _hpBar;
    [SerializeField] Image _mpBar;
    [SerializeField] TextMeshProUGUI _heroNameText;
    [SerializeField] RectTransform _interactionGuideRt; // 상호작용 가이드 렉트 트랜스폼

    public void SetHpBar(float currentHp, float maxHp)
    {
        _hpBar.fillAmount = currentHp / maxHp;
    }

    public void SetMpBar(float currentMp, float maxMp)
    {
        _mpBar.fillAmount = currentMp / maxMp;
    }

    public void SetHeroNameText(string heroName)
    {
        _heroNameText.text = heroName;
    }

    /// <summary>
    /// 상호작용 가이드 UI를 설정하는 함수
    /// </summary>
    /// <param name="isActive">가이드 표시 여부</param>
    /// <param name="worldPos">가이드 표시할 월드 좌표</param>
    public void SetInetractionGuide(bool isActive, Vector3 worldPos)
    {
        // 상호작용 가이드 온오프
        _interactionGuideRt.gameObject.SetActive(isActive);
        if (isActive == false) return;

        // 월드 좌표 -> 스크린 좌표 변환
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        // 스크린 좌표 -> 캔버스 로컬 좌표
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRt, screenPoint, null, out Vector2 localPoint);

        // 캔버스 로컬 좌표를 상호작용 가이드에 적용
        _interactionGuideRt.anchoredPosition = localPoint;

    }
}
