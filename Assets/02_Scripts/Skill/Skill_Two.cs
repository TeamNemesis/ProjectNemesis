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
										if (_skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.attackTech.Deactivate(player);
										}
								}
                skillAttack.Activate(_skillManager, player);
                break;

                // 폭발!
            case 21:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								ActiveTech skillGrenade = new Skill_Two_Grenade(choosedSkill);
								if (_skillManager.bombTech != null)
								{
										if (_skillManager.bombTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.bombTech.Deactivate(player);
										}
								}
								skillGrenade.Activate(_skillManager, player);
								break;

                // 비밀무기
            case 22:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillSPAttack = new Skill_Two_SPAttack(choosedSkill);
                if (_skillManager.attackTech != null)
                {
										if (_skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.attackTech.Deactivate(player);
										}
								}
                skillSPAttack.Activate(_skillManager, player);
                break;

                // 깜짝선물
            case 23:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

                // 전투장비 강화
            case 24:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 폭격 클러스터
            case 25:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 폭사
            case 26:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                //TODO 몬스터 스포너에 몬스터 생성시 이벤트에 연결
                // monsterSpawner.OnSpawnMonster += (monster)=>monster.OnDie+=()=>MakeExplosion(monster.transform);

                break;

                // 기폭제
            case 27:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                //TODO 범위 스킬 구현에 대한 계수 회의 필요
                // 범위 증가
                Constants.SKILL_EXTENT = 1.1f;
                break;
            
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }
    }

    #region 폭사
    public void MakeExplosion(Transform monsterTransform)
    {
        Vector3 position = monsterTransform.position;
        position.y = 0;
        ObjectPool.Instance.GetFromPool<ExplosionDeath>(_explosionDeathPrefab,position);
    }
    #endregion
}


