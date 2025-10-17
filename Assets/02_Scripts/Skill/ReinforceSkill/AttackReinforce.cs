using System;
using UnityEngine;

/// <summary>
/// 비브르 모션 일반공격 강화
/// </summary>
public class Skill_One_Attack : ActiveTech
{

    public override event Action OnTechUsed;

    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        // 공격 적중 시 이벤트에 추가
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
        // 리스트 제거
        base.Deactivate(player);
        // 이벤트 해제
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

        transform.GetComponent<DebuffHandler>().ApplyDebuff(DebuffHandler.DebuffData.CreatePoison());
    }
    public Skill_One_Attack(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}

/// <summary>
/// 파이로 하트 일반공격 강화
/// </summary>
public class Skill_Two_Attack : ActiveTech
{

    public override event Action OnTechUsed;

    private int originalAttack = 100;
    private int originalDroneAttack;
    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        base.Activate(skillManager, player);
        #region Test
        
        // 스킬 효과 적용 (플레이어 일반 공격력에 접근하여 공격력 추가)
        player.playerAttack = (int)(originalAttack * ((float)_skillData.skillBaseValue_1 + (float)_skillData.skillLevelValue_1*_skillData.skillLevel));
        // player.playerAttack = (일반 공격 증가 식)
        // 공격 적중 시 이벤트에 추가
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            //Constants.DRONE_ATTACK = (일반 공격 증가 식);

        }
        Debug.Log(player.playerAttack);
    }
    public override void Deactivate(PlayerModel player)
    {
        // 리스트 제거
        base.Deactivate(player);
        // 공격력 복귀
        player.playerAttack = originalAttack;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            Constants.DRONE_ATTACK = originalDroneAttack;
        }

    }
    #endregion


    public Skill_Two_Attack(SkillData choosedSkill) : base(choosedSkill)
    {

    }
}


/// <summary>
/// GEAR 일반공격 강화, 보류
/// </summary>
public class Skill_Three_Attack : ActiveTech
{


    public override event Action OnTechUsed;

    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        // 공격 적중 시 이벤트에 추가
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
        // 리스트 제거
        base.Deactivate(player);
        // 이벤트 해제
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

    }
    public Skill_Three_Attack(SkillData skillData) : base(skillData)
    {
    }
}


/// <summary>
/// GridForge 일반공격 강화
/// 공격시 스택, 10스택시 공격에 스턴 부여
/// </summary>
public class Skill_Four_Attack : ActiveTech
{
    public int stack;


    public override event Action OnTechUsed;

    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        // 공격 시도 시 이벤트에 추가
        base.Activate(skillManager, player);
        player.AttackTry += AttackTry;
        player.AttackHit += HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.attackTry += AttackTry;
            }
        }
    }
    public override void Deactivate(PlayerModel player)
    {
        // 리스트 제거
        base.Deactivate(player);
        // 이벤트 해제
        player.AttackTry -= AttackTry;
        player.AttackHit -= HitEnemy;

        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.attackTry -= AttackTry;
            }
        }

    }

  

    public override void AttackTry()
    {
        stack++;
    }

    public override void HitEnemy(Transform transform)
    {
        if (stack >= 10)
        {
            // 스턴 적용
            transform.GetComponent<DebuffHandler>().ApplyDebuff(DebuffHandler.DebuffData.CreateStun(1f));
        }
    }


    public Skill_Four_Attack(SkillData skillData) : base(skillData)
    {
    }
}

/// <summary>
/// lux 제약 일반공격 강화
/// </summary>
public class Skill_Five_Attack : ActiveTech
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
        // 이벤트 해제
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
        base.HitEnemy(transform);
        //TODO 적 보스인지 일반인지 구분
        //TODO 약화 구현
        //transform.GetComponent<DebuffHandler>().ApplyDebuff(DebuffHandler.DebuffData.Create);
    }

    public Skill_Five_Attack(SkillData skillData) : base(skillData)
    {
    }
}



