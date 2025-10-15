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
/// 현재 방에서 다음 방 선택지를 생성할 때의 역할을 담당하는 클래스
/// </summary>
public class NextDoorsDecider : MonoBehaviour
{
    [SerializeField] Room[] _allDoors; // 모든 방 프리팹들을 담는 배열

    [Header("다음 방 선택지 개수 확률")]
    [SerializeField] float _oneDoorChance = 0.1f; // 다음 방이 1개일 확률
    [SerializeField] float _twoDoorChance = 0.6f; // 다음 방이 2개일 확률
    [SerializeField] float _threeDoorChance = 0.6f; // 다음 방이 3개일 확률

    [Header("다음 방 선택용 문 생성 위치")]
    [SerializeField] Transform[] _doorPositions; // 문이 생성될 위치들(최대 3개)

    [Header("다음 방 선택지 결정")]
    [SerializeField] RoomType _currentDoorType; // 현재 방의 타입
    [SerializeField] int _countOfNextDoors; // 다음 방 선택지의 개수(1~3)
    [SerializeField] int _currentDoorIndex; // 현재 방의 인덱스(0~15)

    Dictionary<RoomType, Room> _candidateDoorsMap = new(); // 다음 방 후보군을 담는 딕셔너리(초기화 시 고정)
    Dictionary<RoomType, Room> _nextDoorsMap = new(); // 다음 방 선택지로 등장할 방들을 저장할 딕셔너리(계속 바뀜)

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _countOfNextDoors = GetNextDoorCount();
            GenerateDoor(_countOfNextDoors);
        }
    }

    public void Initialize()
    {
        foreach(Room door in _allDoors)
        {
            if(door.RoomType == RoomType.Start || door.RoomType == RoomType.Boss)
                continue;
            _candidateDoorsMap.Add(door.RoomType, door);
        }
    }

    public void GenerateDoor(int countOfNextDoors)
    {
        // 0번방(무기고)에서는 반드시 일반방으로 이어지는 1개의 문만 생성
        // 1번방부터 13번방까지는 확률적으로 1~3개의 문을 생성하되,
        // 선택지에 상점과 실험실, 콜로세움이 중복되거나 연속으로 등장하지 않도록 함
        // 일반방은 중복되거나 연속으로 등장해도 상관없음
        // 12번방에서는 반드시 상점으로 이어지는 1개의 문을 포함하여 1~3개의 문을 생성
        // 13번방에서는 반드시 보스로 이어지는 1개의 문을 생성
        // 14번방(보스방)에서는 게임 클리어 후 엔딩으로 이어지는 1개의 문만 생성
        // 그럼 현재 방의 인덱스별로 어떻게 다음 방 후보군을 구성하지?
        Room[] nextDoors = GetNextDoors(countOfNextDoors);
    }

    /// <summary>
    /// count 수만큼 다음 방 선택지로 등장할 방들을 확률적으로 결정하여 반환
    /// </summary>
    /// <param name="count">반환할 방의 수</param>
    /// <returns></returns>
    public Room[] GetNextDoors(int count)
    {
        // 다음 방 선택지로 등장할 방들을 저장하는 딕셔너리 초기화
        _nextDoorsMap.Clear();

        // 다음 방 후보군 딕셔너리를 복사하여 다음 방 선택지 딕셔너리로 사용
        //_nextDoorsMap = _candidateDoorsMap; ---> 기존 사용방식인데 이건 참조형식이라서 원본이 바뀜
        _nextDoorsMap = new Dictionary<RoomType, Room>(_candidateDoorsMap);

        // 현재 방이 상점, 실험실, 콜로세움인 경우 
        if (_currentDoorType == RoomType.Shop || _currentDoorType == RoomType.Lab || _currentDoorType == RoomType.Colosseum)
        {
            //다음 방 후보군에서 제거(연속으로 등장하지 않도록)
            _nextDoorsMap.Remove(_currentDoorType);
        }

        // 딕셔너리에 남아있는 방들 중에서 확률적으로 다음 방 선택지의 개수를 결정하여 반환
        Room[] nextDoors = new Room[count];
        for (int i = 0; i < count; i++)
        {
            // for문 돌때마다 전체 확률 초기화
            float totalChance = 0f;
            // 딕셔너리에 남아있는 방들의 확률을 모두 더해서 totalChance에 저장
            foreach (var door in _nextDoorsMap.Values)
            {
                totalChance += door.RoomChance;
            }
            // 0부터 totalChance 사이의 랜덤한 값을 생성
            float rand = Random.Range(0f, totalChance);
            // for문 돌때마다 누적 확률 초기화
            float cumulativeChance = 0f;
            // 다시 딕셔너리에 남아있는 방들을 돌면서 누적 확률을 계산하고
            foreach (var door in _nextDoorsMap.Values)
            {
                cumulativeChance += door.RoomChance;
                //랜덤 값이 누적 확률보다 작거나 같아지는 순간
                if (rand <= cumulativeChance)
                {
                    //해당 방을 선택
                    nextDoors[i] = door;
                    // 일반 방이 아닌 방이 선택된 경우 다음 선택지에서 제외
                    if (door.RoomType != RoomType.Normal)
                    {
                        _nextDoorsMap.Remove(door.RoomType);
                    }
                    break;
                }
            }
        }

        //for (int i = 0; i < nextDoors.Length; i++)
        //{
        //    //Debug.Log(nextDoors[i].DoorName);
        //    Instantiate(nextDoors[i]);
        //}
        return nextDoors;
    }

    /// <summary>
    /// 다음 방 선택지의 개수를 확률적으로 결정하여 반환
    /// </summary>
    /// <returns></returns>
    public int GetNextDoorCount()
    {
        // 전체 확률의 합을 계산
        float totalChance = _oneDoorChance + _twoDoorChance + _threeDoorChance;
        // 0부터 totalChance 사이의 랜덤한 값을 생성
        float rand = Random.Range(0f, totalChance);
        Debug.Log(rand);
        // 랜덤 값에 따라 다음 방의 개수를 결정
        if (rand < _oneDoorChance)
        {
            Debug.Log("1개 방 선택됨");
            return 1;
        }
        else if (rand < _oneDoorChance + _twoDoorChance)
        {
            Debug.Log("2개 방 선택됨");
            return 2;
        }
        else
        {
            Debug.Log("3개 방 선택됨");
            return 3;
        }
    }
}