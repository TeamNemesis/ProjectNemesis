using System;
using UnityEngine;

/// <summary>
/// 총기류의 탄환을 관리하는 클래스
/// </summary>
public class Bullet : PoolableObject
{
    [SerializeField] float _moveSpeed = 50f; // 탄환의 이동 속도
    [SerializeField] float _lifeTime = 2f;   // 탄환의 생존 시간
    [SerializeField] Poolable _bulletPoolable;

    public event Action OnLifeTimeExpired; // 생존 시간 만료 이벤트

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Constants.TAG_MONSTER))
        {
            _bulletPoolable.ReturnToPool();
        }
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public GameObject GetGameObject()
    {
        throw new NotImplementedException();
    }

    public void ReleaseObject()
    {
        
    }
}