using System;
using UnityEngine;

public class BulletData
{

}

/// <summary>
/// 총기류의 탄환을 관리하는 클래스
/// </summary>
public class Bullet : PoolableObject
{
    [Header("----- 데이터로 빼기 전 임시 -----")]
    [SerializeField] float _moveSpeed; // 탄환의 이동 속도
    [SerializeField] float _lifeTime;   // 탄환의 생존 시간

    [Header("----- 읽기 전용 -----")]
    [SerializeField] float _lifeTimer;              // 생존 시간 타이머
    [SerializeField] Vector3 _moveDir;

    public event Action OnLifeTimeExpired; // 생존 시간 만료 이벤트

    private void Update()
    {
        Move(_moveDir);

        _lifeTimer += Time.deltaTime;
        if(_lifeTimer >= _lifeTime)
        {
            OnLifeTimeExpired?.Invoke();
        _lifeTimer = 0f;

            GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        }
    }

    public void Initialize()
    {
        // 여기서 데이터 초기화 작업 수행 가능
        _lifeTimer = GameManager.Instance.PlayerStatManager.playerBulletMoveSpeed;
        _lifeTime = GameManager.Instance.PlayerStatManager.playerBulletLifeTime;
    }

    public void SetMoveDir(Vector3 moveDir)
    {
        _moveDir = moveDir.normalized;
    }

    public void Move(Vector3 moveDir)
    {
        transform.Translate(moveDir * _moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Constants.TAG_MONSTER))
        {
            EventBus.MonsterHit(WeaponType.Rifle, ATTACKTYPE.NORMAL, other.transform,transform);
            GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        }
        else if(other.CompareTag("Environment"))
        {
            GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        }
    }
}