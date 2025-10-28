using System;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    #region 공격력 

    #region 검
    /// <summary>
    /// 검 일반 공격력
    /// </summary>
    private float _bladeAttackDamage;
    public float bladeAttackDamage { get { return _bladeAttackDamage; } }
    public void AddBladeAttackDamage(float plusBladeAttack)
    {
        _bladeAttackDamage += plusBladeAttack;
    }


    /// <summary>
    /// 검 특수 공격력
    /// </summary>
    private float _bladeSPAttackDamage;
    public float bladeSPAttackDamage { get { return _bladeAttackDamage; } }
    public void AddBladeSPAttackDamage(float plusBladeSPAttack)
    {
        _bladeSPAttackDamage += plusBladeSPAttack;
    }

    #endregion
    #region 라이플
    /// <summary>
    /// 원거리 일반 공격력
    /// </summary>
    private float _playerRifleAttackDamage;
    public float playerRifleAttackDamage { get { return _playerRifleAttackDamage; } }
    public void AddPlayerRifleAttackDamage(float plusPlayerRifleAttack)
    {
        _playerRifleAttackDamage += plusPlayerRifleAttack;
    }

    /// <summary>
    /// 원거리 특수 공격력
    /// </summary>
    private float _playerRifleSPAttackDamage;
    public float playerRifleSPAttackDamage { get { return _playerRifleSPAttackDamage; } }
    public void AddPlayerRifleSPAttackDamage(float plusPlayerRifleSPAttackDamage)
    {
        _playerRifleSPAttackDamage += plusPlayerRifleSPAttackDamage;
    }

    /// <summary>
    /// 플레이어 블렛 이동 속도
    /// </summary>
    private float _playerBulletMoveSpeed;
    public float playerBulletMoveSpeed { get { return _playerBulletMoveSpeed; } }
    public void AddPlayerBulletMoveSpeed(float plusPlayerBulletMoveSpeed)
    {
        _playerBulletMoveSpeed += plusPlayerBulletMoveSpeed;
    }

    /// <summary>
    /// 플레이어 블렛 지속 시간
    /// </summary>
    private float _playerBulletLifeTime;
    public float playerBulletLifeTime { get { return _playerBulletLifeTime; } }
    public void AddPlayerBulletLifeTime(float plusPlayerBulletLifeTime)
    {
        _playerBulletLifeTime += plusPlayerBulletLifeTime;
    }

    #endregion
    #region 특수장비
    /// <summary>
    /// 특수장비 일반 공격력
    /// </summary>
    private float _playerHackAttackDamage;
    public float playerHackAttackDamage { get { return _playerHackAttackDamage; } }
    public void AddPlayerHackAttackDamage(float plusPlayerHackAttackDamage)
    {
        _playerHackAttackDamage += plusPlayerHackAttackDamage;
    }

    /// <summary>
    /// 특수장비 특수 공격력
    /// </summary>
    private float _playerHackSPAttackDamage;
    public float playerHackSPAttackDamage { get { return _playerHackSPAttackDamage; } }
    public void AddPlayerHackSPAttackDamage(float plusPlayerHackSPAttackDamage)
    {
        _playerHackSPAttackDamage += plusPlayerHackSPAttackDamage;
    }
    #endregion
    /// <summary>
    /// 플레이어 일반 공격 데미지 계수
    /// </summary>
    private float _playerAttackDamage = 1f;
    public float playerAttackDamage { get { return _playerAttackDamage; } }
    public void AddPlayerAttackDamage(float plusDamage)
    {
        _playerAttackDamage += plusDamage;
        OnPlayerAttackDamageChange?.Invoke();
    }
    public event Action OnPlayerAttackDamageChange;



    /// <summary>
    /// 플레이어 유탄 공격 데미지 계수
    /// </summary>
    private float _playerGrenadeDamageMulti;
    public float playerGrenadeDamageMulti { get { return _playerGrenadeDamageMulti; } }
    public void AddPlayerGrenadeDamageMulti(float plusGrenadeDamage)
    {
        _playerGrenadeDamageMulti += plusGrenadeDamage;
    }
  

    /// <summary>
    /// 플레이어 특수 공격 데미지 계수
    /// </summary>
    private float _playerSPAttackDamage;
    public float playerSPAttackDamage { get { return _playerSPAttackDamage; } }
    public void AddPlayerSPAttackDamage(float plusSPDamage)
    {
        _playerSPAttackDamage += plusSPDamage;
    }

    /// <summary>
    /// 플레이어 대쉬 공격력
    /// </summary>
    private float _playerDashDamage;
    public float playerDashDamage { get { return _playerDashDamage; } }
    public void AddPlayerDashDamage(float plusDashDamage)
    {
        _playerDashDamage += plusDashDamage;
    }

    /// <summary>
    /// 플레이어 대쉬 공격력계수
    /// </summary>
    private float _playerDashDamageMulti;
    public float playerDashDamageMulti { get { return _playerDashDamageMulti; } }
    public void AddPlayerDashDamageMulti(float plusDashDamage)
    {
        _playerDashDamageMulti += plusDashDamage;
    }

    #endregion

    #region 모든 데미지, 이동속도, 애니메이션 재생속도

    /// <summary>
    /// 모든 데미지 증가 계수
    /// </summary>
    private float _totalMultiDamage = 0f;
    public float totalMultiDamage { get { return _totalMultiDamage; } }

    /// <summary>
    /// 모든 데미지 계수 증가
    /// </summary>
    /// <param name="addDamage">ex)0.5배</param>
    public void AddTotalMultiDamage(float addDamage)
    {
        _totalMultiDamage += addDamage;
    }
    /// <summary>
    /// 플레이어 이동 속도 계수
    /// </summary>
    private float _playerMoveSpeed;
    public float playerMoveSpeed { get { return _playerMoveSpeed; } }
    public void AddPlayerMoveSpeed(float plusMoveSpeed)
    {
        _playerMoveSpeed += plusMoveSpeed;
        OnPlayerMoveSpeedChange?.Invoke();
    }
    public event Action OnPlayerMoveSpeedChange;




    /// <summary>
    /// 플레이어 대쉬 거리
    /// </summary>
    private float _playerDashDistance = 5f;
    public float playerDashDistance { get { return _playerDashDistance; } }
    public void AddPlayerDashDistance(float plusDashDistance)
    {
        _playerDashDistance += plusDashDistance;
    }

    /// <summary>
    /// 플레이어 대쉬거리 곱 계수
    /// </summary>
    private float _playerDashDistanceMulti;
    public float playerDashDistanceMulti { get { return _playerDashDistanceMulti; } }
    public void AddPlayerDashDistanceMulti(float plusDashDistanceMulti)
    {
        _playerDashDistanceMulti += plusDashDistanceMulti;
        OnPlayerDashDistanceMultiChange?.Invoke(_playerDashDistanceMulti);
    }
    public event Action<float> OnPlayerDashDistanceMultiChange;
    /// <summary>
    /// 플레이어 애니메이션 일반 공격 재생 속도
    /// </summary>
    private float _playerAttackAnimSpeed;
    public float playerAttackAnimSpeed { get { return _playerAttackAnimSpeed; } }
    public void AddPlayerAttackAnimSpeed(float plusAttackAnimSpeed)
    {
        _playerAttackAnimSpeed += plusAttackAnimSpeed;
    }


    #endregion

    #region 유탄, 범위공격

    /// <summary>
    /// 유탄 공격력
    /// </summary>
    private float _playerGrenadeDamage;
    public float playerGrenadeDamage { get { return _playerGrenadeDamage; } }
    public void AddPlayerGreneadeDamage(float plusGreneadeDamage)
    {
        _playerGrenadeDamage += plusGreneadeDamage;
    }

    /// <summary>
    /// 플레이어 범위 공격 범위 계수
    /// </summary>
    private float _playerAreaExtent = 1f;
    public float playerAreaExtent { get { return _playerAreaExtent; } }
    public void AddPlayerAreaExtent(float plusAreaExtent)
    {
        _playerAreaExtent += plusAreaExtent;
        OnAreaExtentChange?.Invoke(_playerAreaExtent);
    }
    public event Action<float> OnAreaExtentChange;

    /// <summary>
    /// 유탄 쿨타임 
    /// </summary>
    private float _grenadeCoolTime;
    public float grenadeCoolTime { get { return _grenadeCoolTime; } }
    public void SetGrenadeCoolTime(float grenadeCoolTime)
    {
        _grenadeCoolTime = grenadeCoolTime;
        OnGrenadeCoolTimeChange?.Invoke(_grenadeCoolTime);
    }
    public event Action<float> OnGrenadeCoolTimeChange;



    /// <summary>
    /// 유탄 쿨타임 계수
    /// </summary>
    private float _grenadeCoolTimeMulti;
    public float grenadeCoolTimeMulti { get { return _grenadeCoolTimeMulti; } }
    public void AddGrenadeCoolTimeMulti(float grenadeCoolTime)
    {
        _grenadeCoolTimeMulti += grenadeCoolTime;
        OnGrenadeCoolTimeMultiChange?.Invoke(_grenadeCoolTimeMulti);
    }
    public event Action<float> OnGrenadeCoolTimeMultiChange;

    #endregion

    #region 받는 피해
    /// <summary>
    /// 플레이어 받는 피해 계수
    /// </summary>
    private float _playerReduceDamagePercent = 1f;
    public float playerReduceDamagePercent { get { return _playerReduceDamagePercent; } }
    public void AddReduceDamagePercent(float plusReduceDamagePercent)
    {
        _playerReduceDamagePercent += plusReduceDamagePercent;
        OnplayerHitPercentChange?.Invoke(_playerReduceDamagePercent);
    }
    public event Action<float> OnplayerHitPercentChange;

    /// <summary>
    /// 플레이어 회피율
    /// </summary>
    private float _playerAvoidance= 0f;
    public float playerAvoidance { get { return _playerAvoidance; } }
    public void AddPlayerAvoidance(float plusPlayerAvoidance)
    {
        _playerAvoidance += plusPlayerAvoidance;
        OnplayerAvoidanceChange?.Invoke(_playerAvoidance);
    }
    public event Action<float> OnplayerAvoidanceChange;
    #endregion

    #region 넉백

    /// <summary>
    /// 넉백 충돌 데미지
    /// </summary>
    private float _knockBackDamage = 60f;
    public float knockBackDamage { get { return _knockBackDamage; } }
    public void AddKockBackDamage(float plusKnockBackDamage)
    {
        _knockBackDamage += plusKnockBackDamage;
    }

    /// <summary>
    /// 넉백 충돌 데미지 계수 
    /// </summary>
    private float _knockBackDamageMulti = 1f;
    public float knockBackDamageMulti { get { return _knockBackDamageMulti; } }
    public void AddKockBackDamageMulti(float plusKnockBackDamageMulti)
    {
        _knockBackDamageMulti += plusKnockBackDamageMulti;
        OnKnockBackDamgeMultiChange?.Invoke(_knockBackDamageMulti);
    }
    public event Action<float> OnKnockBackDamgeMultiChange;

    /// <summary>
    /// 넉백 거리 계수
    /// </summary>
    private float _knockBackDistance;
    public float knockBackDistance { get { return _knockBackDistance; } }
    public void AddKnockBackDistance(float plusKnockBackDistance)
    {
        _knockBackDistance += plusKnockBackDistance;
        OnKnockBackDistanceChange?.Invoke(_knockBackDistance);
    }
    public event Action<float> OnKnockBackDistanceChange;

    /// <summary>
    /// 넉백 미는 힘
    /// </summary>
    private float _knockBackPower;
    public float knockBackPower { get { return _knockBackPower; } }
    public void AddKnockBackPower(float plusKnockBackPower)
    {
        _knockBackPower += plusKnockBackPower;
    }
    #endregion

    #region 추가데미지
    /// <summary>
    /// 취약, 디버프 있으면 추가 데미지 계수
    /// </summary>
    private float _debuffPlusDamage;
    public float debuffPlusDamage { get { return _debuffPlusDamage; } }
    public void AddDebuffPlusDamage(float plusDebuffPlusDamage)
    {
        _debuffPlusDamage += plusDebuffPlusDamage;
    }

    /// <summary>
    /// 약화 추가 데미지 계수
    /// </summary>
    private float _weakenPlusDamage = 0f;
    public float weakenPlusDamage { get { return _weakenPlusDamage; } }
    public void AddWeakenPlusDamage(float PlusWeakenDamage)
    {
        _weakenPlusDamage += PlusWeakenDamage;
    }
    #endregion

    

    public void TakeDamage(WeaponType weaponType,ATTACKTYPE attackType,Transform monster, Transform attackerTransform)
    {
        float damage = 0f;
        switch (attackType)
        {
            case ATTACKTYPE.NORMAL:
                switch (weaponType)
                {
                    case WeaponType.Blade:
                        damage = _bladeAttackDamage * playerAttackDamage;
                        break;
                    case WeaponType.Rifle:
                        damage = _playerRifleAttackDamage * playerAttackDamage;
                        break;
                    case WeaponType.HackingDevice:
                        damage = _playerHackAttackDamage * playerAttackDamage;
                        break;
                    default:
                        break;
                }
                break;
            case ATTACKTYPE.GRENADE:
                damage = playerGrenadeDamage * playerGrenadeDamageMulti;
                break;
            case ATTACKTYPE.SPECIALATTACK:
                switch (weaponType)
                {
                    case WeaponType.Blade:
                        damage = _bladeSPAttackDamage * playerSPAttackDamage;
                        break;
                    case WeaponType.Rifle:
                        damage = _playerRifleSPAttackDamage * playerSPAttackDamage;
                        break;
                    case WeaponType.HackingDevice:
                        damage = _playerHackSPAttackDamage * playerSPAttackDamage;
                        break;
                    default:
                        break;
                }
                break;
            case ATTACKTYPE.DASH:
                damage = _playerDashDamage * playerDashDamageMulti;
                break;
            
            default:
                break;
        }

        monster.GetComponent<MonsterBase>().TakeDamage(damage, null);
    }


    public void Initialize()
    {
        EventBus.OnMonsterHit += TakeDamage;
    }
}
