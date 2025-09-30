using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class RigidbodyMover : Mover
{
    public override event Action<Vector3> OnMoved;

    Rigidbody _rigid;       // 리지드바디 참조
    Vector3 _velocity;      // 이동 속도 벡터
    Quaternion _targetRotation; // 현재 목표 회전값

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _targetRotation = _rigid.rotation;
    }

    private void FixedUpdate()
    {
        // 리지드바디의 y 방향 속력값 그대로 유지
        _velocity.y = _rigid.linearVelocity.y;

        // 리지드바디에 원하는 속도 벡터 적용
        _rigid.linearVelocity = _velocity;

        // 리지드바디에 목표 회전값에 따른 부드러운 회전 적용
        _rigid.rotation = Quaternion.RotateTowards(
            _rigid.rotation,
            _targetRotation,
            _rotSpeed * Time.fixedDeltaTime);

        // 이동 이벤트 발행
        _velocity.y = 0;
        OnMoved?.Invoke(_velocity);
    }

    public override void Move(Vector3 direction)
    {
        // y 방향 이동은 무시
        direction.y = 0;

        // 이동 방향의 크기가 거의 0이면
        //if (direction.magnitude < Util.Epsilon)
        if (direction.magnitude < 0.1f)
        {
            // 현재 속도를 0으로 설정
            _velocity = Vector3.zero;
        }
        else
        {
            // 현재 속도값 설정
            _velocity = direction.normalized * _moveSpeed;

            _targetRotation = Quaternion.LookRotation(direction);
        }
    }
}