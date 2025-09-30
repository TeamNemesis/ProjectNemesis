using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임오브젝트 풀
/// 게임오브젝트들을 미리 생성해 뒀다가 필요할 때 건네주고,
/// 사용이 다 했으면 다시 돌려받는 클래스.
/// </summary>
public class Pool
{
    Stack<GameObject> _pool;     // 복제본 게임오브젝트 스택
    GameObject _prefab;         // 풀링할 원본 프리팹
    Transform _parent;          // 풀링 게임오브젝트들의 부모 트랜스폼

    /// <summary>
    /// Pool 생성자
    /// </summary>
    /// <param name="prefab">풀링할 프리팹</param>
    /// <param name="parent">풀의 부모 트랜스폼</param>
    /// <param name="initialSize">초기 풀 크기</param>
    public Pool(GameObject prefab, Transform parent, int initialSize)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new Stack<GameObject>(initialSize);

        // 초기 크기만큼 풀에 게임오브젝트를 추가
        for (int i = 0; i < initialSize; i++)
        {
            CreatePoolObj();
        }
    }

    /// <summary>
    /// 풀에 새 게임오브젝트를 추가하는 함수
    /// </summary>
    void CreatePoolObj()
    {
        // 원본 프리팹을 복제하여 새 게임오브젝트를 생성
        GameObject go = Object.Instantiate(_prefab);

        // 새 게임오브젝트의 부모를 풀의 부모로 설정
        go.transform.SetParent(_parent);

        // 새 게임오브젝트를 비활성화 상태로 설정
        go.SetActive(false);

        // 새 게임오브젝트에서 Poolable 컴포넌트를 가져온다.
        Poolable poolable = go.GetComponent<Poolable>();
        // Poolable 컴포넌트가 없으면
        if (poolable == null)
        {
            // Poolable 컴포넌트 새로 추가
            poolable = go.AddComponent<Poolable>();
        }

        // 풀을 여기로 설정
        poolable.SetPool(this);

        // 새 게임오브젝트를 스택에 추가
        _pool.Push(go);
    }

    /// <summary>
    /// 풀에서 게임오브젝트를 가져오는 함수
    /// 풀이 비었으면 새로 생성해 반환한다.
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        // 풀에 게임 오브젝트가 하나라도 있으면
        if (_pool.Count > 0)
        {
            // 풀에서 게임오브젝트를 꺼내고
            GameObject go = _pool.Pop();
            // 활성화 상태로 변경
            go.SetActive(true);
            // 꺼낸 게임오브젝트를 반환
            return go;
        }

        // 풀에 게임오브젝트가 없으면 새로 생성해서
        GameObject newGo = Object.Instantiate(_prefab);

        // 게임오브젝트에서 Poolable 컴포넌트를 가져온다.
        Poolable poolable = newGo.GetComponent<Poolable>();

        // Poolable 컴포넌트가 없으면
        if (poolable == null)
        {
            // Poolable 컴포넌트 새로 추가
            poolable = newGo.AddComponent<Poolable>();
        }

        // 풀을 여기로 설정
        poolable.SetPool(this);

        // 생성한 게임오브젝트를 반환
        return newGo;
    }

    /// <summary>
    /// 게임오브젝트를 풀에 반환하는 함수
    /// </summary>
    /// <param name="go"></param>
    public void Push(GameObject go)
    {
        // 반환할 게임오브젝트의 부모를 풀의 부모로 설정
        go.transform.SetParent(_parent);
        // 게임오브젝트를 비활성화 상태로 설정
        go.SetActive(false);
        // 게임오브젝트를 풀에 추가
        _pool.Push(go);
    }
}
