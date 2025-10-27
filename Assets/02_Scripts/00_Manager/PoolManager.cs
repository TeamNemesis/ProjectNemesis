using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 사용 가능한 오브젝트들을 모아두는 풀
    private Dictionary<string, List<GameObject>> availablePools = new Dictionary<string, List<GameObject>>();
    // 현재 사용중인 오브젝트들을 모아두는 풀
    private Dictionary<string, List<GameObject>> inUsePools = new Dictionary<string, List<GameObject>>();
    // 풀을 정리하는 Dictionary
    private Dictionary<string, GameObject> poolContainers = new Dictionary<string, GameObject>();

    #region 오브젝트풀 임시 생성
    /// <summary>
    /// 풀에서 오브젝트 가져오기
    /// </summary>
    public GameObject GetFromPool(PoolableObject poolable, Vector3 position, Quaternion rotation, Transform parentTransform = null, object data = null)
    {
        GameObject prefabObject = poolable.gameObject;
        if (!availablePools.ContainsKey(prefabObject.name))
        {
            availablePools.Add(prefabObject.name, new List<GameObject>());
            if (!inUsePools.ContainsKey(prefabObject.name))
            {
                inUsePools.Add(prefabObject.name, new List<GameObject>());
            }
        }

        if (availablePools[prefabObject.name].Count == 0)
        {
            Debug.LogWarning($"'{prefabObject.name}' 풀이 비어있습니다! 새로운 오브젝트를 생성합니다.");
            GameObject newObj = CreateNewObject(prefabObject, position, rotation, parentTransform);

            if (poolable is IInitializePoolable)
            {
                IInitializePoolable initializePoolable = newObj.GetComponent<PoolableObject>() as IInitializePoolable;
                initializePoolable.Initialize(data);
            }

            return newObj;
        }

        GameObject obj = availablePools[prefabObject.name][availablePools[prefabObject.name].Count - 1];
        availablePools[prefabObject.name].RemoveAt(availablePools[prefabObject.name].Count - 1);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        if (parentTransform != null)
        {
            obj.transform.SetParent(parentTransform);
        }

        if (poolable is IInitializePoolable)
        {
            IInitializePoolable initializePoolable = obj.GetComponent<PoolableObject>() as IInitializePoolable;
            initializePoolable.Initialize(data);
        }

        obj.SetActive(true);
        inUsePools[prefabObject.name].Add(obj);

        return obj;
    }

    /// <summary>
    /// 풀이 비어있을 때 새로운 오브젝트 생성
    /// </summary>
    private GameObject CreateNewObject(GameObject prefabObject, Vector3 position, Quaternion rotation, Transform parentTransform = null)
    {
        GameObject newObj;
        // 새 객체 생성
        if (parentTransform == null)
        {
            newObj = Instantiate(prefabObject);
        }
        else
        {
            newObj = Instantiate(prefabObject, parentTransform);
        }

        newObj.name = prefabObject.name;

        // 객체 저장을 위한 부모 저장용 오브젝트풀 자식 객체
        if (!poolContainers.ContainsKey(prefabObject.name))
        {
            GameObject container = new GameObject($"Pool_{prefabObject.name}");
            container.transform.SetParent(transform);
            poolContainers[prefabObject.name] = container;
        }

        newObj.transform.position = position;
        newObj.transform.rotation = rotation;

        newObj.SetActive(true);
        inUsePools[prefabObject.name].Add(newObj);
        Debug.Log($"'{prefabObject.name}' 풀의 새로운 오브젝트가 생성되었습니다.");
        return newObj;
    }

    /// <summary>
    /// 인터페이스를 이용한 오브젝트풀 해제
    /// </summary>
    /// <param name="poolable"></param>
    public void ReleaseToPoolByInterface(PoolableObject poolable)
    {
        GameObject obj = poolable.gameObject;

        if (poolable is IReleasePoolable)
        {
            IReleasePoolable releaseObject = obj.GetComponent<PoolableObject>() as IReleasePoolable;
            releaseObject.ReleaseObjectPool();
        }

        if (!inUsePools.ContainsKey(obj.name))
        {
            Debug.LogWarning($"'{obj.name}' 풀에 반환할 수 없습니다. 풀이 존재하지 않습니다.");
            Destroy(obj);
            return;
        }

        int index = inUsePools[obj.name].IndexOf(obj);
        if (index >= 0)
        {
            inUsePools[obj.name].RemoveAt(index);
            availablePools[obj.name].Add(obj);
            obj.SetActive(false);
            obj.transform.SetParent(poolContainers[obj.name].transform);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning($"오브젝트 '{obj.name}'은 이 풀에서 사용 중이 아닙니다.");
        }
    }

    #endregion
    /// <summary>
    /// 풀에 오브젝트 반환 (사용 완료)
    /// </summary>
    public void ReleaseToPool(GameObject obj)
    {
        if (!inUsePools.ContainsKey(obj.name))
        {
            Debug.LogWarning($"'{obj.name}' 풀에 반환할 수 없습니다. 풀이 존재하지 않습니다.");
            Destroy(obj);
            return;
        }

        int index = inUsePools[obj.name].IndexOf(obj);
        if (index >= 0)
        {
            inUsePools[obj.name].RemoveAt(index);
            availablePools[obj.name].Add(obj);
            obj.SetActive(false);
            obj.transform.SetParent(poolContainers[obj.name].transform);
        }
        else
        {
            Debug.LogWarning($"오브젝트 '{obj.name}'은 이 풀에서 사용 중이 아닙니다.");
        }
    }

    /// <summary>
    /// 특정 풀 상태 확인
    /// </summary>
    public (int available, int inUse, int total) GetPoolStatus(string poolName)
    {
        if (!availablePools.ContainsKey(poolName))
            return (0, 0, 0);

        int available = availablePools[poolName].Count;
        int inUse = inUsePools[poolName].Count;
        return (available, inUse, available + inUse);
    }



    /// <summary>
    /// 모든 풀 초기화
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in availablePools.Values)
        {
            foreach (GameObject obj in pool)
            {
                Destroy(obj);
            }
        }
        foreach (var pool in inUsePools.Values)
        {
            foreach (GameObject obj in pool)
            {
                Destroy(obj);
            }
        }
        availablePools.Clear();
        inUsePools.Clear();

        foreach (var container in poolContainers.Values)
        {
            Destroy(container);
        }
        poolContainers.Clear();

        Debug.Log("모든 풀이 초기화되었습니다.");
    }
}