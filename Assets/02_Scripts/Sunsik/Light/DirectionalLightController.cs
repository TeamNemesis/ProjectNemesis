using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 라이트 게임오브젝트를 회전시켜 게임의 낮밤 사이클을 추가하는 클래스
/// </summary>
public class DirectionalLightController : MonoBehaviour
{
    // 시간 배속
    [SerializeField] float _timeMultiplier;
    
    // 시작 시 하루 몇 시인가(하루를 정규화해서 사용. 24시간을 1로 보겠다.)
    [SerializeField] float _initialTime;

    float _normalizedTime;
    Vector3 _euler;

    private void Start()
    {
        _normalizedTime = _initialTime;
        _euler = transform.eulerAngles;
    }

    private void Update()
    {
        // 하루는 24시간이고 1시간은 3600초이니까 하루는...
        _normalizedTime += (Time.deltaTime * _timeMultiplier) / 3600 / 24;
        // 24시 지나면 다시 0시부터
        _normalizedTime %= 1;

        // 정규화된 시간에 360도를 곱해서 각도로 변환
        float angle = _normalizedTime * 360.0f;
        // 직접 해보니 Light가 -90도일때 0시처럼 어둡더라
        _euler.x = angle - 90.0f;
        // 라이트의 회전값을 설정
        transform.eulerAngles = _euler;
    }

    //[SerializeField] Light _directionalLight;

    //[SerializeField] float _dayDuration = 60f;

    //private void Update()
    //{
    //    Vector3 rotateSpeed = new Vector3(360f / _dayDuration, 0f, 0f) * Time.deltaTime;
    //    _directionalLight.transform.Rotate(rotateSpeed);
    //}
}
