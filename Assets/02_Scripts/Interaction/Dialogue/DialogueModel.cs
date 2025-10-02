using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대사 런타임 데이터 모델
/// </summary>
public class DialogueModel
{
    DialogueConfig _config;
    string[] _lines;

    /// <summary>
    /// 대화 종료 이벤트
    /// </summary>
    public event Action OnEnded;

    public DialogueModel(DialogueConfig config)
    {
        _config = config;

        // Split('\n');
        // '\n' 개행 문자를 기준으로 전체 문자열을 배열로 만들어 주는 함수
        _lines = _config.Content.Split('\n');
    }

    /// <summary>
    /// 대사의 한 줄을 반환해 주는 함수
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string Getline(int index)
    {
        if (index < 0 || index >= _lines.Length) return null;

        return _lines[index];
    }

    /// <summary>
    /// 대화 종료 이벤트를 발행하는 함수
    /// </summary>
    public void InvokeEnded()
    {
        OnEnded?.Invoke();
    }
}
