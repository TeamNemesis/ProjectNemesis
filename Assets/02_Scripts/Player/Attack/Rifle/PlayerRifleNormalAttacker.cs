using UnityEngine;

/// <summary>
/// 플레이어의 무기타입이 총일 때의 일반 공격을 담당하는 클래스
/// </summary>
public class PlayerRifleNormalAttacker : PlayerNormalAttacker
{
    [SerializeField] Transform _firePoint; // 총알 발사 위치
    [SerializeField] PoolableObject _bulletPrefab; // 총알 프리팹

    public void Initialize(Transform firePoint)
    {
        _firePoint = firePoint;
    }

    public override void Attack()
    {
        Debug.Log("라이플 일반공격 실행");
        GameObject obj = ObjectPool.Instance.GetFromPool(_bulletPrefab, _firePoint.position, transform.rotation);
    }
}