using UnityEngine;

public class ColosseumRoom : Room
{
    [SerializeField] string _roomName = "colosseumDoor";
    [SerializeField] float _roomChance = 0.1f; // 콜로세움룸의 등장 확률 설정
    [SerializeField] RoomType _roomType = RoomType.Colosseum;

    public override string RoomName => _roomName;
    public override float RoomChance => _roomChance;
    public override RoomType RoomType => _roomType;

    public override void Initialize()
    {
        //Debug.Log("콜로세움룸 생성됨");
    }
}