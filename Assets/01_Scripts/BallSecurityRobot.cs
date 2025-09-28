using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BallSecurityRobot : MonoBehaviour
{
    // 상태머신 Enum
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Move,   // 플레이어를 추격 중일 때
        Attack, // 자폭 시도
        Die     // 파괴됨
    }

    [Header("Components")]
    [SerializeField]
    private NavMeshAgent agent;     // 네비게이션 에이전트 컴포넌트
    [SerializeField]
    public Transform player;        // 플레이어의 위치를 추적하기 위한 변수

    [Header("Stats")]
    public float detectionRange = 10f;      // 플레이어 감지 범위
    public float attackRange = 2f;          // 공격 범위 (자폭 도화선 시작 범위)
    public float explosionRadius = 3f;      // 실제 폭발 범위
    public int attackDamage = 10;           // 공격 데미지
    public float attackDelay = 0.5f;        // 공격시 텀
    public bool isDead = false;             // 몬스터 사망 여부

    private bool isFusing = false;          // 자폭 중복 실행 방지용

    [SerializeField]
    private State currentState = State.Idle; // 현재 상태

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (player == null) return;

        // 상태별 로직 실행
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;

            case State.Move:
                HandleMove();
                break;

            case State.Attack:
                StartCoroutine(SelfDestructionAttack());
                break;

            case State.Die:
                isDead = true;
                Destroy(gameObject);
                break;
        }
    }

    // Idle 상태 처리
    private void HandleIdle()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            currentState = State.Move; // Move 상태로 변경
        }
    }

    // Move 상태 처리
    private void HandleMove()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            agent.ResetPath();
            currentState = State.Idle; // Idle 상태로 변경
            return;
        }

        agent.SetDestination(player.position);

        if (distance <= attackRange && !isFusing)
        {
            currentState = State.Attack; // 공격 상태로 변경
        }
    }

    // 자폭 공격 함수 (Attack 상태)
    IEnumerator SelfDestructionAttack()
    {
        if (player != null && !isFusing) // 중복 실행 방지
        {
            isFusing = true; // 플래그 세팅

            // 자폭까지 딜레이
            yield return new WaitForSeconds(attackDelay);

            // 폭발 반경 안에 플레이어가 있는지 다시 확인
            float finalPlayerDistance = Vector3.Distance(transform.position, player.position);
            if (finalPlayerDistance <= explosionRadius) // 폭발 반경 체크
            {
                // 플레이어에게 PlayerHealth 컴포넌트가 있는지 확인하고, 있다면 데미지 적용
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    //플레이어에게 공격력만큼 데미지
                    playerHealth.TakeDamage(attackDamage);
                    currentState = State.Die; // 자폭 후 사망 상태로 변경
                }
            }
        }
    }
}