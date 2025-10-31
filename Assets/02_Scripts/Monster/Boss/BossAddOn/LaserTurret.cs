using System.Collections;
using UnityEngine;

public class LaserTurret : MonsterBase
{
    private float lifeTime = 12f;

    private Coroutine lifeTimeCoroutine;

    private void Start()
    {
        StartCoroutine(LifeTimeCoroutine());
    }

    public override void Initialize()
    {
        StartLifeTime();
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
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }
}
