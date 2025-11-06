using System;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
/// <summary>
/// 플레이어의 유탄공격을 담당하는 클래스
/// 무기타입에 상관없이 공통으로 사용
/// </summary>
public class PlayerGrenadeAttacker : MonoBehaviour
{
    [SerializeField] string _grenadePath = "Prefabs/Bullet/Grenade";
    [SerializeField] string _explodeCirclePath = "Prefabs/Effect/Skill/ExplodeCircle";
    [SerializeField] float travelTime = 1.0f;     // 유탄이 도착하는 시간
    [SerializeField] float _coolTime;   // 쿨타임
    [SerializeField] float _timer = 0.0f;       // 쿨타임 타이머
    [SerializeField] int _maxCount = 3;       // 최대 소지 개수
    [SerializeField] int _currentCount = 0;   // 현재 소지 개수

    public Vector3 _mousePos;

    public event Action<int, int> OnGrenadeCountChanged; // 현재 유탄 개수, 최대 유탄 개수
    public event Action<float, float> OnGrenadeCooltimeChanged; // 현재 쿨타임, 최대 쿨타임

    /// <summary>
    /// 쿨타임 설정
    /// </summary>
    public void SetCoolTime()
    {
        _coolTime = GameManager.Instance.PlayerStatManager.grenadeCoolTime * GameManager.Instance.PlayerStatManager.grenadeCoolTimeMulti;
        OnGrenadeCooltimeChanged?.Invoke(_timer, _coolTime);

    }


    private void Update()
    {
        // 쿨타임 처리
        if(_currentCount < _maxCount)
        {
            _timer += Time.deltaTime;
            OnGrenadeCooltimeChanged?.Invoke(_timer, _coolTime);
            if (_timer >= _coolTime)
            {
                _currentCount++;
                _timer = 0.0f;
                OnGrenadeCountChanged?.Invoke(_currentCount, _maxCount);
            }
        }
    }

    public void Initialize()
    {
        _currentCount = _maxCount;
        _coolTime = GameManager.Instance.PlayerStatManager.grenadeCoolTime * GameManager.Instance.PlayerStatManager.grenadeCoolTimeMulti;
        GameManager.Instance.PlayerStatManager.OnGrenadeCoolTimeMultiChange += SetCoolTime;
        OnGrenadeCooltimeChanged?.Invoke(0.0f, _coolTime);
        OnGrenadeCountChanged?.Invoke(_currentCount, _maxCount);
        _explodeCirclePath = "Prefabs/Effect/Skill/ExplodeCircle";
        _grenadePath = "Prefabs/Bullet/Grenade";
    }

    public void GrenadeAttack(Vector3 mousePos)
    {
        _currentCount--;
        OnGrenadeCountChanged?.Invoke(_currentCount, _maxCount);
        if (EventBus.HasMutant1)
        {
            StartCoroutine(Launch3GrenadeRoutine(mousePos));
            return;
        }
        LaunchGrenade(mousePos);
    }

    public bool RequestAttack()
    {
        if(_currentCount <= 0)
        {
            Debug.Log("유탄이 없습니다.");
            return false;
        }
        
        GrenadeAttack(_mousePos);
        return true;
    }

    public void SetMousePos(Vector3 mousePos)
    {
        _mousePos = mousePos;
    }

    /// <summary>
    /// 유탄을 발사하는 함수
    /// </summary>
    /// <param name="targetPos"></param>
    private void LaunchGrenade(Vector3 targetPos)
    {
        GameObject grenade = GameManager.Instance.PoolManager.GetFromPool(_grenadePath, transform.position + Vector3.up * 1.0f, Quaternion.identity);
        GameObject explodeCircle = GameManager.Instance.PoolManager.GetFromPool(_explodeCirclePath, targetPos + Vector3.up * 0.1f, Quaternion.Euler(90f, 0f, 0f));

        PlayerGrenadeBullet bullet = grenade.GetComponent<PlayerGrenadeBullet>();
        if(bullet == null)
        {
            Debug.Log("PlayerGrenadeBullet 컴포넌트가 없습니다.");
            return;
        }
        bullet.Initialize(transform, targetPos);

        Destroy(explodeCircle, travelTime);
    }

    /// <summary>
    /// 돌연변이 획득 시 유탄 3발 발사하는 코루틴
    /// </summary>
    IEnumerator Launch3GrenadeRoutine(Vector3 targetPos)
    {
        int count = 0;
        while (count < 3)
        {
            LaunchGrenade(targetPos);
            count++;
            yield return new WaitForSeconds(0.2f);  // 발사간격은 나중에 튜닝
        }

    }
}