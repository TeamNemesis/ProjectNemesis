using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;         // 최대 체력
    [SerializeField] protected int currentHealth;          // 현재 체력
    [SerializeField] protected int attackDamage = 10;      // 공격력 (PlayerHealth.TakeDamage가 int라면 int로 맞춤)
    [SerializeField] protected float attackRange = 2f;     // 공격 범위
    [SerializeField] protected float attackDelay = 0.5f;   // 공격 텀(딜레이)
    [SerializeField] protected bool isDead = false;        // 몬스터 사망 여부
    [SerializeField] protected float detectionRange = 10f;      // 플레이어 감지 범위

    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;      // 네비게이션 에이전트 컴포넌트
    [SerializeField] protected Transform player;        // 플레이어 Transform (추적용)

    // 부모 Start: NavMeshAgent와 Player Transform 자동 초기화, 체력 초기화
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 시작 시 현재 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 몬스터 사망시 호출되는 메서드 (자식 클래스에서 오버라이드 요망)
    /// 오버라이드 하지 않을 경우 기본 사망 처리 로직이 실행됨
    /// </summary>
    protected virtual void Die()
    {
        // 몬스터 기본 사망 처리 로직
        isDead = true;
        Destroy(gameObject);
    }
}
