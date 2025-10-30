using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "RoomDataSO", menuName = "ScriptableObjects/Map/RoomDataSO")]
public class RoomDataSO : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] RoomType _roomType;

    [Header("Prefab / Visuals")]
    [SerializeField] GameObject _roomPrefab;
    [SerializeField] string _roomName;
    [SerializeField] Sprite _previewIcon;
    [TextArea][SerializeField] string _description;

    public RoomType RoomType => _roomType;
    public GameObject RoomPrefab => _roomPrefab;
    public string RoomName => _roomName;
    public Sprite PreviewIcon => _previewIcon;
    public string Description => _description;

#if UNITY_EDITOR
    void OnValidate()
    {
        // Prefab이 Room 컴포넌트를 가지고 있는지 검사 (디자이너가 실수했을 때 빠르게 알림)
        if (_roomPrefab != null && !_roomPrefab.TryGetComponent<Room>(out _))
        {
            Debug.LogWarning($"RoomDataSO '{name}': assigned prefab '{_roomPrefab.name}' has no Room component.");
        }
        EditorUtility.SetDirty(this);
    }
#endif

    public bool IsValid(out string reason)
    {
        if (_roomPrefab == null) { reason = "RoomPrefab is null"; return false; }
        if (!_roomPrefab.TryGetComponent<Room>(out _)) { reason = "Prefab missing Room component"; return false; }
        reason = "";
        return true;
    }
}