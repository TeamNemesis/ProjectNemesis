using System;
using UnityEngine;


/// <summary>
/// 비브르 모션 특수 공격 강화 - 피의 갈증
/// </summary>
public class Skill_One_SPAttack : ActiveTech
{

    public override event Action OnTechUsed;

    private Action _AttackTry;
    /// <summary>   
    /// 공격 한 번 당 한 번만 적용하도록 하기 위한 필드값
    /// </summary>
    public bool isHit;

    public override void Activate(SkillManager skillManager, Player player)
    {
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);
        _AttackTry = () => ActiveTry(player);
        //player.SPAttackTry += _AttackTry;
        //player.SPAttackHit += HitEnemy;

    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
       // player.SPAttackTry -= _AttackTry;
       // player.SPAttackHit -= HitEnemy;


    }

    /// <summary>
    /// 공격 시도 시 변수 초기화
    /// </summary>
    public override void ActiveTry(Player player)
    {
        isHit = false;
    }

    public override void HitEnemy(Transform transform)
    {
        // 이미 효과가 발동했었다면 return;
        if (isHit) return;

        isHit = true;
        GameManager.Instance.player.playerModel.Heal(Constants.SKILL_ONE_SPATTACKHEAL);
    }


    public Skill_One_SPAttack(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}


/// <summary>
/// 파이로 하트 특수 공격 강화 - 비밀무기
/// </summary>
public class Skill_Two_SPAttack : ActiveTech
{

    public override event Action OnTechUsed;



    /// <summary> 
    /// 공격 한 번 당 한 번만 적용하도록 하기 위한 필드값
    /// </summary>
    public bool isHit;

    public void Activate(SkillManager skillManager, Player player, int skillLevel)
    {
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);

        // 스킬 효과 적용 (플레이어 특수공격력에 접근하여 공격력 추가)
        // player.playerSPAttack = (특수 공격 증가 식)

    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제


    }





    public Skill_Two_SPAttack(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}

