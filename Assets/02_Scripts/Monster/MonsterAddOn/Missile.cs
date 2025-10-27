using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Missile : MonsterBase
{

    [Header("Local Stats"), SerializeField]
    private float _explosionRadius = 2f;      // 실제 폭발 범위

    // attackDamage, attackRange, attackDelay, isDead 등은 MonsterBase에서 상속됨

    [Header("Effects"), SerializeField]
    private PoolableObject circlePrefab;     // AttackDecalEffect 프리팹 (Inspector에서 지정)

    [SerializeField] private bool _isFusing = false;

    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;

        switch (baseState)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;

            case MonsterState.Move:
                HandleMove();
                break;

            case MonsterState.Attack:
                if (!_isFusing)
                    StartCoroutine(SelfDestructionAttack());
                break;

            case MonsterState.Die:
                Die();
                break;
        }
    }

    private void HandleIdle()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance <= detectionRange)
        {
            baseState = MonsterState.Move;
        }
    }

    private void HandleMove()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > detectionRange)
        {
            agent.ResetPath();
            baseState = MonsterState.Idle;
            return;
        }

        agent.SetDestination(_target.position);

        if (distance <= attackRange && !_isFusing)
        {
            baseState = MonsterState.Attack;
        }
    }

    private IEnumerator SelfDestructionAttack()
    {
        if (_target != null && !_isFusing)
        {
            _isFusing = true;

            // 자폭 카운트다운 원 생성
            if (circlePrefab != null)
            {
                GameObject circle = GameManager.Instance.PoolManager.GetFromPool(circlePrefab, transform.position, circlePrefab.transform.rotation);
                AttackDecalEffect effect = circle.GetComponent<AttackDecalEffect>();
                if (effect != null)
                {
                    effect.Play(0.1f, _explosionRadius);
                }
            }

            yield return new WaitForSeconds(0.1f);

            CheckTarget();

            baseState = MonsterState.Die;
        }
    }

    public void CheckTarget()
    {
        // 콜라이더 탐색
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.tag == targetTag)
            {
                collider.GetComponent<IDamageable>().TakeDamage(attackDamage);
            }
        }
    }
    public override void Initialize(object data = null)
    {
        base.Initialize(data); // 부모 클래스의 Initialize 호출

        // 미사일 전용 초기화
        baseState = MonsterState.Idle;
        _isFusing = false;
        StopAllCoroutines();
    }
}
