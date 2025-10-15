using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 프리팹 리소스를 관리하는 매니저 클래스
/// 게임 시작시 필요한 리소스를 미리 로드하여 관리
/// </summary>
public class ResourceManager : MonoBehaviour
{
    [Header("----- 플레이어 무기 세트 -----")]
    [SerializeField] PlayerWeaponSet[] _playerWeaponSets; // 플레이어 컴포넌트 세트들

    Dictionary<WeaponType, PlayerWeaponSet> _playerWeaponSetMap = new Dictionary<WeaponType, PlayerWeaponSet>();

    public Dictionary<WeaponType, PlayerWeaponSet> PlayerWeaponSetMap => _playerWeaponSetMap;


    public void Initialize()
    {
        _playerWeaponSets = Resources.LoadAll<PlayerWeaponSet>("ScriptableObjects/Player/PlayerWeaponSets");

        InitializePlayerWeaponSetMap();
    }

    void InitializePlayerWeaponSetMap()
    {
        _playerWeaponSetMap.Clear();

        foreach (var set in _playerWeaponSets)
        {
            if (!_playerWeaponSetMap.ContainsKey(set.WeaponType))
            {
                _playerWeaponSetMap.Add(set.WeaponType, set);
            }
            else
            {
                Debug.LogWarning($"이미 {set.WeaponType} 타입의 무기 세트가 존재합니다.");
            }
        }
    }
}