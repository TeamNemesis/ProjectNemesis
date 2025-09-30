using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 대화를 표시해 주는 뷰
/// </summary>
public class DialogueView : MonoBehaviour
{
    [SerializeField] float _characterSpan;              // 한 글자 출력에 걸리는 시간

    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] TextMeshProUGUI _nameText;         // 캐릭터 이름 텍스트
    [SerializeField] TextMeshProUGUI _speechText;       // 대사 본문 텍스트

    string _speech;             // 현재 출력 중인 문자열
    Coroutine _speechRoutine;   // 출력 코루틴
    bool _isPlaying = false;    // 대사 출력 중인지 여부

    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// 대사 출력을 시작하는 함수
    /// </summary>
    /// <param name="speech"></param>
    public void BeginSpeech(string speech)
    {
        _speech = speech;

        // 코루틴 재생
        _speechRoutine = StartCoroutine(SpeechRoutine());
        Debug.Log("대사 출력 시작");
    }

    /// <summary>
    /// 대사 출력을 종료하는 함수
    /// </summary>
    public void EndSpeech()
    {
        // 코루틴 정지
        if (_speechRoutine != null)
        {
            StopCoroutine(_speechRoutine);
            _speechRoutine = null;
        }

        _isPlaying = false;
        _speechText.text = _speech;
        Debug.Log("대사 출력 끝");
    }

    /// <summary>
    /// 대사 한 줄 출력 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SpeechRoutine()
    {
        _isPlaying = true;
        // Time.timescale에 영향을 받는 WaitforSeconds
        //WaitForSeconds waitForSeconds = new WaitForSeconds(_characterSpan);

        // Time.timeScale에 영향을 안 받는 WaitForSeconds
        WaitForSecondsRealtime waitForSeconds = new WaitForSecondsRealtime(_characterSpan);
        for (int i = 1; i <= _speech.Length; i++)
        {
            _speechText.text = _speech.Substring(0, i);
            yield return waitForSeconds;
        }
        _isPlaying = false;
    }

    /// <summary>
    /// 이름 텍스트 설정하는 함수
    /// </summary>
    /// <param name="name"></param>
    public void SetNameText(string name)
    {
        _nameText.text = name;
    }
}
