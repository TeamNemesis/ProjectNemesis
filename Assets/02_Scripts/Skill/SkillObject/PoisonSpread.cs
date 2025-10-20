using System.Collections;
using UnityEngine;

public class PoisonSpread : AreaDamageBase,IPoolable
{


    /// <summary>
    /// 스킬에 맞는 효과 발동
    /// </summary>
    /// <param name="target"></param>
    public override void ActiveSkill(Transform target)
    {
        // 독 적용
        target.GetComponent<DebuffHandler>().ApplyDebuff(DebuffHandler.DebuffData.CreatePoison());

    }


    public IEnumerator DestroyPoisonSpreadCoroutine(float time, IPoolable gameObject)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Instance.ReleaseToPoolByInterface(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Initialize()
    {
        transform.SetParent(GameManager.Instance.player.transform);
        CheckTarget();
        StartCoroutine(DestroyPoisonSpreadCoroutine(0.5f, this));
    }

    public void ReleaseObject()
    {
    }
}
