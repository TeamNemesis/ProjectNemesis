using UnityEngine;

/// <summary>
/// 플레이어의 무기 타입을 정의하는 열거형
/// </summary>
public enum WeaponType
{
    None,
    Blade,
    Rifle,
    HackingDevice,
}

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponType _weaponType;
    public WeaponType WeaponType => _weaponType;

    public void Initialize()
    {
        // 무기 장착 시 이펙트나 사운드 등 초기화 작업 수행
        //Debug.Log($"{_weaponType} 무기 초기화 완료");
    }
}
