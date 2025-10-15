using UnityEngine;

public class BossDoor : Door
{
    [SerializeField] string _doorName = "bossDoor";
    [SerializeField] float _doorChance = 0.05f; // 보스룸의 등장 확률 설정
    [SerializeField] DoorType _doorType = DoorType.Boss;

    public override string DoorName => _doorName;

    public override float DoorChance => _doorChance;

    public override DoorType DoorType => _doorType;

    public override void Initialize()
    {
        //Debug.Log("보스룸 생성됨");
    }
}