using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대사 설정 데이터
/// </summary>
[CreateAssetMenu(fileName = "DialogueConfig", menuName = "GameSettings/Dialogue")]
public class DialogueConfig : ScriptableObject
{
    [SerializeField] string _id;        // 대사 고유번호
    [SerializeField] int _priority;     // 대화 우선순위
    [SerializeField] string _requiredFlag;  // 필요한 플래그
    [SerializeField] string _hiddenFlag;    // 없어야 하는 플래그, 특정 조건을 만족했더라도 이 플래그가 있으면 대화가 안뜨게 하기 위해 필요하다.
    [SerializeField][TextArea(5, 10)] string _content;      // 대화 내용

    public string Id => _id;
    public int Priority => _priority;
    public string RequiredFlag => _requiredFlag;
    public string HiddenFlag => _hiddenFlag;
    public string Content => _content;
}
