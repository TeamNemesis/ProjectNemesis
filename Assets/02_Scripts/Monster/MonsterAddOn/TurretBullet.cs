using Unity.VisualScripting;
using UnityEngine;

public class TurretBullet : PoolableObject
{
    private float speed = 7f; // 총알 속도
    private float lifeTime = 5f; // 총알 수명
    private float damage;
    private string targetTag;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetTarget(string targetTag)
    {
        this.targetTag = targetTag;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // 총알 이동
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage); // 플레이어에게 피해 주기
                Debug.Log("Player Hit! Damage: " + damage);
            }
            ObjectPool.Instance.ReleaseToPool(gameObject);
        }
        else if (!other.CompareTag(targetTag)) // 타겟과 충돌하지 않도록 함
        {
            ObjectPool.Instance.ReleaseToPool(gameObject);
        }
    }
}
