using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public enum PathFollowingType
{
    Loop,       // 0번 -> 끝번 -> 0번 -> 끝번 ...
    PingPong

}

/// <summary>
/// 지정된 경로(웨이포인트)를 따라 순환 이동하는 클래스
/// </summary>
public class PathFollower : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] NavMeshAgent _agent;

    [Header("----- 런타임 데이터 -----")]
    [SerializeField] List<Transform> _waypoints = new();
    [SerializeField] PathFollowingType _follwingType = PathFollowingType.Loop;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _arrivalThreshold;    //  도착 감지 임계값

    int _currentIndex = 0;
    int _direction = 1;     // Loop: 1, PingPong: 1 or -1   

    private void Start()
    {
        _agent.speed = _moveSpeed;
    }

    private void Update()
    {
        if (_waypoints.Count <= 2 && _agent.isStopped == true) return;

        //if (transform.position = _waypoints[_currentIndex].posotion)
        // 불가능, 흔히 하는 실수
        // -> float 값은 정확한 값이 아니기 때문에
        // 거이ㅡ 비슷할 순 있어도 완전히 똑같기는 힘들다.
;
        float distance = Vector3.Distance(transform.position, _waypoints[_currentIndex].position);
        if (distance < _arrivalThreshold)       // 도착한 것으로 판단
        {
            AdvanceToNextWaypoint();
        }

    }

    /// <summary>
    /// 웨이포인트로 향하는 함수
    /// </summary>
    /// <param name="index">웨이포인트 순번</param>
    void MoveToWaypoint(int index)
    {
        if (index < 0 || index >= _waypoints.Count) return;
        
        _agent.SetDestination(_waypoints[index].position);
    }

    /// <summary>
    /// 다음 웨이포인트로 향하는 함수
    /// </summary>
    void AdvanceToNextWaypoint()
    {
        switch (_follwingType)
        {
            case PathFollowingType.Loop:
                _currentIndex = (_currentIndex +1) % _waypoints.Count;
                break;
            case PathFollowingType.PingPong:
                _currentIndex += _direction;

                if(_currentIndex >= _waypoints.Count)
                {
                    _direction = -1; // 방향을 반대로
                    _currentIndex = _waypoints.Count - 2;
                }
                else if (_currentIndex < 0)
                {
                    _direction = 1; // 방향을 반대로
                    _currentIndex = 1;
                }
                break;
            default:
                break;
        }

        MoveToWaypoint(_currentIndex);
    }

    /// <summary>
    /// 이동을 시작하는 함수
    /// </summary>
    public void StartFollowing()
    {
        _agent.isStopped = false;
        MoveToWaypoint(_currentIndex);
    }

    /// <summary>
    /// 이동을 중단하는 함수
    /// </summary>
    public void StopFollowing()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
    }
}