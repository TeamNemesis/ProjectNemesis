using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraController : CameraController
{
    [Header("----- 카메라 포커스 -----")]
    [SerializeField] Transform _target;     // 카메라 포커스가 추적할 타겟
    [SerializeField] Transform _cameraFocus;    // 카메라 포커스

    [Header("----- 이동 -----")]
    [SerializeField] float _followingSpeed;

    [Header("----- 카메라 회전 -----")]
    [Tooltip("x축 회전 민감도")]
    [SerializeField] float _pitchSensitivity;
    [Tooltip("y축 회전 민감도")]
    [SerializeField] float _yawSensitivity;

    [SerializeField] float _minPitch;       // x축 회전 최소값
    [SerializeField] float _maxPitch;       // x축 회전 최대값

    [SerializeField] float _rotRate;        // 목표 회전을 따라가는 비율 값

    [Header("----- 카메라 줌 -----")]
    [SerializeField] float _zoomRate;       // 줌 비율
    [SerializeField] float _minDistance;    // 카메라의 카메라 포커스와의 최소 거리
    [SerializeField] float _maxDistance;    // 카메라의 카메라 포커스와의 최대 거리

    Vector3 _focusOffset;   // 카메라 포커스가 타겟으로부터 얼마만큼 떨어져 있는지
    float _pitch;           // 카메라의 x축 회전 값
    float _yaw;             // 카메라의 y축 회전 값
    float _distance;        // 카메라의 카메라 포커스와의 거리

    private void Start()
    {
        _pitch = transform.eulerAngles.x;
        _yaw = transform.eulerAngles.y;

        _focusOffset = _cameraFocus.position - _target.position;

        _distance = Vector3.Distance(_cameraFocus.position, transform.position);
    }

    private void Update()
    {
        // 카메라 포커스(_cameraFocus)가 타겟(_target)을
        // _focusOffset만큼 떨어진 채로 계속 따라다니게 하는 코드
        //_cameraFocus.position = _target.TransformPoint(_focusOffset);

        // 카메라 포커스가 타겟을 일정한 속력으로 추적하게 하는 방식
        // 카메라가 주인공 캐릭터를 따라가게 하기에는 적절하지 않은 방식
        //    _cameraFocus.position =
        //        Vector3.MoveTowards(_cameraFocus.position,
        //        _target.TransformPoint(_focusOffset),
        //        Time.deltaTime * _followingSpeed);

        // MoveTowards()는 일정한 속력으로 이동할 때 활용
        // Lerp()는 0 ~ 1 사이의 비율
        // 처음에는 빠르다가 갈수록 천천히 접근
        _cameraFocus.position = Vector3.Lerp(_cameraFocus.position,
            _target.TransformPoint(_focusOffset),
            Time.deltaTime * _followingSpeed);   // 진짜 속력은 아니다.
    }

    public override void Rotate(Vector2 rotInput)
    {
        // x축 회전값 설정
        _pitch -= rotInput.y * _pitchSensitivity;

        // y축 회전값 설정
        _yaw += rotInput.x * _yawSensitivity;

        // x축 회전값 구간 내 고정
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
    }

    public override void Zoom(float zoomInput)
    {
        // 카메라의 거리값 설정
        _distance -= zoomInput * _zoomRate;

        // 카메라의 거리값을 구간 내 고정
        _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
    }

    // 모든 Update()가 끝난 다음에 늦게 실행되는 Update()
    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // 목표 회전값 설정
        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0);

        // 카메라의 회전값에 목표 회전값 적용
        //transform.rotation = targetRotation;

        // 카메라의 회전값을 부드럽게 처리
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * _rotRate);

        // 카메라의 카메라 포커스로부터 떨어진 위치 결정
        Vector3 cameraOffset = -transform.forward * _distance;

        // 카메라의 위치값 설정
        transform.position = _cameraFocus.position + cameraOffset;
    }
}
