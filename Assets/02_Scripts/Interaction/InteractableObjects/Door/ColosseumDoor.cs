using UnityEngine;

public class ColosseumDoor : Door
{
    [SerializeField] string _doorName = "colosseumDoor";
    [SerializeField] float _doorChance = 0.1f; // 콜로세움룸의 등장 확률 설정
    [SerializeField] DoorType _doorType = DoorType.Colosseum;

    public override string DoorName => _doorName;
    public override float DoorChance => _doorChance;
    public override DoorType DoorType => _doorType;

    public override void Initialize()
    {
        //Debug.Log("콜로세움룸 생성됨");
    }
}