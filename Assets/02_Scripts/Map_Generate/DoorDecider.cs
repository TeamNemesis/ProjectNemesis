using System;
using System.Collections.Generic;
using UnityEngine;

// 0~14
// 0 = 무기고, 14 = 보스방
// 1 : 무기고에서 나오는 방(무기고 기준 출구 1개이고 1번째방은 일반방 확정)
// 실험실은 맵 전체 구간에서 한번만, 안뜰수도 있고 안들어가면 카운트 증가x
// 상점은 연속으로 안뜨고, 12번째방에서 선택지 중 하나로 무조건 뜸
// 선택지에 중복 등장x 연속 등장x
// 보스는 마지막방에 반드시 등장

// 맵 클리어
// -> 생성할 방의 개수 랜덤(1~3개) (SetDoorCount())
// -> 랜덤으로 선택된 개수만큼의 방을 생성할건데, _nextDoorList에서 확률적으로 선택하여 생성
// 그럼 _nextDoorList는 어떻게 구성?
// -> 현재 방의 타입과 인덱스를 기반으로 다음 방 후보군을 구성
// 그럼 _nextDoorList는 어느시점에 구성?
// -> 현재 방에서 다음 방으로 넘어갈 때마다 구성 (SetNextDoorList())
// -> 선택된 방들 중 하나를 플레이어가 선택하여 이동
// -> 이동한 방에서 다시 생성할 방의 개수를 랜덤으로 결정
// -> 반복
// -> 14번째 방에서는 상점이 무조건 등장(선택지 중 하나로)
// -> 마지막 방에서는 보스방이 무조건 등장

/// <summary>
/// 다음 방 후보군을 구성하고, 확률적으로 다음 방 선택지를 반환하는 정책 담당 클래스
/// 상태(현재 방, 인덱스, 특수방 등장 여부 등)는 외부에서 인자로 받는다.
/// </summary>
public class DoorDecider : MonoBehaviour
{
    [Header("다음 방 선택지 개수 확률")]
    [SerializeField] float _oneDoorChance = 0.1f; // 다음 방이 1개일 확률
    [SerializeField] float _twoDoorChance = 0.6f; // 다음 방이 2개일 확률
    [SerializeField] float _threeDoorChance = 0.3f; // 다음 방이 3개일 확률

    // 다음 방 후보군들의 생성 확률을 저장할 딕셔너리
    Dictionary<RoomType, float> _candidateRoomChanceMap = new();

    public void Initialize()
    {
        InitializeCandidateRoomChanceDict();
    }

    /// <summary>
    /// 다음 방 후보군 딕셔너리 초기화
    /// </summary>
    void InitializeCandidateRoomChanceDict()
    {
        foreach (GameObject roomObj in GameManager.Instance.ResourceManager.RoomPrefabMap.Values)
        {
            Room room = roomObj.GetComponent<Room>();
            if (room != null && room.RoomType != RoomType.Boss && room.RoomType != RoomType.Start && !_candidateRoomChanceMap.ContainsKey(room.RoomType))
            {
                _candidateRoomChanceMap.Add(room.RoomType, room.RoomChance);
            }
        }
    }

    /// <summary>
    /// 문의 개수를 확률적으로 결정하여 반환
    /// </summary>
    public int GetNextDoorCount()
    {
        float totalChance = _oneDoorChance + _twoDoorChance + _threeDoorChance;
        float rand = UnityEngine.Random.Range(0f, totalChance);

        if (rand < _oneDoorChance)
            return 1;
        else if (rand < _oneDoorChance + _twoDoorChance)
            return 2;
        else
            return 3;
    }

    /// <summary>
    /// 조건에 따라 count 수만큼 다음 방 선택지로 등장할 방들의 타입을 확률적으로 결정하여 반환
    /// MapController 등 호출자가 모든 상태 정보를 인자로 넘김
    /// </summary>
    /// <param name="count">반환할 방의 수</param>
    /// <param name="currentRoomType">현재 방 타입</param>
    /// <param name="currentRoomIndex">현재 방 인덱스</param>
    /// <param name="hasLabRoomAppeared">실험실 등장 여부</param>
    /// <returns></returns>
    public RoomType[] GetNextDoorTypes(int count, RoomType currentRoomType, int currentRoomIndex, bool hasLabRoomAppeared)
    {
        if (count < 1 || count > 3)
            throw new ArgumentOutOfRangeException("문의 개수는 1~3개 사이여야 합니다.");
        if (currentRoomIndex < 0 || currentRoomIndex > 14)
            throw new ArgumentOutOfRangeException("방 인덱스는 0~14 사이여야 합니다.");

        // 0번 방(무기고): 다음 방은 무조건 일반방
        if (currentRoomIndex == 0)
            return new RoomType[] { RoomType.Normal };

        // 13번 방(보스 직전): 보스 확정 등장
        if (currentRoomIndex == 13)
            return new RoomType[] { RoomType.Boss };

        // 12번 방(상점 강제)
        var types = new List<RoomType>();
        if (currentRoomIndex == 12)
            types.Add(RoomType.Shop);

        // 후보군 복사
        var nextRoomChanceMap = new Dictionary<RoomType, float>(_candidateRoomChanceMap);

        // 실험실은 맵 전체에서 한 번만 등장
        if (hasLabRoomAppeared)
            nextRoomChanceMap.Remove(RoomType.Lab);

        // 특수방 연속 방지: 현재 방이 Shop/Lab/Colosseum이면 그 방은 후보군에서 제외 (일반방은 제외 안 함)
        foreach (var specialType in new[] { RoomType.Shop, RoomType.Lab, RoomType.Colosseum })
        {
            if (currentRoomType == specialType)
                nextRoomChanceMap.Remove(specialType);
        }

        // 12번 방이면 상점은 이미 넣었으므로 후보군에서 제거
        if (currentRoomIndex == 12)
            nextRoomChanceMap.Remove(RoomType.Shop);

        // 선택지 중복 방지: 일반방은 중복 허용, 특수방은 중복 제거
        for (int i = types.Count; i < count; i++)
        {
            // 특수방 중복 제거: 이미 선택된 특수방은 후보군에서 제거
            foreach (var t in types)
            {
                if (t != RoomType.Normal)
                    nextRoomChanceMap.Remove(t);
            }

            // 확률 계산
            float totalChance = 0f;
            foreach (var kvp in nextRoomChanceMap)
                totalChance += kvp.Value;

            float rand = UnityEngine.Random.Range(0f, totalChance);
            float cumulative = 0f;
            RoomType chosen = RoomType.Normal;

            foreach (var kvp in nextRoomChanceMap)
            {
                cumulative += kvp.Value;
                if (rand <= cumulative)
                {
                    chosen = kvp.Key;
                    break;
                }
            }

            types.Add(chosen);

            // 일반방은 중복 허용이므로 후보군에서 제거 안 함
            // 특수방만 후보군에서 제거(위 for문에서 처리됨)
        }

        return types.ToArray();
    }
}