using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 3단 콤보 근거리 무기
/// 개선사항:
/// - RequestAttack에서 공격 중일 때 즉시 큐잉 허용(acceptBuffer 조건 제거)
/// - fallback 코루틴을 현재 재생중인 애니메이션 클립 길이에 기반해 시작(clip.length + 마진)
/// - 디버그 로그 보강
/// </summary>
[DisallowMultipleComponent]
public class PlayerBladeNormalAttacker : PlayerNormalAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;

    [Header("Combo Settings")]
    [SerializeField] private int maxCombo = 3;
    [SerializeField] private float inputBufferTime = 0.25f;    // 보조용 (필요시 사용)
    [SerializeField] private float comboResetDelay = 1.0f;     // 콤보를 리셋하는 시간 (마지막 공격 후)
    [SerializeField] private float fallbackDelay = 0.5f;       // 최소값, 실제로는 clip 길이를 우선 사용

    [Header("Hit settings (optional)")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask hitLayerMask;

    private int _comboIndex = 0;        // 0 기반: 0,1,2 = 3단 콤보
    private bool _queuedAttack = false;
    private Coroutine _comboResetRoutine;
    private Coroutine _fallbackRoutine;
    private Coroutine _inputBufferRoutine;
    private bool _acceptInputBuffer = false;

    // 외부에서 초기화 가능
    public void Initialize(Player player, Transform firePoint = null)
    {
        _player = player;
    }

    /// <summary>
    /// RequestAttack: 공격 입력이 들어오면 호출
    /// 변경: 공격 중일 때 input buffer 여부와 상관없이 큐잉을 허용하도록 함(단 maxCombo 제한은 유지).
    /// </summary>
    public override bool RequestAttack()
    {
        Debug.Log($"RequestAttack called. CanStartAttack={CanStartAttack()}, IsAttacking={IsAttacking}, acceptBuffer={_acceptInputBuffer}, comboIndex={_comboIndex}");

        if (CanStartAttack())
        {
            Debug.Log("RequestAttack called", this);
            _comboIndex = 0;
            _queuedAttack = false;
            StartAttack();
            Debug.Log("RequestAttack -> StartAttack (combo started step 1)");
            return true;
        }

        // 변경: 공격 중이면 input buffer 여부와 무관하게 큐잉 허용(최대 콤보 제한만 적용)
        if (IsAttacking)
        {
            Debug.Log("RequestAttack called", this);
            if (_comboIndex < maxCombo - 1)
            {
                _queuedAttack = true;
                Debug.Log($"RequestAttack -> queued attack (will go to step {_comboIndex + 2})");
                return true;
            }
            else
            {
                Debug.Log("RequestAttack -> cannot queue: already at final combo step");
            }
        }

        Debug.Log("RequestAttack -> rejected");
        return false;
    }

    /// <summary>
    /// Attack: Animator에 int + single trigger 방식으로 호출
    /// 동일 스텝 재전송 방지 로직 포함
    /// fallback은 현재 재생중인 clip.length 기반으로 시작
    /// </summary>
    public override void Attack()
    {
        if (_player == null)
        {
            Debug.LogWarning("PlayerBladeNormalAttacker.Attack: _player is null.");
            return;
        }

        // 입력 버퍼 윈도우 시작(보조용)
        if (_inputBufferRoutine != null)
        {
            _player.StopCoroutine(_inputBufferRoutine);
            _inputBufferRoutine = null;
        }
        _inputBufferRoutine = _player.StartCoroutine(InputBufferCoroutine());

        // Animator 호출: integer param + 단일 trigger 사용
        int step = _comboIndex + 1; // 1-based for Animator
        var unityAnimator = _player.GetComponentInChildren<Animator>();

        if (unityAnimator != null)
        {
            // normalizedTime 체크로 재진입 방지
            var stateInfo = unityAnimator.GetCurrentAnimatorStateInfo(0);
            bool sameStatePlaying = stateInfo.IsName($"Blade_{step}");

            if (sameStatePlaying && stateInfo.normalizedTime < 0.9f)
            {
#if UNITY_EDITOR
                Debug.Log($"PlayerBlade: prevented re-trigger of Blade_{step} (normalized={stateInfo.normalizedTime:F2})");
#endif
                return;
            }

            // 파라미터 세팅 후 트리거 발생
            _player.Animator?.Animator.SetInteger("BladeStep", step);
            _player.Animator?.Animator.SetTrigger("OnBladeAttack");
            Debug.Log($"Attack() -> triggered Animator Blade step {step}");

            // 기존 fallback 코루틴 취소 후, 현재 재생중인 클립 길이를 얻어 그 길이 + margin 으로 fallback 시작
            if (_fallbackRoutine != null)
            {
                _player.StopCoroutine(_fallbackRoutine);
                _fallbackRoutine = null;
            }

            // try get current clip length
            float clipLength = fallbackDelay; // 기본 최소값
            var clipInfos = unityAnimator.GetCurrentAnimatorClipInfo(0);
            if (clipInfos != null && clipInfos.Length > 0 && clipInfos[0].clip != null)
            {
                clipLength = clipInfos[0].clip.length;
                // 마진: 애니메이션 이벤트가 끝나기 전에 타임아웃이 트리거되지 않도록 넉넉히
                clipLength *= 1.1f;
            }
            // 보장: 최소 fallbackDelay 이상
            clipLength = Mathf.Max(clipLength, fallbackDelay);

            _fallbackRoutine = _player.StartCoroutine(FallbackEndCoroutineWithDelay(clipLength));
            Debug.Log($"Fallback scheduled in {clipLength:F2}s (based on clip length or fallbackDelay)");
        }
        else
        {
            // Animator 래퍼 사용 가능하면 래퍼 호출
            _player.Animator?.TriggerBladeAttackStep(step);
            Debug.Log($"Attack() -> used PlayerAnimator helper for Blade step {step}");

            if (_fallbackRoutine != null)
            {
                _player.StopCoroutine(_fallbackRoutine);
                _fallbackRoutine = null;
            }
            // 안전하게 최소 fallbackDelay로 예약
            _fallbackRoutine = _player.StartCoroutine(FallbackEndCoroutineWithDelay(fallbackDelay));
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 현재 공격 애니메이션이 끝났을 때 호출
    /// </summary>
    public void OnAnimationAttackEnd()
    {
        Debug.Log($"OnAnimationAttackEnd called. queued={_queuedAttack}, comboIndex={_comboIndex}");

        // fallback 취소
        if (_fallbackRoutine != null && _player != null)
        {
            _player.StopCoroutine(_fallbackRoutine);
            _fallbackRoutine = null;
            Debug.Log("OnAnimationAttackEnd: canceled fallback coroutine");
        }

        if (_queuedAttack && _comboIndex < maxCombo - 1)
        {
            _queuedAttack = false;
            _comboIndex++;
            Debug.Log("콤보 이어가기: " + (_comboIndex + 1));

            StartAttack();

            if (_comboResetRoutine != null)
            {
                _player.StopCoroutine(_comboResetRoutine);
                _comboResetRoutine = null;
            }
        }
        else
        {
            if (_comboResetRoutine != null)
                _player.StopCoroutine(_comboResetRoutine);
            _comboResetRoutine = _player.StartCoroutine(ComboResetCoroutine());
            EndAttack();
        }
    }

    public void DoMeleeHit()
    {
        if (_player == null) return;

        Vector3 origin = _player.transform.position + _player.transform.forward * (attackRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(origin, attackRadius, hitLayerMask, QueryTriggerInteraction.Ignore);
        foreach (var c in hits)
        {
            var hit = c.GetComponent<IDamageable>();
            if (hit != null)
            {
                hit.TakeDamage(10); // 실제 데미지 값은 조정하세요
            }
        }

#if UNITY_EDITOR
        Debug.DrawLine(_player.transform.position, origin, Color.red, 0.5f);
#endif
    }

    IEnumerator InputBufferCoroutine()
    {
        _acceptInputBuffer = true;
        yield return new WaitForSeconds(inputBufferTime);
        _acceptInputBuffer = false;
        _inputBufferRoutine = null;
    }

    IEnumerator ComboResetCoroutine()
    {
        yield return new WaitForSeconds(comboResetDelay);
        _comboIndex = 0;
        _queuedAttack = false;
        _comboResetRoutine = null;
        Debug.Log("ComboResetCoroutine: combo reset to 0");
    }

    IEnumerator FallbackEndCoroutineWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _fallbackRoutine = null;
        _queuedAttack = false;
        _comboIndex = 0;
        Debug.Log("FallbackEndCoroutine: fallback triggered, combo reset and EndAttack");
        EndAttack();
    }

    public override void EndAttack()
    {
        if (_fallbackRoutine != null && _player != null)
        {
            _player.StopCoroutine(_fallbackRoutine);
            _fallbackRoutine = null;
        }
        if (_comboResetRoutine != null && _player != null)
        {
            _player.StopCoroutine(_comboResetRoutine);
            _comboResetRoutine = null;
        }
        if (_inputBufferRoutine != null && _player != null)
        {
            _player.StopCoroutine(_inputBufferRoutine);
            _inputBufferRoutine = null;
        }

        _queuedAttack = false;
        _acceptInputBuffer = false;

        base.EndAttack();
        Debug.Log("EndAttack called");
    }
}