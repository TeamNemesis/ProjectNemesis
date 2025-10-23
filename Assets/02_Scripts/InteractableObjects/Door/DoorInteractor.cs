using System;
using UnityEngine;

/// <summary>
/// Door의 상호작용을 담당하는 클래스
/// </summary>
public class DoorInteractor : InteractableObject
{
    [SerializeField] Transform _guidePoint;
    [SerializeField] InteractableType _interactableType = InteractableType.Door;

    [SerializeField] RoomInfo _roomInfo;
    [SerializeField] bool _canInteract = true;

    public override Vector3 GuidePoint
    {
        get
        {
            if (_guidePoint == null)
            {
                // 방어적 처리: GuidePoint가 없으면 transform 위치 사용
                return transform.position;
            }
            return _guidePoint.position;
        }
    }

    public RoomInfo RoomInfo => _roomInfo;
    public override InteractableType InteractableType => _interactableType;

    // 파생에서 event를 다시 선언하지 마세요; base.OnInteracted를 그대로 사용합니다.
    public override event Action<IInteractable> OnInteracted;

    public override void StartInteract(Transform subject)
    {
        if (_roomInfo == null)
        {
            Debug.LogError($"DoorInteractor.StartInteract: RoomInfo가 null입니다. Door가 올바르게 Initialize 되었는지 확인하세요. (object={name})");
            return;
        }
        if (!_canInteract)
        {
            Debug.Log($"{name} 문은 현재 상호작용할 수 없습니다.");
            return;
        }

        Debug.Log("문과 상호작용 함: " + name);
        OnInteracted?.Invoke(this);
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
}