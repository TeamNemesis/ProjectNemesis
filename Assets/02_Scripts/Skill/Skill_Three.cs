using System;
using UnityEngine;

/// <summary>
/// 추후 GEAR 기술 목록
/// </summary>
public class Skill_Three : SkillBase
{

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {

        }


        switch (choosedSkill.skillIdx)
        {
            // 중력자 무기
            case 30:
                ActiveTech skillAttack = new Skill_Three_Attck(choosedSkill);
                if (_skillManager.attachTech != null)
                {
                    _skillManager.attachTech.Deactivate(player);
                }
                skillAttack.Activate(_skillManager, player);
                break;

            // 소용돌이
            case 31:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 반동
            case 32:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 절대영역
            case 33:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;


                // 불운
            case 34:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 적색 편이
            case 35:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                 //TODO 적이 발사하는 발사체에 적 정보 필요
                break;

                // 중력 증폭
            case 36:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 사건의 지평선
            case 37:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
           
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;
        }

    }


}

public class Skill_Three_Attck : ActiveTech
{

    public override event Action OnTechUsed;
    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        base.Activate(skillManager, player);
        player.AttackHit += HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.Attack += HitEnemy;
            }
        }
    }
    public override void Deactivate(PlayerModel player)
    {
        base.Deactivate(player);
        player.AttackHit -= HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.Attack -= HitEnemy;
            }
        }
    }
    public override void HitEnemy(Transform transform)
    {
        Debug.Log("Use " + _skillData.skillIdx);

    }

    public Skill_Three_Attck(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}
