using System;
using UnityEngine;


[RequireComponent(typeof(SkillChoose))]
public class TechItem : MonoBehaviour
{ 
    public void OnTriggerEnter(Collider other)
    {
        GetSkill();
    }

  
    #region 스킬 습득

    private TechSelectPackType _TechCompany;
    /// <summary>
    /// 스킬 습득
    /// </summary>
    public void GetSkill()
    {
        SkillChoose skillchoose = GetComponent<SkillChoose>();
        _TechCompany = TechSelectPackType.Company4;
        // 회사 지정
        skillchoose.SetSkillCompany(_TechCompany);
        skillchoose.SetBtn();
    }
    #endregion

    #region 스킬 업그레이드
   

    public void SkillUpgrade()
    {
        SkillChoose skillchoose = gameObject.GetComponent<SkillChoose>();
        skillchoose.SetUpgradeBtn();
    }
    #endregion
}
