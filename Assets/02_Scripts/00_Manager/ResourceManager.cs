using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 프리팹 리소스를 관리하는 매니저 클래스
/// 게임 시작시 필요한 리소스를 미리 로드하여 관리
/// </summary>
public class ResourceManager : MonoBehaviour
{
    [Header("----- 플레이어 -----")]
    [SerializeField] PlayerWeaponSet[] _playerWeaponSets; // 플레이어 컴포넌트 세트들

    [Header("----- 맵 생성 -----")]
    [SerializeField] GameObject[] _roomPrefabs; // 생성할 방 프리팹들
    [SerializeField] GameObject _doorPrefab;    // 생성할 문 프리팹 (만약 방 타입에 따라 다르면 딕셔너리로 변경)

    Dictionary<WeaponType, PlayerWeaponSet> _playerWeaponSetMap = new Dictionary<WeaponType, PlayerWeaponSet>();
    Dictionary<RoomType, GameObject> _roomPrefabMap = new();

    public Dictionary<WeaponType, PlayerWeaponSet> PlayerWeaponSetMap => _playerWeaponSetMap;
    public Dictionary<RoomType, GameObject> RoomPrefabMap => _roomPrefabMap;
    public GameObject DoorPrefab => _doorPrefab;


    public void Initialize()
    {
        _playerWeaponSets = Resources.LoadAll<PlayerWeaponSet>(Constants.RESOURCES_PATH_PLAYER_WEAPONSET);
        _roomPrefabs = Resources.LoadAll<GameObject>(Constants.RESOURCES_PATH_ROOM_PREFABS);
        _doorPrefab = Resources.Load<GameObject>(Constants.RESOURCES_PATH_DOOR_PREFAB);

        InitializePlayerWeaponSetMap();
        InitializeRoomPrefabDict();
    }

    void InitializePlayerWeaponSetMap()
    {
        _playerWeaponSetMap.Clear();

        foreach (var set in _playerWeaponSets)
        {
            if (!_playerWeaponSetMap.ContainsKey(set.WeaponType))
            {
                _playerWeaponSetMap.Add(set.WeaponType, set);
            }
            else
            {
                Debug.LogWarning($"이미 {set.WeaponType} 타입의 무기 세트가 존재합니다.");
            }
        }
    }

    void InitializeRoomPrefabDict()
    {
        _roomPrefabMap.Clear();
        foreach (GameObject prefab in _roomPrefabs)
        {
            Room room = prefab.GetComponent<Room>();
            if(room == null)
            {
                Debug.LogError($"RoomPrefab {prefab.name}에 Room 컴포넌트가 없습니다.");
                return;
            }
            if (room != null && !_roomPrefabMap.ContainsKey(room.RoomType))
            {
                _roomPrefabMap.Add(room.RoomType, prefab);
            }
            else
            {
                Debug.LogWarning($"이미 {room.RoomType} 타입의 방 프리팹이 존재합니다.");
            }
        }
    }
}