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
    public override void Deactivate(PlayerModel player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
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
    /// <summary>
    /// 파이로 하트 일반공격 강화 스킬 발동시 발행할 이벤트, 스킬 인덱스, 강화 계수
    /// </summary>
    public static event Action<int, float> ActiveSkillEvent;

    /// <summary>
    /// 파이로 하트 일반공격 강화 해제 이벤트, 스킬 인덱스
    /// </summary>
    public static event Action<int> DeactiveSkillEvent;
    public override event Action OnTechUsed;

    private float originalAttack = 100f;
    private float originalDroneAttack = 2f;
    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        base.Activate(skillManager, player);
        #region Test

        // 스킬 효과 적용 (플레이어 일반 공격력에 접근하여 공격력 추가)
        float plusAttack = _skillData.skillBaseValue_1 + _skillData.skillLevelValue_1 * _skillData.skillLevel;
        // player.playerAttack = (일반 공격 증가 식)
        ActiveSkillEvent?.Invoke(_skillData.skillIdx, plusAttack);
        // 드론 공격력 증가
        skillManager.playerStatManager.PlusDroneAttack(plusAttack);

    }
    public override void Deactivate(PlayerModel player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);

        // 공격력 복귀
        //player.playerAttack = originalAttack;
        DeactiveSkillEvent?.Invoke(_skillData.skillIdx);

        // 드론 공격력 복귀
        GameManager.Instance.playerStatManager.SetDroneAttack(Constants.DRONE_ATTACK);


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
    public override void Deactivate(PlayerModel player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
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

    /// <summary>
    /// 공격 시도시 실행할 액션
    /// </summary>
    private Action _AttackTry;

    public override event Action OnTechUsed;

    public override void Activate(SkillManager skillManager, PlayerModel player)
    {
        // 공격 시도 시 이벤트에 추가
        base.Activate(skillManager, player);
        _AttackTry = () => ActiveTry(player);
        player.AttackTry += _AttackTry;
        player.AttackHit += HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.attackTry += _AttackTry;
            }
        }
    }
    public override void Deactivate(PlayerModel player, bool isSameSkill)
    {
        // 리스트 제거s
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
        player.AttackTry -= _AttackTry;
        player.AttackHit -= HitEnemy;

        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.attackTry -= _AttackTry;
            }
        }

    }



    public override void ActiveTry(PlayerModel player)
    {
        stack++;
        // 최대 스택 체한
        if (stack > 10)
        {
            stack = 10;
        }
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
        stack = 0;
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

    public override void Deactivate(PlayerModel player, bool isSameSkill)
    {
        base.Deactivate(player, isSameSkill);
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



