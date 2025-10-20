using UnityEngine;

/// <summary>
/// 최소 책임: Resources 폴더에서 에셋을 로드해서 보관만 한다.
/// - 딕셔너리/매핑은 만들지 않음.
/// - 나중에 Addressables로 교체할 수 있게 구현부만 수정하면 됨.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    [Header("Editor Assignment (옵션)")]
    [SerializeField] GameObject _doorPrefab;

    [Header("Runtime Loaded (Resources 폴더에서 로드)")]
    public GameObject DoorPrefab { get; private set; }

    public PlayerWeaponSet[] PlayerWeaponSets { get; private set; }
    public RoomDataSO[] RoomDataSOs { get; private set; }

    // 호출: 게임 시작 시 한 번만
    public void Initialize()
    {
        DoorPrefab = _doorPrefab != null
            ? _doorPrefab
            : Resources.Load<GameObject>(Constants.RESOURCES_PATH_DOOR_PREFAB); // 경로 규칙 예시

        PlayerWeaponSets = Resources.LoadAll<PlayerWeaponSet>(Constants.RESOURCES_PATH_PLAYER_WEAPONSET);
        RoomDataSOs = Resources.LoadAll<RoomDataSO>(Constants.RESOURCES_PATH_ROOMDATASO);

        // (선택) 간단한 검증 로그
        if (DoorPrefab == null)
            Debug.LogWarning("ResourceManager: DoorPrefab이 비어있습니다.");
    }
}