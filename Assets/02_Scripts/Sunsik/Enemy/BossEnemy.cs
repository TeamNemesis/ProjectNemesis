using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy, ISkillUser
{
    [Header("----- Component References -----")]
    [SerializeField] Transform _missilePortPos; // 미사일 발사 위치
    [SerializeField] GameObject _missilePrefab; // 미사일 프리팹

    [Header("----- Boss Stats(Temp) -----")]
    [SerializeField] float _skill1CoolTime = 10f; // 스킬1 쿨타임

    [Header("----- ReadOnly -----")]
    [SerializeField] float _currentSkill1Timer; // 현재 스킬1 타이머
    [SerializeField] bool _isSkill1Ready; // 스킬1 사용 가능 여부

    public bool CanUseSkill => _isSkill1Ready;

    private void Update()
    {
        if (_hasFirstMet && !_isSkill1Ready)
        {
            _currentSkill1Timer += Time.deltaTime;
            if(_currentSkill1Timer >= _skill1CoolTime)
            {
                _currentSkill1Timer = 0f;
                _isSkill1Ready = true;
            }
        }
    }

    // Animation Event용 함수
    public void OnFirstMeetAnimationEnd()
    {
        // 앞으로는 FirstMeet 안 들어가게
        SetFirstMet();

        // 전투 상태 진입
        _stateMachine.ChangeState(EnemyStateType.Combat);
    }

    // Animation Event용 함수
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
        Debug.Log("공격 애니메이션 종료");
    }

    public void SetSkill1Ready(bool isReady)
    {
        _isSkill1Ready = isReady;
    }

    public void UseSkill()
    {
        BossMissile missile = Instantiate(_missilePrefab, _missilePortPos.position, Quaternion.identity).GetComponent<BossMissile>();
        missile.Initialize(_target);
        SetSkill1Ready(false);
    }
}