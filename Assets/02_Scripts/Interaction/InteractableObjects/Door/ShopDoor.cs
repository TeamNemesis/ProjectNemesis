using UnityEngine;

public class ShopDoor : Door
{
    [SerializeField] string _doorName = "shopDoor";
    [SerializeField] float _doorChance = 0.1f; // 상점룸의 등장 확률 설정
    [SerializeField] DoorType _doorType = DoorType.Shop;

    public override string DoorName => _doorName;
    public override float DoorChance => _doorChance;
    public override DoorType DoorType => _doorType;

    public override void Initialize()
    {
        //Debug.Log("상점 생성됨");
    }
}