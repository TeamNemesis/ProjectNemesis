using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpState
{
    Grounded,       // 착지해 있는 상태, 지면에 닿아 있는 상태, 점프를 할 수 있는 상태
    Jumping,        // 뛰어오르는 중인 상태
    Falling,        // 떨어지는 중인 상태
}

/// <summary>
/// 캐릭터의 점프를 담당하는 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Jumper : MonoBehaviour
{
    [SerializeField] float _jumpPower;      // 점프력
    [SerializeField] LayerMask _groundLayerMask;        // 지면 레이어 마스크
    [SerializeField] Transform _groundChecker;          // 지면 체크용 트랜스폼
    [SerializeField] float _groundCheckRadius;          // 지면 체크 반경

    /// <summary>
    /// 점프 상태 변경 이벤트
    /// </summary>
    public event Action<JumpState> OnStateChanged;

    Rigidbody _rigid;   // 리지드바디 참조
    JumpState _state = JumpState.Grounded;   // 현재 점프 상태

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ChangeState(JumpState.Grounded);
    }

    private void FixedUpdate()
    {
        // Physics.Sphere()
        // 원한느 위치에 가상의 구를 만들어서
        // 겹치는 콜라이더가 있으면 true, 없으면 false를 반환해 주는 함수
        bool isGrounded = Physics.CheckSphere(
            _groundChecker.position,
            _groundCheckRadius,
            _groundLayerMask.value);

        // y 방향 속력
        float velocityY = _rigid.linearVelocity.y;

        // 점프 중 상태인 경우(Jumping)
        if (_state == JumpState.Jumping)
        {
            // 최초로 y 방향 속력이 음수가 되면
            if (velocityY < 0)
            {
                // 추락 중 상태로 변경
                ChangeState(JumpState.Falling);
            }
        }

        // 추락 중 상태인 경우
        else if (_state == JumpState.Falling)
        {
            // 최초로 지면에 닿으면
            if (isGrounded == true)
            {
                // 착지 중 상태로 변경
                ChangeState(JumpState.Grounded);
            }
        }

        // 착지 중 상태인 경우
        else //(_state == JumpState.Grounded)
        {
            // 최초로 지면에 닿지 않게 되면
            if (isGrounded == false)
            {
                // 추락 중 상태로 변경
                ChangeState(JumpState.Falling);
            }
        }
    }

    /// <summary>
    /// 점프 동작을 실행하는 함수
    /// </summary>
    public void Jump()
    {
        // 착지해 있는 상태가 아니면 리턴
        if (_state != JumpState.Grounded) return;

        // y방향 속력 초기화
        Vector3 velocity = _rigid.linearVelocity;
        velocity.y = 0;
        _rigid.linearVelocity = velocity;

        // 점프 힘을 준다.
        _rigid.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);

        // 상태 전환
        ChangeState(JumpState.Jumping);
    }

    void ChangeState(JumpState state)
    {
        // 현재 상태가 새 상태와 동일하면 리턴
        if (_state == state) return;

        if (state == JumpState.Jumping)
        {
            Debug.Log("점프!");
        }
        else if (state == JumpState.Falling)
        {
            Debug.Log("추락시작!");
        }
        else if (state == JumpState.Grounded)
        {
            Debug.Log("착지!");
        }
        _state = state;
        OnStateChanged?.Invoke(_state);
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundChecker == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckRadius);
    }
}
