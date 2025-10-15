using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] DoorInteractor _doorInteractor;    // 문이 상호작용 가능하도록 하는 컴포넌트

    public void Initialize(RoomType roomType)
    {
        
        SetReward(roomType);
    }

    /// <summary>
    /// 문이 생성될 때 문의 보상을 설정하는 함수
    /// </summary>
    public void SetReward(RoomType roomType)
    {
        
    }
}