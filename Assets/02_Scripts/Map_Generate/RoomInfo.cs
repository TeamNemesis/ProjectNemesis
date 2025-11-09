using System;
using System.Collections.Generic;

/// <summary>
/// 각 방의 런타임 정보를 담는 클래스
/// </summary>
[Serializable]
public class RoomInfo
{       
    public RoomType RoomType;                          // 방 타입
    public NormalRoomType? NormalType;             // 일반 방 타입(일반 방이 아닐 경우 null)
    public TechSelectPackType? TechType;     // 일반 방의 보상이 기술팩 타입일 경우 해당 기술팩 타입(기술팩이 아닐 경우 null)

    Dictionary<RoomType, string> _roomTitle = new Dictionary<RoomType, string>()
    {
        { RoomType.Normal, "NormalRoomTitle" },
        { RoomType.Shop, "ShopTitle" },
        { RoomType.Colosseum, "ColosseumTitle" },
        { RoomType.Lab, "LabTitle" },
        { RoomType.Boss, "BossTitle" },
    };

    Dictionary<RoomType, string> _roomDescription = new Dictionary<RoomType, string>()
    {
        {  RoomType.Normal, "normal" },
        { RoomType.Shop, "shop" },
        { RoomType.Colosseum, "colosseum" },
        { RoomType.Lab, "lab" },
        { RoomType.Boss, "boss" },
    };

    public RoomInfo(RoomType roomType, NormalRoomType? normalRoomType = null, TechSelectPackType? techSelectPackType = null)
    {
        RoomType = roomType;
        NormalType = normalRoomType;
        TechType = techSelectPackType;
    }

    public bool TryGetTechSelect(out TechSelectPackType value)
    {
        if (RoomType == RoomType.Normal &&
            NormalType.HasValue &&
            NormalType.Value == NormalRoomType.TechSelect &&
            TechType.HasValue)
        {
            value = TechType.Value;
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGetNormal(out NormalRoomType value)
    {
        if (RoomType == RoomType.Normal && NormalType.HasValue)
        {
            value = NormalType.Value;
            return true;
        }
        value = default;
        return false;
    }

    public string GetRoomTitle()
    {
        if (_roomTitle.TryGetValue(RoomType, out var title))
        {
            return title;
        }
        return "알 수 없는 방";
    }

    public string GetRoomDescription()
    {
        if (_roomDescription.TryGetValue(RoomType, out var description))
        {
            return description;
        }
        return "설명 없음";
    }
}