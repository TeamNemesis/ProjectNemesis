using System;
using UnityEngine;

/// <summary>
/// 플레이어의 화폐(골드, 크롬 등)를 관리하는 매니저 클래스입니다.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    [Header("----- 데이터(임시) -----")]
    [SerializeField] int _startingCredit = 100;
    [SerializeField] int _creditLimitPerRoom = 50;

    [Header("----- 읽기 전용 -----")]
    [SerializeField] int _currentCredit;
    [SerializeField] int _currentChrome;
    [SerializeField] int _roomCredit;       // 이번 방에서 획득한 골드

    public int CurrentGold => _currentCredit;
    public int CurrentChrome => _currentChrome;

    /// <summary>
    /// 골드가 변경되었을 때 발생하는 이벤트입니다.
    /// 매개변수는 변경된 후 최종적으로 갖고있는 현재 골드 양입니다.
    /// </summary>
    public event Action<int> OnCreditChanged;

    /// <summary>
    /// 크롬이 변경되었을 때 발생하는 이벤트입니다.
    /// 매개변수는 변경된 후 최종적으로 갖고있는 현재 크롬 양입니다.
    /// </summary>
    public event Action<int> OnChromeChanged;

    public void Initialize()
    {
        _currentCredit = _startingCredit;
        OnCreditChanged?.Invoke(_currentCredit);
    }

    /// <summary>
    /// 외부에서 현재 화폐 상태를 가져올 수 있도록 업데이트 이벤트를 강제로 발생시킵니다.
    /// </summary>
    public void GetCurrentCurrency()
    {
        OnChromeChanged?.Invoke(_currentChrome);
        OnCreditChanged?.Invoke(_currentCredit);
    }

    /// <summary>
    /// 몬스터 처치로 인한 골드 획득
    /// </summary>
    /// <param name="cost"></param>
    public void AddCreditByMonsterDeath(int cost)
    {
        if(_roomCredit >= _creditLimitPerRoom)
        {
            return;
        }
        if ( _roomCredit + cost > _creditLimitPerRoom)
        {
            cost = _creditLimitPerRoom - _roomCredit;
        }
        _currentCredit += cost;
        OnCreditChanged?.Invoke(_currentCredit);
    }

    public void ResetRoomCredit()
    {
        _roomCredit = 0;
    }

    public void AddCredit(int amount)
    {
        _currentCredit += amount;
        OnCreditChanged?.Invoke(_currentCredit);
    }

    public void AddChrome(int amount)
    {
        _currentChrome += amount;
        OnChromeChanged?.Invoke(_currentChrome);
    }

    public bool TrySpendCredit(int amount)
    {
        if (_currentCredit < amount)
        {
            Debug.Log("돈이 부족합니다.");
            return false;
        }

        _currentCredit -= amount;
        OnCreditChanged?.Invoke(_currentCredit);
        return true;
    }
}