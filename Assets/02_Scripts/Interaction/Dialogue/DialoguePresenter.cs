using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineCommandType
{
    Speech = -1,
    Name,
    FlagOn,
    FlagOff
}

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] string[] _commandWords;        // 명령어 배열
    [SerializeField] DialogueView _view;

    DialogueModel _model;       // 현재 재생 중인 대화 모델
    int _lineIndex = 0;         // 현재 재생할 대사 줄 번호

    /// <summary>
    /// 대화 재생 종료 이벤트
    /// </summary>
    public event Action OnEnded;

    /// <summary>
    /// 대화를 시작하는 함수
    /// </summary>
    /// <param name="model"></param>
    public void Play(DialogueModel model)
    {
        _model = model;

        _view.gameObject.SetActive(true);

        _lineIndex = 0;
        _view.SetNameText(string.Empty);
        Debug.Log("대화 창 띄움");

        PlayCurrentLine();
    }

    /// <summary>
    /// 대화를 끝내는 함수
    /// </summary>
    public void Stop()
    {
        _view.gameObject.SetActive(false);
        Debug.Log("대화 창 닫음");
        _model.InvokeEnded();
        OnEnded?.Invoke();
    }

    /// <summary>
    /// 대화 다음 줄로 넘어가는 함수
    /// </summary>
    public void Next()
    {
        // 뷰가 아직 대사를 출력 중이면
        if (_view.IsPlaying == true)
        {
            _view.EndSpeech();
        }
        // 뷰가 대사 출력을 다 했으면
        else
        {
            _lineIndex++;
            PlayCurrentLine();
        }
    }

    /// <summary>
    /// 현재 대사 줄을 재생하는 함수
    /// </summary>
    void PlayCurrentLine()
    {
        string line = _model.Getline(_lineIndex);
        if (string.IsNullOrEmpty(line) == false)
        {
            LineCommandType commandType = ParseLine(line, out string str);
            switch (commandType)
            {
                case LineCommandType.Name:
                    _view.SetNameText(str);
                    break;
                case LineCommandType.FlagOn:
                    EventBus.AddFlag(str);
                    break;
                    case LineCommandType.FlagOff:
                    EventBus.RemoveFlag(str);
                    break;
                default:
                    _view.BeginSpeech(str);
                    break;
            }

            if (commandType != LineCommandType.Speech)
            {
                _lineIndex++;
                PlayCurrentLine();
            }
        }
        else
        {
            // 대화 종료
            Stop();
        }
    }

    /// <summary>
    /// 대사 한 줄의 명령어와 내용을 반환해 주는 함수
    /// </summary>
    /// <param name="line">해독할 한 줄</param>
    /// <param name="str">명령어 내용</param>
    /// <returns>명령어</returns>
    LineCommandType ParseLine(string line, out string str)
    {
        str = line;

        for (int i = 0; i < _commandWords.Length; i++)
        {
            // 대사 한 줄이 명령어로 시작하는 경우
            if (line.StartsWith(_commandWords[i]) == true)
            {
                // 대사 한 줄이 명령어와 동일한 경우
                if (line.Length == _commandWords[i].Length)
                {
                    str = string.Empty;
                }
                // 대사 한 줄이 명령어와 동일하지 않은 경우
                else
                {
                    str = line.Substring(_commandWords[i].Length);
                }
                return (LineCommandType)i;
            }
        }
        return LineCommandType.Speech;
    }
}
