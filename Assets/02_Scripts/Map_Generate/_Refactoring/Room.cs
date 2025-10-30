using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Room 컴포넌트는 프리팹의 구조/런타임 동작을 담당.
/// 타입/메타는 RoomDataSO에서 주입받아 Initialize로 설정한다.
/// 초기화 진입점은 Initialize(RoomInfo) 로 통일된다.
/// </summary>
[RequireComponent(typeof(Transform))]
public abstract class Room : MonoBehaviour, IRoom
{
    [Header("Door spawn points (setup on prefab)")]
    [SerializeField] protected Transform[] _doorSpawnPointsLeft;
    [SerializeField] protected Transform[] _doorSpawnPointsRight;

    protected RoomInfo _roomInfo;
    protected List<GameObject> _poolableObjectsInRoom = new List<GameObject>();

    // 기존 프로퍼티 노출 (호환성 유지)
    public Transform[] DoorSpawnPointsLeft => _doorSpawnPointsLeft;
    public Transform[] DoorSpawnPointsRight => _doorSpawnPointsRight;

    // IRoom 계약
    public RoomInfo RoomInfo => _roomInfo;
    public abstract event Action<IRoom> OnEntered;
    public event Action<IRoom> OnExited;

    // Room이 보유한 풀 오브젝트 목록을 읽기 전용으로 제공
    public IReadOnlyList<GameObject> PoolableObjectsInRoom => _poolableObjectsInRoom.AsReadOnly();

    #region Initialization / lifecycle

    /// <summary>
    /// 기존 Initialize(RoomInfo)와 동일한 진입점.
    /// 필요하면 서브클래스에서 base.Initialize(roomInfo) 호출 후 추가 초기화 수행.
    /// </summary>
    public virtual void Initialize(RoomInfo roomInfo)
    {
        if (roomInfo == null)
        {
            Debug.LogWarning($"{nameof(Room)}.Initialize called with null RoomInfo on '{name}'. Defaulting to Start room info.");
            return;
        }
        else
        {
            _roomInfo = roomInfo;
        }
    }

    /// <summary>
    /// 룸 시작(입장). 기본 구현은 즉시 OnEntered를 호출합니다.
    /// 서브클래스는 입장 연출(애니/타이머)이 필요하면 오버라이드하여 준비가 끝났을 때 OnEntered?.Invoke(this) 호출하세요.
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 룸 종료(퇴장). 서브클래스는 필요한 정리 동작을 오버라이드하여 수행하세요.
    /// </summary>
    public abstract void Exit();

    #endregion

    #region Door point helpers (unchanged)

    /// <summary>
    /// 입력받은 count 수만큼 다음 문들이 생성될 위치들을 Transform배열로 반환
    /// - 내부적으로 배열 길이/인덱스 미존재에 대해 방어적으로 처리합니다.
    /// - 가능한 경우에만 Transform을 반환하고, 없으면 빈 배열을 반환합니다.
    /// </summary>
    public Transform[] GetNextDoorPositions(int count)
    {
        if (count < 1 || count > 3)
        {
            Debug.LogWarning("GetNextDoorPositions: 생성될 문의 개수는 1~3개 사이여야 합니다.");
            return Array.Empty<Transform>();
        }

        Transform GetPointSafe(Transform[] arr, int idx)
        {
            if (arr == null || arr.Length == 0) return null;
            if (idx >= 0 && idx < arr.Length) return arr[idx];
            int clamped = Mathf.Clamp(idx, 0, arr.Length - 1);
            return arr[clamped];
        }

        Transform PickOneOfFirst()
        {
            var left = GetPointSafe(_doorSpawnPointsLeft, 0);
            var right = GetPointSafe(_doorSpawnPointsRight, 0);
            if (left == null && right == null) return null;
            if (left == null) return right;
            if (right == null) return left;
            return UnityEngine.Random.value < 0.5f ? left : right;
        }

        var results = new List<Transform>();

        switch (count)
        {
            case 1:
                {
                    var chosen = PickOneOfFirst();
                    if (chosen != null) results.Add(chosen);
                    break;
                }
            case 2:
                {
                    float rand2 = UnityEngine.Random.value;
                    if (rand2 < 1f / 3f)
                    {
                        var a = GetPointSafe(_doorSpawnPointsLeft, 0);
                        var b = GetPointSafe(_doorSpawnPointsRight, 0);
                        if (a != null) results.Add(a);
                        if (b != null) results.Add(b);
                    }
                    else if (rand2 < 2f / 3f)
                    {
                        var a = GetPointSafe(_doorSpawnPointsLeft, 1);
                        var b = GetPointSafe(_doorSpawnPointsLeft, 2);
                        if (a != null) results.Add(a);
                        if (b != null && b != a) results.Add(b);
                    }
                    else
                    {
                        var a = GetPointSafe(_doorSpawnPointsRight, 1);
                        var b = GetPointSafe(_doorSpawnPointsRight, 2);
                        if (a != null) results.Add(a);
                        if (b != null && b != a) results.Add(b);
                    }
                    break;
                }
            case 3:
                {
                    float rand3 = UnityEngine.Random.value;
                    if (rand3 < 0.25f)
                    {
                        var a = GetPointSafe(_doorSpawnPointsLeft, 1);
                        var b = GetPointSafe(_doorSpawnPointsLeft, 2);
                        var c = GetPointSafe(_doorSpawnPointsRight, 1);
                        if (a != null) results.Add(a);
                        if (b != null && b != a) results.Add(b);
                        if (c != null && c != a && c != b) results.Add(c);
                    }
                    else if (rand3 < 0.5f)
                    {
                        var a = GetPointSafe(_doorSpawnPointsLeft, 1);
                        var b = GetPointSafe(_doorSpawnPointsLeft, 2);
                        var c = GetPointSafe(_doorSpawnPointsRight, 2);
                        if (a != null) results.Add(a);
                        if (b != null && b != a) results.Add(b);
                        if (c != null && c != a && c != b) results.Add(c);
                    }
                    else if (rand3 < 0.75f)
                    {
                        var a = GetPointSafe(_doorSpawnPointsRight, 1);
                        var b = GetPointSafe(_doorSpawnPointsRight, 2);
                        var c = GetPointSafe(_doorSpawnPointsLeft, 1);
                        if (a != null) results.Add(a);
                        if (b != null && b != a) results.Add(b);
                        if (c != null && c != a && c != b) results.Add(c);
                    }
                    else
                    {
                        var a = GetPointSafe(_doorSpawnPointsRight, 1);
                        var b = GetPointSafe(_doorSpawnPointsRight, 2);
                        var c = GetPointSafe(_doorSpawnPointsLeft, 2);
                        if (a != null) results.Add(a);
                        if (b != null && b != a) results.Add(b);
                        if (c != null && c != a && c != b) results.Add(c);
                    }
                    break;
                }
        }

        return results.ToArray();
    }

    #endregion

    #region Poolable helpers and reward finish

    // Room이 스폰한/소유한 풀 오브젝트 목록을 반환(원래 GetPoolableObjectsInRoom과 호환성 유지)
    public List<GameObject> GetPoolableObjectsInRoom()
    {
        return _poolableObjectsInRoom;
    }

    #endregion
}