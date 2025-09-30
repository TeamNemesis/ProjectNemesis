using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라의 줌, 회전을 제어하는 클래스
/// Cinemachine Virtual Camera가 CameraFocus를 따라다니게 한다.
/// </summary>
public class VCameraController : CameraController
{
    [Header("----- 카메라 포커스 -----")]
    [SerializeField] Transform _target;
    [SerializeField] Transform _cameraFocus;
    //[SerializeField] CinemachineCameraOffset _cineCamOffset;

    [Header("----- 카메라 회전 -----")]
    [Tooltip("x축 회전 민감도")]
    [SerializeField] float _pitchSensitivity;
    [Tooltip("y축 회전 민감도")]
    [SerializeField] float _yawSensitivity;

    [SerializeField] float _minPitch;
    [SerializeField] float _maxPitch;

    [SerializeField] float _rotSpeed;

    [Header("----- 카메라 줌 -----")]
    [SerializeField] float _zoomSpeed;
    [SerializeField] float _minZoom;
    [SerializeField] float _maxZoom;

    Vector3 _focusOffset;       // _cameraFocus가 _target으로부터 얼마만큼 떨어져 있는지
    float _pitch;
    float _yaw;

    private void Start()
    {
        _focusOffset = _cameraFocus.position - _target.position;
    }

    private void Update()
    {
        _cameraFocus.position = _target.TransformPoint(_focusOffset);
    }

    public override void Rotate(Vector2 rotInput)
    {
        _pitch -= rotInput.y * _pitchSensitivity;
        _yaw += rotInput.x * _yawSensitivity;

        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
        _cameraFocus.rotation = Quaternion.Slerp(_cameraFocus.rotation, targetRotation, _rotSpeed*Time.deltaTime);
    }

    public override void Zoom(float zoomInput)
    {
        //_cineCamOffset.m_Offset.z += zoomInput * _zoomSpeed;
        //_cineCamOffset.m_Offset.z = Mathf.Clamp(_cineCamOffset.m_Offset.z,
        //    _minZoom, _maxZoom);
    }
}
