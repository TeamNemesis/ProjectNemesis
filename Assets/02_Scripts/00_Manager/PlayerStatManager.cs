using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    #region 공격력 

    #region 검
    /// <summary>
    /// 검 일반 공격력
    /// </summary>
    private float _bladeAttackDamage;
    public float bladeAttackDamage;
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
    private float RifleAttackDamage;
    #endregion 
    /// <summary>
    /// 플레이어 일반 공격 데미지 계수
    /// </summary>
    private float _playerAttackDamage;
    public float playerAttackDamage { get { return _playerAttackDamage;} }
    public void AddPlayerAttackDamage(float plusDamage)
    {
        _playerAttackDamage += plusDamage;
    }

    /// <summary>
    /// 플레이어 유탄 공격 데미지 계수
    /// </summary>
    private float _playerGrenadeDamage;
    public float playerGrenadeDamage { get { return _playerGrenadeDamage;} }
    public void AddPlayerGrenadeDamage(float plusGrenadeDamage)
    {
        _playerAttackDamage += plusGrenadeDamage;
    }

    /// <summary>
    /// 플레이어 특수 공격 데미지 계수
    /// </summary>
    private float _playerSPAttackDamage;
    public float playerSPAttackDamage { get { return _playerSPAttackDamage;} }
    public void AddPlayerSPAttackDamage(float plusSPDamage)
    {
        _playerSPAttackDamage += plusSPDamage;
    }    

    /// <summary>
    /// 플레이어 대쉬 공격력 계수
    /// </summary>
    private float _playerDashDamage;
    public float playerDashDamage { get { return _playerDashDamage; } }
    public void AddPlayerDashDamage(float plusDashDamage)
    {
        _playerDashDamage += plusDashDamage;    
    }

    #endregion

    #region 모든 데미지, 이동속도, 애니메이션 재생속도

    /// <summary>
    /// 모든 데미지 증가 계수
    /// </summary>
    private float _totalMultiDamage;
    public float totalMultiDamage { get { return _totalMultiDamage; }}

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
    private float playerMoveSpeed { get { return _playerMoveSpeed; }}
    private void AddPlayerMoveSpeed(float plusMoveSpeed)
    {
        _playerMoveSpeed += plusMoveSpeed;
    }
    #endregion

    #region 유탄, 범위공격

    /// <summary>
    /// 플레이어 범위 공격 범위 계수
    /// </summary>
    private float _playerAreaExtent = 1f;
    public float playerAreaExtent { get { return _playerAreaExtent; }}
    public void AddPlayerAreaExtent(float plusAreaExtent)
    {
        _playerAreaExtent += plusAreaExtent;
    }


    /// <summary>
    /// 유탄 쿨타임 계수
    /// </summary>
    private float _grenadeCoolTime;
    public float grenadeCoolTime { get { return _grenadeCoolTime; } }
    public void SetGrenadeCoolTime(float grenadeCoolTime)
    {
        _grenadeCoolTime *= grenadeCoolTime;
    }

    #endregion

    #region 받는 피해
    /// <summary>
    /// 플레이어 받는 피해 계수
    /// </summary>
    private float _playerHitPercent = 1f;
    public float playerHitPercent { get { return _playerHitPercent; }}
    public void AddPlayerHitPercent(float plusHitPercent)
    {
        _playerHitPercent += plusHitPercent;
    }
    #endregion


}
