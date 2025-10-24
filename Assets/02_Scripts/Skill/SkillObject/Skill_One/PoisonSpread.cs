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
        GameManager.Instance.PoolManager.ReleaseToPoolByInterface(this);
    }


    public void Initialize()
    {
        CheckTarget();
        StartCoroutine(DestroyPoisonSpreadCoroutine());
    }

  
}
