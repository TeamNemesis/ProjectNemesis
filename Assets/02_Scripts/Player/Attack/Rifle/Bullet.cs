using System;
using UnityEngine;

/// <summary>
/// 총기류의 탄환을 관리하는 클래스
/// </summary>
public class Bullet : PoolableObject, IInitializePoolable
{
    [Header("----- 데이터로 빼기 전 임시 -----")]
    [SerializeField] float _moveSpeed = 20f; // 탄환의 이동 속도
    [SerializeField] float _lifeTime = 2f;   // 탄환의 생존 시간
    [SerializeField] float _damage = 10f;    // 탄환의 데미지 값

    [SerializeField]
    float _lifeTimer;              // 생존 시간 타이머

    public event Action OnLifeTimeExpired; // 생존 시간 만료 이벤트

    private void Update()
    {
        Move();

        _lifeTimer += Time.deltaTime;
        if(_lifeTimer >= _lifeTime)
        {
            OnLifeTimeExpired?.Invoke();
        _lifeTimer = 0f;

            ObjectPool.Instance.ReleaseToPool(gameObject);
        }
    }

    public void Initialize()
    {
        // 여기서 데이터 초기화 작업 수행 가능
        _lifeTimer = 0f;
        _lifeTime = 2f;
        Debug.Log("bulletInitialize");  
    }

    void Move()
    {
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Constants.TAG_MONSTER))
        {
            ObjectPool.Instance.ReleaseToPool(gameObject);
            MonsterBase monsterBase= other.GetComponent<MonsterBase>();
            monsterBase.TakeDamage(_damage); // 예시로 10의 데미지를 입힘
        }
        else if(other.CompareTag("Environment"))
        {
            ObjectPool.Instance.ReleaseToPool(gameObject);
        }
    }
}