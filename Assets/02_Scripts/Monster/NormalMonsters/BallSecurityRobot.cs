using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BallSecurityRobot : MonsterBase
{
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Move,   // 플레이어를 추격 중일 때
        Attack, // 자폭 시도
        Die     // 파괴됨
    }

    [Header("Local Stats"), SerializeField]
    private float _explosionRadius = 3f;      // 실제 폭발 범위

    // attackDamage, attackRange, attackDelay, isDead 등은 MonsterBase에서 상속됨

    [Header("Effects"), SerializeField]
    private GameObject circlePrefab;     // AttackDecalEffect 프리팹 (Inspector에서 지정)

    [SerializeField] private bool _isFusing = false;
    [SerializeField]
    private State currentState = State.Idle; 

    private void Update()
    {
        if (isDead || _target == null) return;
        if (isStunned) return;

        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;

            case State.Move:
                HandleMove();
                break;

            case State.Attack:
                if (!_isFusing)
                    StartCoroutine(SelfDestructionAttack());
                break;

            case State.Die:
                Die();
                break;
        }
    }

    private void HandleIdle()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance <= detectionRange)
        {
            currentState = State.Move;
        }
    }

    private void HandleMove()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > detectionRange)
        {
            agent.ResetPath();
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(_target.position);

        if (distance <= attackRange && !_isFusing)
        {
            currentState = State.Attack;
        }
    }

    private IEnumerator SelfDestructionAttack()
    {
        if (_target != null && !_isFusing)
        {
            _isFusing = true;

            // 자폭 카운트다운 원 생성 (로봇의 자식으로 붙임)
            if (circlePrefab != null)
            {
                GameObject circle = Instantiate(circlePrefab, transform.position, circlePrefab.transform.rotation, transform);
                AttackDecalEffect effect = circle.GetComponent<AttackDecalEffect>();
                if (effect != null)
                {
                    effect.Play(attackDelay, _explosionRadius);
                }
            }

            // 자폭까지 딜레이
            yield return new WaitForSeconds(attackDelay);

            CheckTarget();

            currentState = State.Die;
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
}
