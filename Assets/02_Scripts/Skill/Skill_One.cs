using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 추후 비브르 기술로 변경
/// </summary>
public class Skill_One : SkillBase
{
    private Coroutine _autoHeal;

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            case 10:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillAttack = new Skill_One_Attack(choosedSkill);
                if (_skillManager.attachTech != null)
                {
                    _skillManager.attachTech.Deactivate(player);
                }
                skillAttack.Activate(_skillManager, player);
                break;
            case 11:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 12:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 13:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

                //넘치는 활력
            case 14:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                player.SetMaxHp(choosedSkill.skillLevel * 30);
                break;

                //초재생
            case 15:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                //TODO 방 입장시 이벤트에 초재생 연결
                //TODO 전투 종료시 이벤트에 초재생 해제 연결
                break;
            case 16:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 17:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 18:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 19:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;
        }

    }

    #region 초재생

    //초재생 코루틴
    public IEnumerator StartAutoHealRoutine()
    {
        int stack = 0;
        while (stack < 20)
        {
            yield return new WaitForSeconds(Constants.HEAL_SECOND);
            player.Heal(Constants.HEAL_AMOUNT);
            stack++;
        }
    }


    /// <summary>
    /// 초재생 실행
    /// </summary>
    public void StartAutoHeal()
    { 
        _autoHeal = StartCoroutine(StartAutoHealRoutine());
    }

    /// <summary>
    /// 초재생 종료
    /// </summary>
    public void StopAutoHeal()
    {
        StopCoroutine(_autoHeal);
        _autoHeal = null;
    }
    #endregion
}


