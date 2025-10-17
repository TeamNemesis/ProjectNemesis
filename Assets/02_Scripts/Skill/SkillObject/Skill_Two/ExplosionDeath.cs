using System.Collections;
using UnityEngine;

public class ExplosionDeath : AreaDamageBase, IPoolable
{


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Initialize()
    {
        _areaExtent = Constants.EXPLOSIONDEATH_EXTENT;
        CheckTarget();
        StartCoroutine(DestroyExplosionCoroutine(0.5f, this));
    }

    public override void ActiveSkill(Transform target)
    {
        target.GetComponent<IDamageable>().TakeDamage(Constants.EXPLOSIONDEATH_DAMAGE);
    }

    public IEnumerator DestroyExplosionCoroutine(float time, IPoolable gameObject)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Instance.ReleaseToPoolByInterface(gameObject);
    }

    public void ReleaseObject()
    {
    }
}
