using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유니티 Resources를 활용해 게임의 리소스를 관리하는 매니저.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    //Dictionary<string, GameObject> _prefabCache = new();

    ///// <summary>
    ///// 지정 경로의 프리팹을 로드해 반환하는 함수
    ///// </summary>
    ///// <param name="path">Resources 폴더 안의 프리팹 경로</param>
    ///// <returns></returns>
    //public GameObject LoadPrefab(string path)
    //{
    //    // 이미 캐시에 로드한 프리팹 참조가 저장되어 있으면
    //    if (_prefabCache.ContainsKey(path) == true)
    //    {
    //        return _prefabCache[path];
    //    }

    //    GameObject prefab = Resources.Load<GameObject>(path);
    //    if (prefab == null)
    //    {
    //        Debug.LogError($"{path} 경로 프리팹이 없습니다.");
    //    }
    //    else
    //    {
    //        _prefabCache[path] = prefab;
    //    }
    //    return prefab;
    //}

    public void Initialze()
    {
        // Resources 폴더 안의 프리팹들을 미리 로드해 캐시에 저장할 수 있다.
        // 예를 들어, "Prefabs/Enemy" 경로에 있는 모든 적 프리팹을 로드할 수 있다.
        // 이 부분은 필요에 따라 구현할 수 있다.
    }

    Dictionary<Type, Dictionary<string, UnityEngine.Object>> _resourceCache = new();
    /// <summary>
    /// 리소스를 로드합니다. 이미 캐시에 있다면 캐시에서 반환합니다.
    /// </summary>
    /// <typeparam name="T">로드할 리소스의 타입</typeparam>
    /// <param name="path">Resources 폴더 내 리소스의 경로</param>
    /// <returns>로드된 리소스 객체</returns>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        // 타입별 캐시 확인
        if (_resourceCache.TryGetValue(typeof(T), out var cache) == false)
        {
            cache = new Dictionary<string, UnityEngine.Object>();
            _resourceCache[typeof(T)] = cache;
        }

        // 경로(폴더 경로 포함한 이름)에 따른 캐시 확인
        if (cache.ContainsKey(path) == true)
        {
            return cache[path] as T;
        }

        // 캐시에 없으면 Resources.Load로 로드
        T resource = Resources.Load<T>(path);
        if(resource == null)
        {
            Debug.LogError($"{path} 경로의 리소스를 Resources 폴더에서 찾을 수 없습니다.");
        }
        else
        {
            cache[path] = resource;
        }
        return resource;

        //    // 딕셔너리에서 먼저 Type으로 필터링하고 필터링했을 때 그 Type이 있으면
        //    // 내부 딕셔너리에서 또 path를 Key로써 필터링해서 해당하는 Value가 있는지 확인한다.
        //    // 만약 있다면 해당 Value를 resource변수로 out한다.
        //    if (_resourceCache.ContainsKey(typeof(T)) && _resourceCache[typeof(T)].TryGetValue(path, out var resource))
        //    {
        //        // resource가 T 타입인지 확인하고
        //        if (resource is T cachedResource)
        //        {
        //            // 맞다면 반환
        //            return cachedResource;
        //        }
        //        else
        //        {
        //            // 아니라면 로그띄우기
        //            Debug.LogWarning($"Resource at path '{path}' is not of type {typeof(T)}.");
        //            return null;
        //        }

        //        // 처음에 이렇게하면 될줄알았는데 위가 더 안전한 방법이라는듯?
        //        //return resource as T;
        //    }

        //    // 리소스가 캐시에 없으면 Resources.Load를 사용하여 로드
        //    T loadedResource = Resources.Load<T>(path);

        //    // 만약 로드한 리소스가 null이라면 
        //    if (loadedResource == null)
        //    {
        //        // 로드 실패 로그 출력
        //        Debug.Log("Resource를 로드할 수 없습니다.");
        //        // null 반환
        //        return null;
        //    }

        //    // 만약에 내가 로드한 타입이 딕셔너리에 없었다면
        //    // 즉 처음 로드하는 타입일 경우
        //    if (!_resourceCache.ContainsKey(typeof(T)))
        //    {
        //        // 해당 타입으로 딕셔너리를 하나 만든다.
        //        _resourceCache[typeof(T)] = new Dictionary<string, UnityEngine.Object>();
        //    }

        //    // 로드한 리소스를 캐시에 저장
        //    _resourceCache[typeof(T)][path] = loadedResource;

        //    // 그리고 방금 저장한 리소스를 반환
        //    return loadedResource;
        //
    }
}
