using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMover : Mover
{
    public override event Action<Vector3> OnMoved;

    private CharacterController _controller;
    private Vector3 _velocity;          // 이동 벡터
    private Quaternion _targetRotation; // 목표 회전
    private float _gravity = -9.81f;    // 중력 값

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _targetRotation = transform.rotation;
    }

    private void Update()
    {
        Vector3 move = _velocity * Time.deltaTime;

        // 중력 적용
        if (!_controller.isGrounded)
            move.y += _gravity * Time.deltaTime;

        // 이동
        _controller.Move(move);

        // 부드러운 회전
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _targetRotation,
            _rotSpeed * Time.deltaTime);

        // 이동 이벤트 발행 (y 제외)
        Vector3 horizontalVelocity = _velocity;
        horizontalVelocity.y = 0;
        OnMoved?.Invoke(horizontalVelocity);
    }

    public override void Move(Vector3 direction)
    {
        direction.y = 0;

        // 너무 작은 방향 벡터가 입력되었으면 무시
        if (direction.magnitude < 0.1f)
        {
            _velocity = Vector3.zero;
        }
        else
        {
            // 방향 벡터 정규화 후 이동 속도를 곱해서 속력으로 설정
            _velocity = direction.normalized * _moveSpeed;
            // 목표 회전방향 설정
            _targetRotation = Quaternion.LookRotation(direction);
        }
    }
}
