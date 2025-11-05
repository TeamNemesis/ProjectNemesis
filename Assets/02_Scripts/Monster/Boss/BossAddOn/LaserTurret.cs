using System.Collections;
using UnityEngine;

public class LaserTurret : MonsterBase
{
    private float lifeTime = 12f;
    private Coroutine lifeTimeCoroutine;

    public override void Initialize(object data = null)
    {
        base.Initialize(data);
        StartLifeTime();
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 코루틴 정리
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }
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

        // 풀로 반환
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);

        lifeTimeCoroutine = null; // 코루틴 참조 정리
    }
}