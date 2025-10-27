using System;
using UnityEngine;

/// <summary>
/// 추후 파이로 하트 모션
/// </summary>
public class Skill_Two : SkillBase
{
    /// <summary>
    /// 폭사 프리팹
    /// </summary>
    [SerializeField]
    private ExplosionDeath _explosionDeathPrefab;

    /// <summary>
    /// 전투스킬 강화, 모든 데미지 추가, 스킬 인덱스, 증가값
    /// </summary>
    public event Action<int, float> CombatEquipment;

    public override void ActivateSkill(SkillData choosedSkill)
    {

        switch (choosedSkill.skillIdx)
        {
            // 베고, 찌르고, 부수고
            case 20:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillAttack = new Skill_Two_Attack(choosedSkill);
                if (_skillManager.attackTech != null)
                {
                    _skillManager.attackTech.Deactivate(_skillManager.playScene.player, _skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                skillAttack.Activate(_skillManager, _skillManager.playScene.player);
                break;

            // 폭발!
            case 21:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillGrenade = new Skill_Two_Grenade(choosedSkill);
                if (_skillManager.bombTech != null)
                {

                    _skillManager.bombTech.Deactivate(_skillManager.playScene.player, _skillManager.bombTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                skillGrenade.Activate(_skillManager, _skillManager.playScene.player);
                break;

            // 비밀무기
            case 22:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillSPAttack = new Skill_Two_SPAttack(choosedSkill);
                if (_skillManager.skillTech != null)
                {

                    _skillManager.skillTech.Deactivate(_skillManager.playScene.player, _skillManager.skillTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                skillSPAttack.Activate(_skillManager, _skillManager.playScene.player);
                break;

            // 깜짝선물
            case 23:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillDashAttack = new Skill_Two_Dash(choosedSkill);
                if (_skillManager.dashTech != null)
                {

                    _skillManager.dashTech.Deactivate(_skillManager.playScene.player, _skillManager.dashTech.skillData.skillIdx != choosedSkill.skillIdx);

                }
                skillDashAttack.Activate(_skillManager, _skillManager.playScene.player);
                break;

            // 전투장비 강화
            case 24:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                OnCombatEquipment(choosedSkill);
                break;

            // 개선된 폭격
            case 25:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                if (choosedSkill.skillLevel == 1)
                {
                    ActiveImprovedBomb(choosedSkill.skillBaseValue_1 + choosedSkill.skillLevelValue_1);
                }
                else
                {
                    ActiveImprovedBomb(choosedSkill.skillLevelValue_1);
                }
                break;

            // 폭사
            case 26:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");


                //TODO 몬스터 스포너에 몬스터 생성시 이벤트에 연결
                skillManager.playScene.MapController.MonsterController.MonsterSpawner.OnMonsterSpawned -= ConnectMakeExpolsion;
                skillManager.playScene.MapController.MonsterController.MonsterSpawner.OnMonsterSpawned += ConnectMakeExpolsion;
                break;

            // 기폭제
            case 27:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                //TODO 범위 스킬 구현에 대한 계수 회의 필요
                // 범위 증가
                if (choosedSkill.skillLevel == 1)
                {
                    ActivePriming(choosedSkill.skillBaseValue_1 + choosedSkill.skillLevelValue_1);
                }
                else
                {
                    ActivePriming(choosedSkill.skillLevelValue_1);
                }
                break;

            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }

    #region 폭사
    public void ConnectMakeExpolsion(MonsterBase monster)
    {
        Action handler = null;
        handler = () =>
        {
            monster.OnDieEvent -= handler; 
            MakeExplosion(monster.transform);
        };

        monster.OnDieEvent += handler;
    }

    public void MakeExplosion(Transform monsterTransform)
    {
        Vector3 position = monsterTransform.position;
        position.y = 0;
        GameManager.Instance.PoolManager.GetFromPool(_explosionDeathPrefab, position, _explosionDeathPrefab.transform.rotation).GetComponent<ExplosionDeath>().Initialize();
    }
    #endregion

    #region 전투장비 강화

    /// <summary>
    /// 전투장비 이벤트 발행
    /// </summary>
    /// <param name="skillData"></param>
    public void OnCombatEquipment(SkillData skillData)
    {
        float totalDamageUp = (float)(skillData.skillLevel * skillData.skillLevelValue_1 + skillData.skillBaseValue_1);
        CombatEquipment?.Invoke(skillData.skillIdx, totalDamageUp);

    }

    #endregion

    #region 개선된 폭격
    public void ActiveImprovedBomb(float skillData)
    {
        _skillManager.playerStatManager.MinusGrenadeCoolTimeMulti(skillData);
    }
    #endregion

    #region 기폭제
    public void ActivePriming(float skillData)
    {
        _skillManager.playerStatManager.AddPlayerAreaExtent(skillData);
    }
    #endregion
}


