using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Door: 문 프리팹 루트. DoorInteractor가 실제 입력을 감지하고,
/// Door는 RoomInfo를 보관하여 문 위의 UI/프리뷰를 보여주고 상호작용을 중계한다.
/// 개선사항:
/// - Lock / Unlock API 추가
/// - 툴팁 표시(접근시) 추가 (선택적 컴포넌트)
/// - ResetForReuse에서 뷰/이벤트/상태 초기화 강화
/// - Initialize는 RoomInfo를 주입하고 DoorView에 프리뷰를 설정하지만, 실제 잠금 정책은 외부(StageController)와 협력
/// </summary>
[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] DoorInteractor _doorInteractor; // 반드시 프리팹에 연결되어 있어야 함
    [SerializeField] DoorView _doorView;             // 시각적 표현 담당(아이콘, 프리뷰 등)

    // RoomInfo는 런타임에 주입만 받고 프리팹에 저장되지 않도록 SerializeField 제거
    RoomInfo _roomInfo;
    public RoomInfo RoomInfo => _roomInfo; // 외부는 읽기 전용으로 접근

    bool _isLocked = true;
    public bool IsLocked => _isLocked;

    private void Awake()
    {
        // Defensive: null 체크
        if (_doorInteractor == null) Debug.LogWarning($"Door ({name}): DoorInteractor not assigned.");
        if (_doorView == null) Debug.LogWarning($"Door ({name}): DoorView not assigned.");
    }

    /// <summary>
    /// 초기화: 반드시 RoomInfo를 주입해야 함.
    /// 의존성 매니저는 주입하지 않으면 GameManager.Instance에서 가져오도록 하지 않음(테스트 편의성).
    /// </summary>
    public void Initialize(RoomInfo info)
    {
        if (info == null) throw new ArgumentNullException(nameof(info));

        // 기존 구독 제거(재초기화 안전)
        if (_doorInteractor != null)
        {
            _doorInteractor.OnInteracted -= OnDoorInteracted;
        }

        _roomInfo = info;

        // DoorInteractor에 RoomInfo 전달(Interactor가 툴팁/범위 감지용으로 쓴다면)
        if (_doorInteractor != null)
        {
            try
            {
                _doorInteractor.SetRoomInfo(info);
                // 기본적으로 상호작용 비활성화(잠금 상태는 StageController가 결정)
                _doorInteractor.ToggleInteraction(false);
                _doorInteractor.OnInteracted -= OnDoorInteracted;
                _doorInteractor.OnInteracted += OnDoorInteracted;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Door.Initialize: DoorInteractor usage threw: {ex}");
            }
        }

        // 뷰에 정보 전달 (프리뷰 텍스트/아이콘)
        if (_doorView != null)
        {
            try
            {
                _doorView.SetReward(info);
                // 보상 표시 여부는 ToggleReward로 제어: 기본은 숨김(선택 필요 시 StageController가 표시)
                _doorView.ToggleReward(false);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Door.Initialize: DoorView.SetReward threw: {ex}");
            }
        }
    }

    // 문 상호작용 중계
    void OnDoorInteracted(IInteractable interactable)
    {
        EventBus.RaiseInteracted(interactable);
    }

    #region Lock / Unlock / Reward-complete API

    /// <summary>
    /// 외부(StageController)가 보상 생성 전 호출하여 상호작용을 잠급니다.
    /// </summary>
    public void Lock()
    {
        _isLocked = true;
        // Interactor 비활성화
        if (_doorInteractor != null)
            _doorInteractor.ToggleInteraction(false);

        // 시각적 잠금 표시(애니메이션/아이콘) 가능 지점
    }

    /// <summary>
    /// 외부(StageController)가 모든 보상 선택/획득이 완료되었을 때 호출합니다.
    /// </summary>
    public void Unlock()
    {
        _isLocked = false;
        if (_doorInteractor != null)
            _doorInteractor.ToggleInteraction(true);

        // 잠금 해제 연출 가능 지점
    }

    /// <summary>
    /// 보상 선택 흐름이 끝났을 때 StageController가 호출할 수 있는 편의 메서드.
    /// 내부적으로 Unlock + 뷰 표시 동작을 수행합니다.
    /// </summary>
    public void NotifyRewardSelectionCompleted()
    {
        // 보상 뷰 보이기
        if (_doorView != null)
            _doorView.ToggleReward(true);

        Unlock();
    }

    #endregion

    #region Pooling / Reuse Safety

    /// <summary>
    /// 풀링용 초기화 해제: 재사용 시 반드시 호출해야 함.
    /// - 구독 해제, 뷰 초기화, 내부 RoomInfo null화
    /// </summary>
    public void ResetForReuse()
    {
        if (_doorInteractor != null)
        {
            _doorInteractor.OnInteracted -= OnDoorInteracted;
            try { _doorInteractor.ToggleInteraction(false); } catch { }
            try { _doorInteractor.SetRoomInfo(null); } catch { }
        }

        if (_doorView != null)
        {
            try { _doorView.ToggleReward(false); } catch { }
            // _doorView.ClearPreview() 같은 메서드가 있으면 호출
        }

        _roomInfo = null;
        _isLocked = true;
    }

    #endregion
}