using System.Collections;
using UnityEngine;

public class PoisonSpread : AreaDamageBase
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


    public IEnumerator DestroyPoisonSpreadCoroutine()
    {
        yield return new WaitForSeconds(Constants.SKILL_ONE_HITPOISONSPREAD_TIME);
        ObjectPool.Instance.ReleaseToPoolByInterface(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Initialize()
    {
        transform.SetParent(GameManager.Instance.player.transform);
        CheckTarget();
        StartCoroutine(DestroyPoisonSpreadCoroutine());
    }

    public void ReleaseObject()
    {
    }
}
