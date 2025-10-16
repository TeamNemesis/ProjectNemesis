using UnityEngine;

public class PoisonSpread : AreaDamageBase
{

    public void Start()
    {
        CheckTarget();
    }

    /// <summary>
    /// 스킬에 맞는 효과 발동
    /// </summary>
    /// <param name="target"></param>
    public override void ActiveSkill(Transform target)
    {
        // 독 적용
        target.GetComponent<DebuffHandler>().ApplyDebuff(DebuffHandler.DebuffData.CreatePoison());
    }

}
