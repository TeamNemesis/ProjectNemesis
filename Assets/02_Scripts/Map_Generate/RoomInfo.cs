/// <summary>
/// 각 방의 런타임 정보를 담는 클래스
/// </summary>
public class RoomInfo
{       
    public RoomType RoomType;                          // 방 타입
    public NormalRoomType? NormalRoomType;             // 일반 방 타입(일반 방이 아닐 경우 null)
    public TechSelectPackType? TechSelectPackType;     // 일반 방의 보상이 기술팩 타입일 경우 해당 기술팩 타입(기술팩이 아닐 경우 null)

    public RoomInfo(RoomType roomType, NormalRoomType? normalRoomType = null, TechSelectPackType? techSelectPackType = null)
    {
        RoomType = roomType;
        NormalRoomType = normalRoomType;
        TechSelectPackType = techSelectPackType;
    }
}