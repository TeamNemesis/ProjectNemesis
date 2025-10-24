using System;
using UnityEngine;

/// <summary>
/// 몬스터와 플레이어 부모 클래스
/// </summary>
[RequireComponent(typeof(DebuffHandler))]
public abstract class CharacterModelBase : PoolableObject, IDamageable
{
    [SerializeField]
    protected int _maxHealth = 100;
    public int maxHealth { get { return _maxHealth; } } // 최대 체력을 반환하는 속성
    public void SetMaxHp(int plusMaxHp)
    {
        _maxHealth += plusMaxHp;
        _currentHealth += plusMaxHp;
        OnHpChanged?.Invoke(_maxHealth, _currentHealth);
    }


    [SerializeField]
    protected int _currentHealth;
    public int currentHealth { get { return _currentHealth; } } // 현재 체력을 반환하는 속성

    /// <summary>
    /// Initialize용 현제 체력 세팅
    /// </summary>
    /// <param name="currentHp"></param>
    public void SetCurrentHp(int currentHp)
    {
        _currentHealth = currentHp;
        OnHpChanged?.Invoke(_maxHealth, _currentHealth);
    }

    [SerializeField]
    protected float _moveSpeed = 5; // 이동 속도
    public float moveSpeed { get { return _moveSpeed; } }
    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
        OnMoveSpeedChanged?.Invoke(_moveSpeed);
    }



    [Header("상태이상")]
    [SerializeField] public bool isStunned = false;    // 스턴
    [SerializeField] public bool isPushed = false;     // 밀림
    [SerializeField] public bool isBindned = false;    // 속박
    [SerializeField] public bool isWeaken = false;     // 약화
    [SerializeField] public bool isDead = false;       // 죽음

    /// <summary>
    /// 약화시 추가 데미지가 들어갈지
    /// </summary>
    protected bool BIsWeakenPlusDamage;
    protected float _weakenPlusDamage;

    public event Action<int, int> OnHpChanged; // 체력 변경 시 발생하는 이벤트
    public event Action<float> OnMoveSpeedChanged; // 이동 속도 변경 시 발생하는 이벤트

    public event Action OnDieEvent;

    [SerializeField] protected DebuffHandler debuffHandler;
    public DebuffHandler GetDebuffHandler()
    {
        return debuffHandler;
    }

    private void OnWeakenPlusDamageChange()
    {
        if (GameManager.Instance.PlayerStatManager.playerAvoidance > 0)
        {
            BIsWeakenPlusDamage = true;
        }
        else
        {
            BIsWeakenPlusDamage = false;
        }
        _weakenPlusDamage = GameManager.Instance.PlayerStatManager.weakenPlusDamage;
    }

    /// <summary>
    /// 캐릭터 생성 시 초기화 함수
    /// </summary>
    public virtual void Initialize()
    {
        debuffHandler = GetComponent<DebuffHandler>();
        GameManager.Instance.PlayerStatManager.OnWeakenPlusDamageChange += OnWeakenPlusDamageChange;
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    /// <param name="plusHp"></param>
    public void Heal(int plusHp)
    {
        _currentHealth += plusHp;
        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }
        OnHpChanged?.Invoke(_maxHealth, _currentHealth);
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        // 과부하 디버프
        if (debuffHandler != null && debuffHandler.HasDebuff(Constants.DEBUFF_OVERLOAD))
        {
            int stacks = debuffHandler.GetStackCount(Constants.DEBUFF_OVERLOAD);
            float bonus = 1f + (0.05f * stacks);
            damage *= bonus;
        }

        // 약화
        if (debuffHandler != null && BIsWeakenPlusDamage)
        {
            if (debuffHandler.HasDebuff(Constants.DEBUFF_WEAKEN))
            {
                damage *= _weakenPlusDamage;
            }
        }


        // 화상 디버프
        if (debuffHandler != null && debuffHandler.HasDebuff(Constants.DEBUFF_BURN))
        {
            damage *= 2f;
            debuffHandler.RemoveDebuff(Constants.DEBUFF_BURN);
        }

        if (this is MonsterBase monster)
        {

            damage *= GameManager.Instance.PlayerStatManager.totalMultiDamage;
        }

        _currentHealth -= (int)damage;
        if (currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
        else
        {
            OnHpChanged?.Invoke(_maxHealth, _currentHealth); // 체력 변경 이벤트 발행
        }
    }



    protected virtual void Die()
    {
        OnDieEvent?.Invoke();

        isDead = true;
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        OnDieEvent = null;
    }
}
