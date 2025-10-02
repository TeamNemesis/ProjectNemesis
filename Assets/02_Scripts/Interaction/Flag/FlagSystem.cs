using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Flag를 관리하는 클래스
/// </summary>
public class FlagSystem : MonoBehaviour
{
    // 해쉬셋: 중복 비허용 집합
    HashSet<string> _flags = new HashSet<string>();

    [SerializeField] List<string> _flagsForTest;

    private void Awake()
    {
        // 글로벌 이벤트 구독
        // - 글로벌 이벤트는 반드시 구독 해제를 해야 한다.
        // - 구독 해제하지 않으면 FlagSystem 객체가 Destroy되어도 참조가 유지되어
        // 메모리에 남아 있게 돼 메모리 누수가 발생할 수 있다.
        EventBus.OnAddFlag += AddFlag;
        EventBus.OnRemoveFlag += RemoveFlag;
    }

    private void OnDestroy()
    {
        EventBus.OnAddFlag -= AddFlag;
        EventBus.OnRemoveFlag -= RemoveFlag;
    }

    /// <summary>
    /// 플래그를 추가하는 함수
    /// </summary>
    /// <param name="flag"></param>
    public void AddFlag(string flag)
    {
        _flags.Add(flag);

#if UNITY_EDITOR
        // 에디터 전용
        _flagsForTest = new List<string>(_flags);
#endif
    }

    /// <summary>
    /// 플래그를 제거하는 함수
    /// </summary>
    /// <param name="flag"></param>
    public void RemoveFlag(string flag)
    {
        _flags.Remove(flag);

#if UNITY_EDITOR
        // 에디터 전용
        _flagsForTest = new List<string>(_flags);
#endif
    }

    /// <summary>
    /// 플래그가 현재 있는지 여부를 반환해 주는 함수
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool ContainsFlag(string flag)
    {
        return _flags.Contains(flag);
    }
}
