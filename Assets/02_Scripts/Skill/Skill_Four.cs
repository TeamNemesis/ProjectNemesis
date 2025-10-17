using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GridForge 기술 목록
/// </summary>

public class Skill_Four : SkillBase
{

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
    public override void ActivateSkill(SkillData choosedSkill)
    {

        switch (choosedSkill.skillIdx)
        {
            // 찌릿찌릿
            case 40:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

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

                break;

                // 드론무리
            case 46:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateSpawnDrone(choosedSkill.skillLevel);
                break;


                // 강화된 추진력
            case 47:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

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
        Transform playerTranform = GameManager.Instance.player.transform;
        Drone drone = ObjectPool.Instance.GetFromPool<Drone>(dronePrefab,Vector3.zero ,playerTranform);
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
}
