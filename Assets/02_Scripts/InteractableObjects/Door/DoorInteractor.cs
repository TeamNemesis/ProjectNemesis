using System;
using UnityEngine;

/// <summary>
/// Door의 상호작용을 담당하는 클래스
/// </summary>
public class DoorInteractor : InteractableObject
{
    [SerializeField] string _instruction;

    [SerializeField] InteractableType _interactableType = InteractableType.Door;

    [SerializeField] RoomInfo _roomInfo;
    [SerializeField] bool _canInteract = true;

    // 재진입 방지 (선택)
    bool _isInteracting = false;

    public RoomInfo RoomInfo => _roomInfo;
    public override InteractableType InteractableType => _interactableType;

    // 파생에서 event를 다시 선언하지 마세요; base.OnInteracted를 그대로 사용합니다.
    public override event Action<IInteractable> OnInteracted;

    /// <summary>
    /// 상호작용 시도. 성공하면 true 반환(상호작용 시작), 실패하면 false 반환.
    /// </summary>
    public override bool TryInteract(Transform subject)
    {
        // 이미 상호작용 중이면 거부
        if (_isInteracting)
        {
            Debug.Log($"{name}은(는) 이미 상호작용 중입니다.");
            return false;
        }

        if (_roomInfo == null)
        {
            Debug.LogError($"DoorInteractor.TryInteract: RoomInfo가 null입니다. Door가 올바르게 Initialize 되었는지 확인하세요. (object={name})");
            return false;
        }

        if (!_canInteract)
        {
            Debug.Log($"{name} 문은 현재 상호작용할 수 없습니다.");
            return false;
        }

        // 필요하면 subject 체크
        if (subject == null)
        {
            Debug.LogWarning("TryInteract 호출 시 subject가 null입니다.");
        }

        // 상호작용 시작
        _isInteracting = true;

        Debug.Log("문과 상호작용 함: " + name);
        OnInteracted?.Invoke(this);

        // 즉시 처리형이면 바로 끝내고 _isInteracting 리셋 (혹은 연출/비동기라면 End 시 리셋)
        _isInteracting = false;
        return true;
    }

    // RoomInfo를 세팅하는 안전한 공개 메서드
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        if (roomInfo == null)
        {
            Debug.LogError("DoorInteractor.SetRoomInfo 호출 시 roomInfo가 null입니다! 호출자 스택을 확인하세요.");
            return;
        }
        _roomInfo = roomInfo;
        Debug.Log($"{name}: {_roomInfo.RoomType} 방 정보로 문 설정됨");
    }

    public void ToggleInteraction(bool canInteract)
    {
        _canInteract = canInteract;
    }

    public override void GetInteractionMessage(out string title, out string instruction)
    {
        title = _roomInfo != null ? _roomInfo.GetRoomTitle() : "알 수 없는 방";
        instruction = _roomInfo != null ? _roomInfo.GetRoomDescription() : "설정되지 않은 방입니다.";
    }
}