using System;
using UnityEngine;

/// <summary>
/// 비브르 강화 대쉬 강화 (약육강식)
/// </summary>
public class Skill_One_Dash : ActiveTech
{
    /// <summary>
    /// 대쉬 시작 독 프리팹
    /// </summary>
    [SerializeField]
    private PoisonDash _poisonDashPrefab;


    /// <summary>
    /// 대쉬 실행시 실행할 액션
    /// </summary>
    public Action _DashTry;


    public override void Activate(SkillManager skillManager, Player player)
    {
        if(_poisonDashPrefab == null)
        {
            _poisonDashPrefab = Resources.Load<PoisonDash>("Prefabs/Skill/SkillObject/Skill_One/PoisonDash");
        }
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);

        _DashTry = () => ActiveTry(player);
        player.OnDashStarted += _DashTry;
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
        player.OnDashStarted -= _DashTry;
    }

    public override void ActiveTry(Player player)
    {
        Vector3 position = player.transform.position;
        position.y = 0;


        ObjectPool.Instance.GetFromPool(_poisonDashPrefab,position, _poisonDashPrefab.transform.rotation).GetComponent<PoisonDash>().Initialize();
    }


    public Skill_One_Dash(SkillData skillData) : base(skillData)
    {
    }
}

/// <summary>
/// 파이로 하트 대쉬 강화 (깜짝선물)
/// </summary>
public class Skill_Two_Dash : ActiveTech
{
    /// <summary>
    /// 대쉬시 남길 폭탄
    /// </summary>
    private DashReinforcePrefab _dashReinforcePrefab;

    /// <summary>
    /// 대쉬 실행시 실행할 액션
    /// </summary>
    public Action _DashTry;

    

    public override void Activate(SkillManager skillManager, Player player)
    {
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);
        _dashReinforcePrefab = Resources.Load<DashReinforcePrefab>("Prefabs/Skill/SkillObject/Skill_Two/SkillTwoDash");

        _DashTry = () => ActiveTry(player);
        player.OnDashStarted += _DashTry;
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);

        // 이벤트 해제
        player.OnDashStarted -= _DashTry;

    }

    public override void ActiveTry(Player player)
    {
        Vector3 position = player.transform.position;
        position.y = 0;
        ObjectPool.Instance.GetFromPool(_dashReinforcePrefab, position, _dashReinforcePrefab.transform.rotation).GetComponent<DashReinforcePrefab>().Initialize();
    }

    public Skill_Two_Dash(SkillData skillData) : base(skillData)
    {
    }
}


public class Skill_Three_Dash : ActiveTech
{
    /// <summary>
    /// 대쉬 시작 넉백 프리팹
    /// </summary>
    [SerializeField]
    private KnockBackDash _knockBackDashPrefab;



    /// <summary>
    /// 대쉬 실행시 실행할 액션
    /// </summary>
    public Action _DashTry;


    public override void Activate(SkillManager skillManager, Player player)
    {
        if (_knockBackDashPrefab == null)
        {
            _knockBackDashPrefab = Resources.Load<KnockBackDash>("Prefabs/Skill/SkillObject/Skill_Three/KnockBackDash");
        }
        // 공격 적중 시 이벤트에 추가
        base.Activate(skillManager, player);

        _DashTry = () => ActiveTry(player);
        player.OnDashStarted += _DashTry;
    }
    public override void Deactivate(Player player, bool isSameSkill)
    {
        // 리스트 제거
        base.Deactivate(player, isSameSkill);
        // 이벤트 해제
        player.OnDashStarted -= _DashTry;
    }

    public override void ActiveTry(Player player)
    {
        Vector3 position = player.transform.position;
        position.y = 0;
        KnockBackDashData dashData = new KnockBackDashData(
            _skillData.skillBaseValue_1 + _skillData.skillLevelValue_1 * _skillData.skillLevel, // 데미지
            _skillData.skillBaseValue_2 + _skillData.skillLevelValue_2 * _skillData.skillLevel); // 넉백 거리

        ObjectPool.Instance.GetFromPool(_knockBackDashPrefab, position, _knockBackDashPrefab.transform.rotation,null, dashData).GetComponent<KnockBackDash>().Initialize();
    }


    public Skill_Three_Dash(SkillData skillData) : base(skillData)
    {
    }
}