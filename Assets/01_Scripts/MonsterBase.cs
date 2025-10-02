using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;         // 최대 체력
    public int currentHealth;          // 현재 체력
    public int attackDamage = 10;      // 공격력 (PlayerHealth.TakeDamage가 int라면 int로 맞춤)
    public float attackRange = 2f;     // 공격 범위
    public float attackDelay = 0.5f;   // 공격 텀(딜레이)
    public bool isDead = false;        // 몬스터 사망 여부

    [Header("Components")]
    protected NavMeshAgent agent;      // 네비게이션 에이전트 컴포넌트
    protected Transform player;        // 플레이어 Transform (추적용)

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

    // 데미지 적용 함수
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리 (파티클, 사운드 등 추가 가능)
    public virtual void Die()
    {
        // 몬스터 사망 처리 로직 추가 요망
        isDead = true;
        Destroy(gameObject);
    }
}
