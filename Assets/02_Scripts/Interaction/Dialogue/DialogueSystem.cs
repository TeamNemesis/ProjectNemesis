using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 대화 시스템 클래스
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogueConfig[] _configs;
    [SerializeField] DialoguePresenter _presenter;

    Dictionary<string, DialogueConfig> _configMap = new();
    bool _isPlaying = false;

    /// <summary>
    /// 대화 중인지 여부 이벤트
    /// </summary>
    public event Action<bool> OnToggled;

    public void Initialize()
    {
        foreach (var config in _configs)
        {
            _configMap[config.Id] = config;
        }

        // 대화 재생 종료 이벤트 구독
        _presenter.OnEnded += EndDialogue;

        // 글로벌 대화 재생 이벤트 구독
        EventBus.OnPlayDialogue += PlayDialogue;
    }

    private void OnDestroy()
    {
        // 반드시 구독 해제
        EventBus.OnPlayDialogue -= PlayDialogue;
    }

    public void PlayDialogue(string id)
    {
        if (_isPlaying == true) return;

        if (_configMap.TryGetValue(id, out DialogueConfig config) == true)
        {
            DialogueModel model = new DialogueModel(config);
            _isPlaying = true;
            _presenter.Play(model);
            OnToggled?.Invoke(_isPlaying);
        }
    }

    public void PlayDialogue(DialogueModel model)
    {
        if(_isPlaying == true) return;
        if(model == null) return;

        _isPlaying = true;
        _presenter.Play(model);
        OnToggled?.Invoke(_isPlaying);
    }

    void EndDialogue()
    {
        _isPlaying = false;
        OnToggled?.Invoke(_isPlaying);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayDialogue("Test");
        }
    }
}
