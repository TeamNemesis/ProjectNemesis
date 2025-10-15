using UnityEngine;

public class StartDoor : Door
{
    [SerializeField] string _doorName = "startDoor";
    [SerializeField] float _doorChance = 1.0f; // 시작룸의 등장 확률 설정
    [SerializeField] DoorType _doorType = DoorType.Start;

    public override string DoorName => _doorName;
    public override float DoorChance => _doorChance;
    public override DoorType DoorType => _doorType;

    public override void Initialize()
    {
        //Debug.Log("시작룸 생성됨");
    }
}