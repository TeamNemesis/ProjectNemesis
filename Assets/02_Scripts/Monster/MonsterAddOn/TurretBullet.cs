using System.Collections;
using UnityEngine;

public class TurretBullet : PoolableObject
{
    private float speed = 7f;
    private float lifeTime; 
    private float damage;
    [SerializeField] private string targetTag;

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
        transform.Translate(Vector3.forward * speed * Time.deltaTime); 
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
