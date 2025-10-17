using System;
using UnityEngine;

/// <summary>
/// Door(프리팹 루트) — 실제 상호작용은 DoorInteractor가 담당.
/// Door는 DoorInteractor를 통해 RoomInfo를 전달하고, 등록/구독 관리를 보조합니다.
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] DoorInteractor _doorInteractor;    // 문 상호작용 컴포넌트 (IInteractable 구현체)
    RoomInfo _roomInfo;

    public RoomInfo RoomInfo => _roomInfo;

    // DoorSpawner / MapController가 구독할 수 있는 중계 이벤트
    public event Action<IInteractable> DoorInteracted;

    /// <summary>
    /// 생성 직후 호출해서 초기화합니다.
    /// - info는 null이 아니어야 합니다.
    /// - 내부 DoorInteractor가 반드시 프리팹에 연결되어 있어야 합니다.
    /// </summary>
    public void Initialize(RoomInfo info)
    {
        // 안전 검사
        if (info == null)
        {
            Debug.LogError("Door.Initialize: info가 null입니다.");
            return;
        }

        if (_doorInteractor == null)
        {
            Debug.LogError("Door.Initialize: _doorInteractor가 할당되어 있지 않습니다. Door 프리팹을 확인하세요.");
            return;
        }

        // RoomInfo 설정
        _roomInfo = info;

        // DoorInteractor에 정보 전달
        _doorInteractor.SetRoomInfo(_roomInfo);

        // DoorInteractor의 이벤트를 중계하도록 구독
        // (중복 구독을 방지하려면 먼저 해제 시도)
        _doorInteractor.OnInteracted -= OnDoorInteracted;
        _doorInteractor.OnInteracted += OnDoorInteracted;

        // InteractableManager에 등록(InteractionController는 여기서 등록된 IInteractable을 받음)
        var mgr = GameManager.Instance?.InteractableManager;
        if (mgr != null)
        {
            mgr.Register(_doorInteractor); // 반드시 IInteractable 구현체를 등록
            Debug.Log($"Door.Initialize: Registered door interactor to InteractableManager. Current room: {_roomInfo.RoomType}");
        }
        else
        {
            Debug.LogWarning("Door.Initialize: InteractableManager가 null입니다. 등록되지 않았습니다.");
        }

        // 시각 / 보상 등 초기화
        SetRoomInfo(_roomInfo);
    }

    void OnDoorInteracted(IInteractable interactable)
    {
        // 중계 이벤트: InteractionController / MapController가 DoorInteracted 이벤트를 구독할 수 있음
        DoorInteracted?.Invoke(interactable);
    }

    void SetRoomInfo(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        SetReward();
    }

    void SetReward()
    {
        // TODO: roomInfo에 따라 아이콘/라벨/보상을 설정
    }

    // 안전하게 구독 해제하고 레지스트리에서 제거
    void OnDisable()
    {
        if (_doorInteractor != null)
        {
            _doorInteractor.OnInteracted -= OnDoorInteracted;

            var mgr = GameManager.Instance?.InteractableManager;
            if (mgr != null)
            {
                mgr.Unregister(_doorInteractor);
            }
        }

        // 중계 이벤트도 초기화 (옵션)
        DoorInteracted = null;
    }
}