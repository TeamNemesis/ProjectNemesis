using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 다양한 이벤트들의 허브 역할
/// </summary>
public static class EventBus
{
    /// <summary>
    /// 플래그 추가 이벤트
    /// </summary>
    public static event Action<string> OnAddFlag;

    /// <summary>
    /// 플래그 제거 이벤트
    /// </summary>
    public static event Action<string> OnRemoveFlag;

    /// <summary>
    /// 대화 재생 이벤트
    /// </summary>
    public static event Action<DialogueModel> OnPlayDialogue;

    /// <summary>
    /// 플래그를 추가하는 함수
    /// </summary>
    /// <param name="flag"></param>
    public static void AddFlag(string flag)
    {
        OnAddFlag?.Invoke(flag);
    }

    /// <summary>
    /// 플래그를 제거하는 함수
    /// </summary>
    /// <param name="flag"></param>
    public static void RemoveFlag(string flag)
    {
        OnRemoveFlag?.Invoke(flag);
    }

    /// <summary>
    /// 대화를 재생하는 함수
    /// </summary>
    /// <param name="dialogueModel"></param>
    public static void PlayDialogue(DialogueModel dialogueModel)
    {
        OnPlayDialogue?.Invoke(dialogueModel);
    }
}
