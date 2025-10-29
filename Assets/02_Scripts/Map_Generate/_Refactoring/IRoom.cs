using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IRoom 인터페이스: StageController / RoomSpawner 등과의 계약.
/// - Room은 Enter/Exit 흐름을 제공하고 보상(SpawnRewards)과 풀 오브젝트 목록을 관리한다.
/// - 이 인터페이스를 구현하면 StageController가 룸 라이프사이클을 일관되게 제어할 수 있습니다.
/// </summary>
public interface IRoom
{
    // 런타임 메타
    RoomInfo RoomInfo { get; }

    // 룸 진입/퇴장 이벤트 (StageController가 구독)
    event Action<IRoom> OnEntered;
    event Action<IRoom> OnExited;

    // 룸이 소유/관리하는 풀 오브젝트들 (StageController가 반환할 때 사용)
    IReadOnlyList<GameObject> PoolableObjectsInRoom { get; }

    // 초기화: RoomInfo를 받아 내부 상태를 설정
    void Initialize(RoomInfo roomInfo);

    // 룸 시작(입장). 내부 연출/준비가 끝난 뒤 OnEntered를 호출해야 함.
    void Enter();

    // 룸 종료(퇴장). 내부 정리 작업 수행.
    void Exit();

    Transform[] GetNextDoorPositions(int count);
}