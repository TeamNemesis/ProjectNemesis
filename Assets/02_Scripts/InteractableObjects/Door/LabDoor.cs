using UnityEngine;

public class LabDoor : Door
{
    [SerializeField] string _doorName = "labDoor";
    [SerializeField] float _doorChance = 0.1f; // 실험실룸의 등장 확률 설정
    [SerializeField] DoorType _doorType = DoorType.Lab;

    public override string DoorName => _doorName;
    public override float DoorChance => _doorChance;
    public override DoorType DoorType => _doorType;

    public override void Initialize()
    {
        //Debug.Log("실험실 생성됨");
    }
}