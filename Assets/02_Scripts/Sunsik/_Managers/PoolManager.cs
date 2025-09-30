using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀들을 관리하는 클래스
/// </summary>
public class PoolManager : MonoBehaviour
{
    // 선택 사항
    const string _prefabPathFormat = "Prefabs/{0}";

    ResourceManager _resourceManager;
    Dictionary<string, Pool> _poolMap = new();

    public void Initialze(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    /// <summary>
    /// 프리팹 경로에 해당하는 풀을 반환하는 함수
    /// 아직 생성되지 않은 풀이면 새로 생성해 반환한다.
    /// '프리팹 경로'만 있으면 그 경로에 있는 '어떤 프리팹이던' 풀을 만들어줌.
    /// </summary>
    /// <param name="prefabPath">프리팹 경로</param>
    /// <param name="size">새 풀 생성 시 초기 풀 크기</param>
    /// <returns></returns>
    public Pool GetPool(string prefabPath, int size = 10)
    {
        // 프리팹 경로에 해당하는 풀이 없었으면
        if(_poolMap.ContainsKey(prefabPath) == false)
        {
            // 리소스 매니저에서 프리팹 경로에 해당하는 프리팹을 로드
            GameObject prefab = _resourceManager.LoadResource<GameObject>(string.Format(_prefabPathFormat, prefabPath));
            
            // 프리팹 로드에 실패한 경우
            if (prefab == null)
            {
                Debug.LogError($"프리팹 경로 {prefabPath}에 해당하는 프리팹이 없습니다.");
                return null;
            }

            // 프리팹 로드에 성공했으면 부모 게임오브젝트를 하나 생성해 이름을 정하고
            Transform parent = new GameObject($"Pool_{prefabPath}").transform;
            DontDestroyOnLoad(parent.gameObject);
            Pool pool = new Pool(prefab, parent, size);
            _poolMap[prefabPath] = pool;
        }
        return _poolMap[prefabPath];
    }

    /// <summary>
    /// 풀에서 게임오브젝트를 가져오는 함수
    /// </summary>
    /// <param name="prefabPath"></param>
    /// <returns></returns>
    public GameObject GetFromPool(string prefabPath)
    {
        // 프리팹 경로에 해당하는 풀을 가져온다.
        Pool pool = GetPool(prefabPath);
        if(pool == null)
        {
            Debug.LogError($"풀을 가져올 수 없습니다: {prefabPath}");
            return null;
        }
        return pool.Pop();
    }
}
