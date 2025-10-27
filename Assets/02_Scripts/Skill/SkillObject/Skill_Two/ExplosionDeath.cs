using System.Collections;
using UnityEngine;

public class ExplosionDeath : AreaDamageBase
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
        Debug.LogWarning("Æø»ç ¹ßµ¿");
    }

    public override void ActiveSkill(Transform target)
    {
        target.GetComponent<IDamageable>().TakeDamage(Constants.EXPLOSIONDEATH_DAMAGE, null);
    }

    public IEnumerator DestroyExplosionCoroutine(float time, PoolableObject gameObject)
    {
        yield return new WaitForSeconds(time);
        Debug.LogWarning("Æø»ç Á¦°Å");
        GameManager.Instance.PoolManager.ReleaseToPoolByInterface(gameObject);
    }

}
