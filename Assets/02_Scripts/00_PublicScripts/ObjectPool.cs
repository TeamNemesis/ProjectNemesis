using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // 사용 가능한 오브젝트들을 모아두는 풀
    private Dictionary<string, List<GameObject>> availablePools = new Dictionary<string, List<GameObject>>();
    // 현재 사용중인 오브젝트들을 모아두는 풀
    private Dictionary<string, List<GameObject>> inUsePools = new Dictionary<string, List<GameObject>>();
    // 풀을 정리하는 Dictionary
    private Dictionary<string, GameObject> poolContainers = new Dictionary<string, GameObject>();

    // ****(중요!!!) 프리펩으로 만들어둔 오브젝트들만 사용 가능 ****
    // 오브젝트 풀은 싱글톤으로 사용.
    private static ObjectPool instance;

    [SerializeField] private List<GameObject> poolObjects = new List<GameObject>();

    private void Awake()
    {
        InitializeAllPools();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject poolObject = new GameObject("ObjectPool");
                instance = poolObject.AddComponent<ObjectPool>();
            }
            return instance;
        }
    }

    /// <summary>
    /// 인스펙터에서 설정한 모든 풀 자동 초기화
    /// </summary>
    private void InitializeAllPools()
    {
        foreach (GameObject prefab in poolObjects)
        {
            if (prefab != null)
            {
                CreatePool(prefab, 5);
            }
        }
    }

    /// <summary>
    /// 풀 생성 (특정 프리팹으로 미리 생성)
    /// </summary>
    public void CreatePool(GameObject initialPrefab, int initialSize = 5)
    {
        if (!availablePools.ContainsKey(initialPrefab.name))
        {
            availablePools[initialPrefab.name] = new List<GameObject>();
            inUsePools[initialPrefab.name] = new List<GameObject>();

            // 정리용 게임 오브젝트
            GameObject container = new GameObject($"Pool_{initialPrefab.name}");
            container.transform.SetParent(transform);
            poolContainers[initialPrefab.name] = container;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject prefab = Instantiate(initialPrefab, container.transform);
                prefab.SetActive(false);
                availablePools[initialPrefab.name].Add(prefab);
            }

            Debug.Log($"'{initialPrefab.name}' 풀 생성. (초기 개수: {initialSize})");
        }
    }

    /// <summary>
    /// 풀에 오브젝트 추가 (이미 생성된 오브젝트를 풀에 등록)
    /// </summary>
    public void AddToPool(GameObject obj)
    {
        if (!availablePools.ContainsKey(obj.name))
        {
            availablePools[obj.name] = new List<GameObject>();
            inUsePools[obj.name] = new List<GameObject>();

            if (!poolContainers.ContainsKey(obj.name))
            {
                GameObject container = new GameObject($"Pool_{obj.name}");
                container.transform.SetParent(transform);
                poolContainers[obj.name] = container;
            }
        }

        // 사용 중이던 오브젝트면 사용 완료 상태로 변경
        if (inUsePools[obj.name].Contains(obj))
        {
            inUsePools[obj.name].Remove(obj);
        }

        obj.SetActive(false);
        obj.transform.SetParent(poolContainers[obj.name].transform);
        availablePools[obj.name].Add(obj);
    }

    /// <summary>
    /// 풀에서 오브젝트 가져오기
    /// </summary>
    public GameObject GetFromPool(string poolName)
    {
        if (!availablePools.ContainsKey(poolName) || availablePools[poolName].Count == 0)
        {
            Debug.LogWarning($"'{poolName}' 풀이 비어있습니다! 새로운 오브젝트를 생성합니다.");
            return CreateNewObject(poolName);
        }

        GameObject obj = availablePools[poolName][availablePools[poolName].Count - 1];
        availablePools[poolName].RemoveAt(availablePools[poolName].Count - 1);

        obj.SetActive(true);
        inUsePools[poolName].Add(obj);

        return obj;
    }

    /// <summary>
    /// 풀이 비어있을 때 새로운 오브젝트 생성
    /// </summary>
    private GameObject CreateNewObject(string poolName)
    {
        // 기존 풀에서 프리팹 정보를 찾아 새로운 오브젝트 생성
        foreach (GameObject prefab in poolObjects)
        {
            if (prefab.name == poolName)
            {
                GameObject newObj = Instantiate(prefab, poolContainers[poolName].transform);
                newObj.SetActive(true);
                inUsePools[poolName].Add(newObj);
                Debug.Log($"'{poolName}' 풀의 새로운 오브젝트가 생성되었습니다.");
                return newObj;
            }
        }
        return null;
    }

    #region 오브젝트풀 임시 생성
    /// <summary>
    /// 풀에서 오브젝트 가져오기
    /// </summary>
    public T GetFromPool<T>(IPoolable poolable, Vector3 position, Transform parentTransform = null)
    {
        GameObject prefabObject = poolable.GetGameObject();
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
            return CreateNewObject(prefabObject, position,parentTransform).GetComponent<T>();
        }

        GameObject obj = availablePools[prefabObject.name][availablePools[prefabObject.name].Count - 1];
        availablePools[prefabObject.name].RemoveAt(availablePools[prefabObject.name].Count - 1);
        obj.transform.position = position;
        if(parentTransform!=null)
        {
            obj.transform.SetParent(parentTransform);
        }
        obj.SetActive(true);
        obj.GetComponent<IPoolable>().Initialize();
        inUsePools[prefabObject.name].Add(obj);

        return obj.GetComponent<T>();
    }

    /// <summary>
    /// 풀이 비어있을 때 새로운 오브젝트 생성
    /// </summary>
    private GameObject CreateNewObject(GameObject prefabObject, Vector3 position, Transform parentTransform = null)
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

        newObj.SetActive(true);
        newObj.GetComponent<IPoolable>().Initialize();
        inUsePools[prefabObject.name].Add(newObj);
        Debug.Log($"'{prefabObject.name}' 풀의 새로운 오브젝트가 생성되었습니다.");
        return newObj;
    }

    /// <summary>
    /// 인터페이스를 이용한 오브젝트풀 해제
    /// </summary>
    /// <param name="poolable"></param>
    public void ReleaseToPoolByInterface(IPoolable poolable)
    {
        poolable.ReleaseObject();

        GameObject obj = poolable.GetGameObject();
        if (!inUsePools.ContainsKey(obj.name))
        {
            Debug.LogWarning($"'{obj.name}' 풀에 반환할 수 없습니다. 풀이 존재하지 않습니다.");
            Destroy(obj);
            return;
        }

        int index = inUsePools[obj.name].IndexOf(obj);
        Debug.Log(index);
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
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
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
    /// 모든 풀 상태 출력
    /// </summary>
    public void PrintAllPoolStatus()
    {
        Debug.Log("===== 전체 풀 상태 =====");
        foreach (var pool in availablePools.Keys)
        {
            var (available, inUse, total) = GetPoolStatus(pool);
            Debug.Log($"[{pool}] 사용가능: {available} | 사용중: {inUse} | 총: {total}");
        }
    }

    /// <summary>
    /// 풀이 존재하는지 확인
    /// </summary>
    public bool HasPool(string poolName)
    {
        return availablePools.ContainsKey(poolName);
    }

    /// <summary>
    /// 특정 풀 초기화
    /// </summary>
    public void ClearPool(string poolName)
    {
        if (availablePools.ContainsKey(poolName))
        {
            foreach (GameObject obj in availablePools[poolName])
            {
                Destroy(obj);
            }
            foreach (GameObject obj in inUsePools[poolName])
            {
                Destroy(obj);
            }
            availablePools.Remove(poolName);
            inUsePools.Remove(poolName);

            if (poolContainers.ContainsKey(poolName))
            {
                Destroy(poolContainers[poolName]);
                poolContainers.Remove(poolName);
            }

            Debug.Log($"'{poolName}' 풀이 초기화되었습니다.");
        }
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