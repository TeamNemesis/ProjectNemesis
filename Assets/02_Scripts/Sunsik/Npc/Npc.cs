using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] DialogueInteractor _interactor;
    [SerializeField] PathFollower _pathFollower;

    [Header("----- 런타임 데이터----- ")]
    [SerializeField] float _rotSpeed;

    Coroutine _rotateTowardRoutine;



    private void Start()
    {
        _interactor.OnBegun += OnInteractionBegun;
        _interactor.OnEnded += OnInteractionEnded;

        _pathFollower.StartFollowing();

        int childCount = transform.childCount;
        //Debug.Log("자식 개수: " + childCount);

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            //Debug.Log("자식 이름: " + child.name);
        }

        // foreach도 사용 가능
        foreach (Transform child in transform)
        {
            //Debug.Log("foreach 자식 이름: " + child.name);
        }
    }

    /// <summary>
    /// 상호작용 시작 시 자동으로 호출되는 함수
    /// </summary>
    void OnInteractionBegun(Transform subject)
    {
        _pathFollower.StopFollowing();
        _rotateTowardRoutine = StartCoroutine(RotateTowardRoutine(subject));
        Debug.Log("Npc의 상호작용 시작");
    }

    /// <summary>
    /// 상호작용 종료 시 자동으로 호출되는 함수
    /// </summary>
    void OnInteractionEnded()
    {
        if(_rotateTowardRoutine != null)
        {
            StopCoroutine(_rotateTowardRoutine);
            _rotateTowardRoutine = null;
        }

        _pathFollower.StartFollowing();
    }
    
    IEnumerator RotateTowardRoutine(Transform target)
    {
        // 바라볼 방향 벡터 계산
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if(direction == Vector3.zero) yield break;

        // 목표 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        // 목표 회전값과 자신이ㅡ 회전값 사이의 각도가 거의 0이 아닌 동안
        while (angle > Util.Epsilon)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotSpeed * Time.deltaTime);
            angle = Quaternion.Angle(transform.rotation, targetRotation);
            yield return 0;
        }
    }
}
