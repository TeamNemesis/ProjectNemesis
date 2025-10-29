using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최소 책임: Resources 폴더에서 에셋을 로드해서 보관만 한다.
/// - 단일 에셋은 LoadResource<T>(path)
/// - 여러 에셋(배열)은 LoadAllResources<T>(path)
/// - 나중에 Addressables로 교체 가능하도록 분리
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public GameObject DoorPrefab { get; private set; }

    public PlayerWeaponSet[] PlayerWeaponSets { get; private set; }
    public RoomDataSO[] RoomDataSOs { get; private set; }
    public RewardDataSO[] RewardDataSOs { get; private set; }

    // 호출: 게임 시작 시 한 번만
    public void Initialize()
    {
        // 단일 에셋 로드
        DoorPrefab = LoadResource<GameObject>(Constants.RESOURCES_PATH_DOOR_PREFAB);

        // 배열(폴더) 로드: Resources.LoadAll 사용
        PlayerWeaponSets = LoadAllResources<PlayerWeaponSet>(Constants.RESOURCES_PATH_PLAYER_WEAPONSET);
        RoomDataSOs = LoadAllResources<RoomDataSO>(Constants.RESOURCES_PATH_ROOMDATASO);
        RewardDataSOs = LoadAllResources<RewardDataSO>(Constants.RESOURCES_PATH_REWARD_DATA_SO);

        // (선택) 간단한 검증 로그
        if (DoorPrefab == null)
            Debug.LogWarning("ResourceManager: DoorPrefab이 비어있습니다.");
    }

    readonly Dictionary<Type, Dictionary<string, UnityEngine.Object>> _singleCache = new();
    readonly Dictionary<Type, Dictionary<string, UnityEngine.Object[]>> _allCache = new();

    /// <summary>
    /// 리소스를 로드합니다. 이미 캐시에 있다면 캐시에서 반환합니다.
    /// 단일 에셋용: Resources.Load<T>(path)
    /// </summary>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path)) return null;
        var type = typeof(T);

        // 1) 해당 타입의 하위 캐시가 있고, 그 하위 캐시에 path가 있으면 바로 반환
        if (_singleCache.TryGetValue(type, out var cache) && cache.TryGetValue(path, out var cachedObj))
        {
            if (cachedObj is T tObj) return tObj;
            // 캐시에 잘못된 타입이 들어있다면 경고하고 제거(또는 덮어쓰기)
            Debug.LogWarning($"ResourceManager: cached object at '{path}' is not of expected type {type.Name}. Reloading.");
            cache.Remove(path);
        }

        // 2) 캐시가 없거나 엔트리가 없으면 Resources.Load로 로드
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogWarning($"ResourceManager: Resources.Load<{type.Name}> failed for path: {path}");
            return null;
        }

        // 3) 하위 캐시가 아직 없으면 생성하고 저장
        if (!_singleCache.TryGetValue(type, out cache))
        {
            cache = new Dictionary<string, UnityEngine.Object>();
            _singleCache[type] = cache;
        }

        // 4) 캐시에 넣고 반환
        cache[path] = resource;
        return resource;
    }

    /// <summary>
    /// 여러 에셋을 배열로 로드합니다. Resources.LoadAll<T>(path) 사용.
    /// 캐시 key는 타입 + "::ALL::" + path 로 관리.
    /// </summary>
    public T[] LoadAllResources<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path)) return Array.Empty<T>();
        var type = typeof(T);
        if (!_allCache.TryGetValue(type, out var cache))
        {
            cache = new Dictionary<string, UnityEngine.Object[]>();
            _allCache[type] = cache;
        }
        if (cache.TryGetValue(path, out var cached)) return cached as T[] ?? Array.Empty<T>();

        var results = Resources.LoadAll<T>(path) ?? Array.Empty<T>();
        cache[path] = results;
        if (results.Length == 0) Debug.LogWarning($"ResourceManager: Resources.LoadAll<{type.Name}> returned empty for path: {path}");
        return results;
    }
}