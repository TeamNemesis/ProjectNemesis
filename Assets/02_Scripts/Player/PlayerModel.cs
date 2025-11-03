using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 상태, 속성, 데이터(체력, 경험치, 레벨 등)를 관리하는 클래스입니다.
/// 게임 내에서 플레이어의 다양한 정보와 상태값을 저장하고 제공합니다.
/// </summary>
public class PlayerModel : CharacterModelBase
{

    /// <summary>
    /// 플레이어 피격시
    /// </summary>
    public event Action<Transform> PlayerHit;

    /// <summary>
    /// 회피 가능한지
    /// </summary>
    private bool bIsAvoid;

    /// <summary>
    /// 무적 상태인지
    /// </summary>
    [SerializeField]
    private bool _bIsInvincibility;
    public bool bIsInvincibility { get { return _bIsInvincibility; } }
    public void SetIsInvincibility(bool bIsInvincibility)
    {
        _bIsInvincibility = bIsInvincibility;
    }

    // 남은 무적 시간
    private float _invincibilityTimeRemaining;


    /// <summary>
    /// 무적 활성에 대한 코루틴
    /// </summary>
    private Coroutine _invincibilityCoroutine;

    /// <summary>
    /// 회피율
    /// </summary>
    private float _avoidNum;
    public float avoidNum { get { return _avoidNum; } }
    public void SetAvoidNum(float avoidNum)
    {
        _avoidNum = avoidNum;
    }

    /// <summary>
    /// 데미지 감소 효과 적용
    /// </summary>
    private bool bIsReduceDamage;
    private float _damageReducePercent;

    public override void TakeDamage(float damage, Transform attacker)
    {
        // 무적 상태라면 데미지X
        if(_bIsInvincibility)
        {
            return;
        }

        // 회피가능하다면 확률 계산 후 회피
        if (bIsAvoid)
        {
            int tempNum = UnityEngine.Random.Range(0, 100);
            if (tempNum < 100 * _avoidNum)
            {
                Debug.LogError("회피");
                return;
            }
        }

        // 받는 피해 감소가 있다면 데미지 감소
        if(bIsReduceDamage)
        {
            damage*=(1f-_damageReducePercent);
        }

        if(attacker != null)
        {
            OnPlayerHit(attacker);
        }

        base.TakeDamage(damage, attacker);
    }
    public override void Initialize()
    {
        base.Initialize();
        SetCurrentHp(maxHealth); // 초기화 시 현재 체력을 최대 체력으로 설정

        // 필드값 초기화
        _bIsInvincibility = false;
        _invincibilityCoroutine = null;
        bIsAvoid = false;
        _avoidNum = 0f;
        bIsReduceDamage = false;
        _damageReducePercent = 0f;
        _invincibilityTimeRemaining = 0f;
        debuffHandler.InitializePlayer();

        Debug.Log("연결");
        GameManager.Instance.PlayerStatManager.OnplayerAvoidanceChange += OnPlayerAvoidanceChange;
        GameManager.Instance.PlayerStatManager.OnplayerHitPercentChange += OnPlayerHitReducePercentChange;
    }

    private void OnPlayerHitReducePercentChange(float reducePercent)
    {
        if (reducePercent > 0)
        {
            bIsReduceDamage = true;
        }
        else
        {
            bIsReduceDamage = false;
        }
        _damageReducePercent = reducePercent;
    }

    private void OnPlayerAvoidanceChange(float avoidance)
    {
        if (avoidance > 0)
        {
            bIsAvoid = true;
        }
        else
        {
            bIsAvoid = false;
        }
        _avoidNum = avoidance;
    }

    public void OnPlayerHit(Transform monsterTransform)
    {
        PlayerHit?.Invoke(monsterTransform);
    }



    public void PlayerInvincibility(float time)
    {
        // 무적 상태가 이미 활성화되어 있고, 남은 시간이 더 길면 무시
        if (_bIsInvincibility && _invincibilityTimeRemaining >= time)
        {
            return;
        }

        Debug.LogError("무적 시작");
        // 기존 코루틴 중단
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = null;
        }

        // 새 시간으로 무적 재시작
        _bIsInvincibility = true;
        _invincibilityTimeRemaining = time;
        _invincibilityCoroutine = StartCoroutine(PlayerInvincibilityCoroutine());
    }

    private IEnumerator PlayerInvincibilityCoroutine()
    {
        while (_invincibilityTimeRemaining > 0f)
        {
            _invincibilityTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        _bIsInvincibility = false;
        _invincibilityCoroutine = null;
        Debug.LogError("무적 상태 종료");
    }
}
