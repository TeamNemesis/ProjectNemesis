using UnityEngine;

public class TurretBullet : MonoBehaviour
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
    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }

    public void Initialize(string targetTag, float damage, float lifeTime)
    {
        SetTarget(targetTag);
        SetDamage(damage);
        SetLifeTime(lifeTime);
        this.owner = gameObject;
        StartLifeTime();
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
            Destroy(gameObject); // 충돌 시 총알 제거
        }
        else if (!other.CompareTag("Monster")) // 몬스터와 충돌하지 않도록 함
        {
            Destroy(gameObject); // 벽이나 다른 오브젝트와 충돌 시 총알 제거
        }
    }
}
