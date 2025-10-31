using System;
using UnityEngine;

/// <summary>
/// Door를 Instantiate 해서 초기화하고 (선택적으로) InteractableManager에 등록 및 이벤트 연결까지 처리합니다.
/// </summary>
public class DoorSpawner : MonoBehaviour
{
    // MapController 등에서 구독할 수 있는 이벤트(문과 상호작용이 발생했음을 알림)
    public event Action<IInteractable> DoorInteracted;

    /// <summary>
    /// position 위치에 RoomInfo로 초기화된 Door 인스턴스를 생성하여 반환합니다.
    /// parent를 전달하면 Instantiate 시 부모를 지정합니다.
    /// </summary>
    public Door SpawnDoor(Transform position, RoomInfo info, Transform parent = null)
    {
        if (info == null)
        {
            Debug.LogError("SpawnDoor: info가 null 입니다.");
            return null;
        }

        // 우선 로컬 프리팹 필드가 비어있으면 ResourceManager에서 가져오기(둘 중 하나라도 할당되어야 함)
        GameObject prefab = GameManager.Instance?.ResourceManager?.DoorPrefab;
        if (prefab == null)
        {
            Debug.LogError("SpawnDoor: Door prefab이 할당되어 있지 않습니다.");
            return null;
        }

        // Instantiate 할 때 부모를 지정하면 SetParent 관련 타이밍 문제를 피할 수 있음
        GameObject go = parent != null
            ? Instantiate(prefab, position.position, position.rotation, parent)
            : Instantiate(prefab, position.position, position.rotation);

        if (go == null)
        {
            Debug.LogError("SpawnDoor: Instantiate 실패");
            return null;
        }

        Door door = go.GetComponent<Door>();
        if (door == null)
        {
            Debug.LogError("SpawnDoor: Door 컴포넌트가 없음. 생성한 객체를 파괴합니다.");
            Destroy(go);
            return null;
        }

        // 중요한 초기화: RoomInfo를 전달하여 내부 DoorInteractor가 올바르게 설정되도록 함
        door.Initialize(info);

        // Door이 IInteractable이고 내부에서 이벤트를 제공하는 구조라면 구독
        door.DoorInteracted += OnDoorInteracted;

        Debug.Log($"SpawnDoor: instantiated door '{go.name}' at {position.position} (sceneValid={go.scene.IsValid()}) for RoomType {info.RoomType}");
        return door;
    }

    void OnDoorInteracted(IInteractable interactable)
    {
        // 중계 이벤트
        DoorInteracted?.Invoke(interactable);
    }
}