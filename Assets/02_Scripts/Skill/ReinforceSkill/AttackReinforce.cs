using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 비브르 모션 일반공격 강화
/// </summary>
public class Skill_One_Attack : ActiveTech
{
    public override void Activate(SkillManager skillManager, Player player)
    {
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);
        //player.AttackHit += HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.Attack += HitEnemy;
            }
        }
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
        //player.AttackHit -= HitEnemy;
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

    private float plusDamage;
    private int originalDroneAttack = 2;
    public override void Activate(SkillManager skillManager, Player player)
    {
        base.Activate(skillManager, player);
        #region Test

        // 스킬 효과 적용 (플레이어 일반 공격력에 접근하여 공격력 추가)
        plusDamage = _skillData.skillBaseValue_1 + _skillData.skillLevelValue_1 * _skillData.skillLevel;
        GameManager.Instance.PlayerStatManager.AddPlayerAttackDamage(plusDamage);
        Debug.Log(GameManager.Instance.PlayerStatManager.playerAttackDamage);

        ActiveSkillEvent?.Invoke(_skillData.skillIdx, plusDamage);
        // 공격 적중 시 이벤트에 추가
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            Constants.DRONE_ATTACK = (originalDroneAttack * (1 + plusDamage));
        }
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);

        // 공격력 복귀
        GameManager.Instance.PlayerStatManager.AddPlayerAttackDamage(-plusDamage);
        Debug.Log(GameManager.Instance.PlayerStatManager.playerAttackDamage);
        DeactiveSkillEvent?.Invoke(_skillData.skillIdx);
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
    private Action<Transform> knockbackAction;
    private Dictionary<Drone, Action<Transform>> droneHandlers = new();


    public override void Activate(SkillManager skillManager, Player player)
    {
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);
        knockbackAction = (enemy) => KnockBackEnemy(enemy, player.transform);
        //player.AttackHit += HitEnemy; 
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                DroneEventConnect(drone);
            }
        }
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
        //player.AttackHit -= HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        foreach (Drone drone in drones)
        {
            if (droneHandlers.TryGetValue(drone, out var handler))
            {
                drone.Attack -= handler;
            }
        }

        droneHandlers.Clear();
    }

    public void KnockBackEnemy(Transform enemyTransform, Transform attackerTransform)
    {
        Vector3 direction = enemyTransform.position - attackerTransform.position;
        direction.Normalize();
        MonsterBase monster = enemyTransform.GetComponent<MonsterBase>();
        if (monster != null)
        {
            monster.KnockBackEnemy(direction, 0f, 6f);
        }

    }

    /// <summary>
    /// 드론에 이벤트 등록
    /// </summary>
    /// <param name="drone"></param>
    public void DroneEventConnect(Drone drone)
    {
        Action<Transform> handler = (enemy) => KnockBackEnemy(enemy, drone.transform);
        drone.Attack += handler;
        droneHandlers[drone] = handler;
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
    private Action<Transform> _DroneAttack;

    public override void Activate(SkillManager skillManager, Player player)
    {

        // 공격 시도 시 이벤트에 추가
        base.Activate(skillManager, player);
        _AttackTry = () => ActiveTry(player);
        _DroneAttack = (transform) => ActiveTry(player);
        player.OnNormalAttackStarted += _AttackTry;
        //player.AttackHit += HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                DroneEventConnect(drone);
            }
        }
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        Debug.Log("40번 스킬 해제");
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
        player.OnNormalAttackStarted -= _AttackTry;
        //player.AttackHit -= HitEnemy;

        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {

                drone.Attack -= _DroneAttack;
            }
        }

    }

    public void DroneEventConnect(Drone drone)
    {
        // ActiveTry 연결
        drone.Attack += _DroneAttack;
    }



    public override void ActiveTry(Player player)
    {
        stack++;
        // 최대 스택 체한
        if (stack > 10)
        {
            stack = 10;
        }
        Debug.Log("stack : " + stack);
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
    public override void Activate(SkillManager skillManager, Player player)
    {
        base.Activate(skillManager, player);
        //TODO 일반공격 적중시 이벤트에 연결
        //player.playerModel.AttackHit += HitEnemy;
        Drone[] drones = player.transform.GetComponentsInChildren<Drone>();
        if (drones.Length > 0)
        {
            foreach (Drone drone in drones)
            {
                drone.Attack += HitEnemy;
            }
        }
    }

    public override void Deactivate(Player player, bool isSameSkill)
    {
        base.Deactivate(player, isSameSkill);
        //TODO 이벤트 해제
        //player.playerModel.AttackHit -= HitEnemy;

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
        MonsterBase monster =  transform.GetComponent<MonsterBase>();

        if(monster.GetMonsterSize() == MonsterSize.BIG)
        {
            monster.GetDebuffHandler().ApplyDebuff(DebuffHandler.DebuffData.CreateWeaken(5f, 10f));
        }
        else
        {
            monster.GetDebuffHandler().ApplyDebuff(DebuffHandler.DebuffData.CreateWeaken(5f, 30f));
        }
    }

    public Skill_Five_Attack(SkillData skillData) : base(skillData)
    {

    }
}



