using NUnit.Framework;
using System.Collections;
using UnityEngine;

/// <summary>
/// 돌연변이 4 획득시 특수공격에 사용되는 에너지 구체
/// </summary>
public class EnergyCircle : MonoBehaviour
{
    [Header("----- 초기 세팅 값 -----")]
    [SerializeField] float _initialRadius = 1f;     // 초기 반경
    [SerializeField] float _maxRadius = 2f;         // 풀 차지시 반경
    [SerializeField] float _bonusRadius = 2f;       // 풀 차지시 보너스로 제공되는 반경
    [SerializeField] float _initialDamage = 5f;     // 초기 데미지
    [SerializeField] float _maxDamage = 20f;        // 풀 차지시 데미지
    [SerializeField] float _bonusDamage = 20f;      // 풀 차지시 보너스로 제공되는 데미지
    [SerializeField] float _moveSpeed = 5f;         // 구체 이동 속도
    [SerializeField] float _lifetime = 10f;          // 발사 후 생존 시간

    [Header("----- 발사시 받아올 값 -----")]
    [SerializeField] float _expansionDuration;      // 최소 0초부터 1.5초까지 받아옴
    [SerializeField] Vector3 _moveDir;

    [Header("----- 런타임 데이터 -----")]
    [SerializeField] float _currentRadius;
    [SerializeField] float _currentDamage;
    [SerializeField] float _timer;

    public void Initialize(float expansionDuration, Vector3 moveDir)
    {
        _expansionDuration = expansionDuration;
        _moveDir = moveDir;
        SetEnergyCircle(_expansionDuration);
        _timer = 0f;
    }

    private void Update()
    {
        Move(_moveDir);
        // 혹시라도 벽에 부딪히지 않고 일정 시간 지나면 자동 소멸
        _timer += Time.deltaTime;
        if(_timer >= _lifetime)
        {
            GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        }
    }

    void SetEnergyCircle(float expansionDuration)
    {
        // 0초일땐 초기값, 1.5초일땐 최대값만큼 localScale과 데미지 설정
        float ratio = Mathf.Clamp01(expansionDuration / 1.5f);
        _currentRadius = Mathf.Lerp(_initialRadius, _maxRadius, ratio);
        _currentDamage = Mathf.Lerp(_initialDamage, _maxDamage, ratio);

        // 만약 최대치에 도달했다면 보너스 추가
        if (expansionDuration >= 1.5f)
        {
            _currentRadius += _bonusRadius;
            _currentDamage += _bonusDamage;
        }
        Vector3 currentScale = Vector3.one * 0.5f;
        transform.localScale = currentScale * _currentRadius;
    }

    void Move(Vector3 moveDir)
    {
        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_MONSTER))
        {
            GameManager.Instance.PlayerStatManager.TakeDamage(WeaponType.Rifle, ATTACKTYPE.SPECIALATTACK, other.transform);
            GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        }
        if(other.CompareTag(Constants.TAG_WALL))
        {
            GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
        }
    }
}