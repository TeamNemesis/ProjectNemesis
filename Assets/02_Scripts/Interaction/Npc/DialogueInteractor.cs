using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 대화 상호작용 대상 클래스
/// </summary>
public class DialogueInteractor : MonoBehaviour, IInteractable
{
    FlagSystem _flagSystem;

    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Transform _interactionGuidePoint;

    [Header("----- 설정 데이터 -----")]
    [SerializeField] DialogueConfig[] _configs;     // 대화 설정 데이터 배열

    List<DialogueConfig> _sortedConfigs;             // 우선순위 내림차순으로 정렬된 대화 설정 데이터 리스트

    DialogueModel _currentDialogue;

    /// <summary>
    /// 상호작용 시작 이벤트
    /// </summary>
    public event Action<Transform> OnBegun;

    /// <summary>
    /// 상호작용 종료 이벤트
    /// </summary>
    public event Action OnEnded;

    public Vector3 GuidePoint => _interactionGuidePoint.position;

    private void Awake()
    {
        // configs에 들어 있는 config를 config.Priority 내림차순으로 정렬
        _sortedConfigs = _configs.OrderByDescending(config => config.Priority).ToList();
    }

    private void Start()
    {
        _flagSystem = FindAnyObjectByType<FlagSystem>();
    }

    /// <summary>
    /// 상호작용을 실행하는 함수
    /// </summary>
    public void Interact(Transform subject)
    {
        foreach (var config in _sortedConfigs)
        {
            // 필요한 플래그 조건 통과 여부
            // 1) 대화 설정 데이터 자체가 필요 플래그가 없는 경우거나
            // 2) 대화 설정 데이터의 필요 플래그가 커져 있는 경우
            bool requiredPassed = string.IsNullOrEmpty(config.RequiredFlag) ||
                _flagSystem.ContainsFlag(config.RequiredFlag);

            // 없어야 하는 플래그 조건 통과 여부
            // 1) 대화 설정 데이터 자체가 숨김 플래그가 없는 경우거나
            // 2) 대화 설정 데이터의 숨김 플래그가 꺼져 있는 경우
            bool hiddenPassed = string.IsNullOrEmpty(config.HiddenFlag) ||
                (_flagSystem.ContainsFlag(config.HiddenFlag) == false);

            if (requiredPassed && hiddenPassed)
            {
                // DialogueModel 생성
                _currentDialogue = new DialogueModel(config);

                // 대화 종료 이벤트 구독
                _currentDialogue.OnEnded += OnDialogueEnded;

                // 상호작용 시작 이벤트 발행
                OnBegun?.Invoke(subject);

                // 대화 재생 이벤트 발행
                EventBus.PlayDialogue(_currentDialogue);
                return;
            }
        }
    }

    // 1. 대화 시작/종료 이벤트 구독
    // 2. 내가 시작한 대화의 종료 이벤트 구독

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F11))
        //{
        //    Interact();
        //}
    }

    void OnDialogueEnded()
    {
        // 현재 대화 종료 이벤트 구독 해제
        _currentDialogue.OnEnded -= OnDialogueEnded;

        _currentDialogue = null;
        OnEnded?.Invoke();
    }
}
