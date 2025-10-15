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
    public override void ActivateSkill(SkillData choosedSkill)
    {

        switch (choosedSkill.skillIdx)
        {
            case 40:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 41:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 42:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 43:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                break;

            case 44:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 45:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 46:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateSpawnDrone(choosedSkill.skillLevel);
                break;
            case 47:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 48:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 49:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            default:
                Debug.Log("에러, 배정되지 않은 idx");
                break;

        }

    }


    /// <summary>
    /// 드론 소환
    /// </summary>
    /// <param name="skillLevel"></param>
    public void ActivateSpawnDrone(int skillLevel)
    {
        Transform playerTranform = GameManager.Instance.player.transform;
        Drone drone = Instantiate(dronePrefab, playerTranform);
        drone.InitializeDrone(GameManager.Instance.player);
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
}
