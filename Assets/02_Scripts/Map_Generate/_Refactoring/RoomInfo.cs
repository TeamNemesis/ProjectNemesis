using System;

/// <summary>
/// 각 방의 런타임 정보를 담는 클래스
/// - 보상 미리보기(DecidedRewards)를 저장하도록 확장되었습니다.
/// </summary>
[Serializable]
public class RoomInfo
{
    public RoomType RoomType;                          // 방 타입
    public NormalRoomType? NormalType;             // 일반 방 타입(일반 방이 아닐 경우 null)
    public TechSelectPackType? TechType;     // 일반 방의 보상이 기술팩 타입일 경우 해당 기술팩 타입(기술팩이 아닐 경우 null)

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

    string GetKoreanNormalRoomType(NormalRoomType normalType)
    {
        string result = normalType switch
        {
            NormalRoomType.Credit => "크레딧",
            NormalRoomType.Heal => "체력회복",
            NormalRoomType.Chrome => "크롬",
            NormalRoomType.TechSelect => "기술 선택",
            NormalRoomType.TechUpgrade => "기술 업그레이드",
            _ => "알 수 없는 일반 방",
        };
        return result;
    }

    public string GetRoomTitle()
    {
        return RoomType switch
        {
            RoomType.Normal => GetKoreanNormalRoomType(NormalType.Value),
            RoomType.Boss => "보스 전투",
            RoomType.Shop => "상점",
            RoomType.Lab => "연구소",
            RoomType.Colosseum => "콜로세움",
            _ => "알 수 없는 방",
        };
    }

    public string GetRoomDescription()
    {
        return RoomType switch
        {
            RoomType.Normal => $"스폰되는 적을 무찌르고 {GetKoreanNormalRoomType(NormalType.Value)}을(를) 획득하세요.",
            RoomType.Boss => "강력한 보스와의 전투가 기다리고 있습니다!",
            RoomType.Shop => "전투에 도움이 되는 아이템들을 구매할 수 있는 상점입니다.",
            RoomType.Lab => "돌연변이를 획득하여 강력한 기술을 획득할 수 있는 연구소입니다.",
            RoomType.Colosseum => "엘리트 몬스터와의 1:1 진검승부를 할 수 있는 콜로세움입니다.",
            _ => "알 수 없는 방입니다.",
        };
    }
}