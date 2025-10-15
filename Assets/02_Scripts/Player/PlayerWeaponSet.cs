using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponSet", menuName = "ScriptableObjects/Player/PlayerWeaponSet")]
public class PlayerWeaponSet : ScriptableObject
{
    [Header("----- 무기가 바뀌면 같이 바뀌는 컴포넌트 -----")]
    [SerializeField] GameObject _weaponPrefab;                 // 무기 프리팹
    [SerializeField] WeaponType _weaponType;                   // 무기 타입
    [SerializeField] PlayerNormalAttacker _normalAttacker;     // 플레이어 일반 공격 컴포넌트
    [SerializeField] PlayerGrenadeAttacker _grenadeAttacker;   // 플레이어 유탄 발사 컴포넌트(얘는 일단 안바뀜)
    [SerializeField] PlayerSpecialAttacker _specialAttacker;   // 플레이어 특수 공격 컴포넌트
    [SerializeField] PlayerAnimator _animator;                 // 플레이어 애니메이터 컴포넌트

    public GameObject WeaponPrefab => _weaponPrefab;
    public WeaponType WeaponType => _weaponType;
    public PlayerNormalAttacker NormalAttacker => _normalAttacker;
    public PlayerGrenadeAttacker GrenadeAttacker => _grenadeAttacker;
    public PlayerSpecialAttacker SpecialAttacker => _specialAttacker;
    public PlayerAnimator Animator => _animator;
}