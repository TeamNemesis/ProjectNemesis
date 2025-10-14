using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 추후 GridForge 기술로 변경
/// </summary>
public class Skill_One : SkillBase
{
    [SerializeField]
    private Drone dronePrefab;
    private List<Drone> droneList = new List<Drone>();

    public override void ActivateSkill(SkillData choosedSkill)
    {
        switch (choosedSkill.skillIdx)
        {
            case 10:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
                ActivateSpawnDrone(choosedSkill.skillLevel);
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

            case 14:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

                break;
            case 15:
                Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");

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

    /// <summary>
    /// 드론 소환
    /// </summary>
    /// <param name="skillLevel"></param>
    public void ActivateSpawnDrone(int skillLevel)
    {
        Transform playerTranform = GameManager.Instance().player.transform;
        Drone drone = Instantiate(dronePrefab, playerTranform);
        drone.InitializeDrone(GameManager.Instance().player);
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
