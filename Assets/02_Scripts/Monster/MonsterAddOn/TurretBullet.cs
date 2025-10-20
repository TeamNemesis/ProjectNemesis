using System.Collections;
using UnityEngine;

public class TurretBullet : PoolableObject
{
    private float speed = 7f; // 총알 속도
    private float lifeTime; // 총알 수명
    private float damage;
    [SerializeField]private string targetTag;

    private GameObject owner;
    private Coroutine lifeTimeCoroutine;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetTarget(string targetTag)
    {
        this.targetTag = targetTag;
    }
    public void SetLifeTime(float duration)
    {
        this.lifeTime = duration;
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


    private void StartLifeTime()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
        }

        lifeTimeCoroutine = StartCoroutine(LifeTimeCoroutine());
    }

    private IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPool.Instance.ReleaseToPool(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner)
        {
            return;
        }
        if (other.CompareTag(targetTag))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log("Player Hit! Damage: " + damage);
            }

            // 코루틴 정리 후 반환
            if (lifeTimeCoroutine != null)
            {
                StopCoroutine(lifeTimeCoroutine);
                lifeTimeCoroutine = null;
            }
            ObjectPool.Instance.ReleaseToPool(gameObject);
        }
        else
        {
            if (lifeTimeCoroutine != null)
            {
                StopCoroutine(lifeTimeCoroutine);
                lifeTimeCoroutine = null;
            }
            ObjectPool.Instance.ReleaseToPool(gameObject);
        }
    }
}
