using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 풀링된 게임오브젝트를 관리하는 클래스
/// 풀에서 가져온 게임오브젝트를 풀로 반환하는 기능.
/// </summary>
public class Poolable : MonoBehaviour
{
    /// <summary>
    /// 자신 게임오브젝트가 생성된 풀
    /// </summary>
    Pool _pool;

    /// <summary>
    /// 풀을 설정하는 함수
    /// </summary>
    /// <param name="pool"></param>
    public void SetPool(Pool pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// 풀로 되돌리는 함수
    /// </summary>
    public void ReturnToPool()
    {
        // 풀이 있으면
        if (_pool != null)
        {
            // 풀에 게임오브젝트를 되돌려준다.
            _pool.Push(gameObject);
        }
        // 풀이 없으면
        else
        {
            // 파괴한다.
            Destroy(gameObject);
        }
    }
}
