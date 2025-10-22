using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GridForge 기술 목록
/// </summary>

public class Skill_Four : SkillBase
{
    #region 드론
    [SerializeField]
    private Drone dronePrefab;
    private List<Drone> droneList = new List<Drone>();
    public override void InitializeSkill(SkillManager skillManager)
    {
        base.InitializeSkill(skillManager);
        if (dronePrefab == null)
        {
            dronePrefab = Resources.Load<Drone>("Prefabs/Drone/DronePrefab");

        }
    }
    #endregion

    #region 강화된 추진력
    private float plusMoveSpeed;
    private float plusMoveSpeedTime;
    #endregion
    public override void ActivateSkill(SkillData choosedSkill)
    {

        switch (choosedSkill.skillIdx)
        {
            // 찌릿찌릿
            case 40:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActiveTech skillAttack = new Skill_Four_Attack(choosedSkill);
                if (_skillManager.attackTech != null)
                {
                    _skillManager.attackTech.Deactivate(player, _skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx);
                }
                skillAttack.Activate(_skillManager, player);
                break;

                // EMP
            case 41:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 급속냉동
            case 42:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 플라즈마 쉴드
            case 43:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

                // 취약
            case 44:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;

                // 점진되는 고통
            case 45:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                player.playerModel.GetDebuffHandler().ConnectIncreasePain();
                break;

                // 드론무리
            case 46:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateSpawnDrone(choosedSkill.skillLevel);
                break;


                // 강화된 추진력
            case 47:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateThrust(choosedSkill);
                break;


          
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }

    }

    #region 드론무리
    /// <summary>
    /// 드론 소환
    /// </summary>
    /// <param name="skillLevel"></param>
    public void ActivateSpawnDrone(int skillLevel)
    {
        Transform playerTransform = GameManager.Instance.player.transform;
        Drone drone = ObjectPool.Instance.GetFromPool(dronePrefab,Vector3.zero , playerTransform.rotation,playerTransform).GetComponent<Drone>();
        droneList.Add(drone);

        float x = 0 - (float)(skillLevel - 1) / 2;

        for (int i = 0; i < skillLevel; i++)
        {

            if (skillLevel % 2 == 1)
            {
                droneList[i].transform.localPosition = new Vector3((x + i), 1, -1 + Mathf.Abs(skillLevel / 2 - i) * Constants.DRONE_Z_POSITION);

            }
            else if (skillLevel == 4)
            {
                if (i == 0 || i == 3)
                {
                    droneList[i].transform.localPosition = new Vector3((x + i), 1, -1 + Constants.DRONE_Z_POSITION);

                }
                else
                {
                    droneList[i].transform.localPosition = new Vector3((x + i), 1, -1);

                }
            }
            else
            {
                droneList[i].transform.localPosition = new Vector3((x + i), 1, -1);
            }
        }

    }
    #endregion

    #region 강화된 추진력
    private void ActivateThrust(SkillData choosedSkill)
    {
        Debug.Log(skillManager.playerStatManager == null);
        // 대쉬 이동거리 value1 만큼 증가
        skillManager.playerStatManager.AddPlayerDashDistanceMulti(choosedSkill.skillBaseValue_1);
        // 대쉬 후 value3초간 value2 만큼 이동속도 증가 이벤트 연결
        plusMoveSpeed = choosedSkill.skillBaseValue_2;
        plusMoveSpeedTime = choosedSkill.skillBaseValue_3;


        //TODO 대쉬 끝 이벤트에 연결
        //player.dashEnd += plusMoveSpeedAfterDash;
    }

    private void plusMoveSpeedAfterDash()
    {
        skillManager.playerStatManager.AddPlayerMoveSpeed(plusMoveSpeed);
        StartCoroutine(MinusMoveSpeedAfterDash());
    }
    IEnumerator  MinusMoveSpeedAfterDash()
    {
        yield return new WaitForSeconds(plusMoveSpeedTime);
        skillManager.playerStatManager.AddPlayerMoveSpeed(-plusMoveSpeed);
    }
    #endregion
}
