using System;
using UnityEngine;

/// <summary>
/// 플레이어의 화폐(골드, 크롬 등)를 관리하는 매니저 클래스입니다.
/// </summary>
public class CurrenyManager : MonoBehaviour
{
    [Header("----- 데이터(임시) -----")]
    [SerializeField] float _startingGold = 100f;

    [Header("----- 읽기 전용 -----")]
    [SerializeField] float _currentGold;
    [SerializeField] float _currentChrome;

    public float CurrentGold => _currentGold;
    public float CurrentChrome => _currentChrome;

    /// <summary>
    /// 골드가 변경되었을 때 발생하는 이벤트입니다.
    /// 매개변수는 변경된 후 최종적으로 갖고있는 현재 골드 양입니다.
    /// </summary>
    public event Action<float> OnGoldChanged;

    /// <summary>
    /// 크롬이 변경되었을 때 발생하는 이벤트입니다.
    /// 매개변수는 변경된 후 최종적으로 갖고있는 현재 크롬 양입니다.
    /// </summary>
    public event Action<float> OnChromeChanged;

    public void Initialize()
    {
        _currentGold = _startingGold;
        OnGoldChanged?.Invoke(_currentGold);
    }

    public void AddGold(float amount)
    {
        _currentGold += amount;
        OnGoldChanged?.Invoke(_currentGold);
    }

    public void AddChrome(float amount)
    {
        _currentChrome += amount;
        OnChromeChanged?.Invoke(_currentChrome);
    }
}