using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// Set Active 시
    /// </summary>
    public void Initialize();

    /// <summary>
    /// 인터페이스가 달려있는 게임오브젝트
    /// return gameObject;
    /// </summary>
    /// <returns></returns>
    public GameObject GetGameObject();

    /// <summary>
    /// Destroy 시
    /// </summary>
    public void ReleaseObject();
}
