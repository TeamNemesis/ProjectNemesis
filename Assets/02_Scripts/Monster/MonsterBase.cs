using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;        // 최대 체력
    [SerializeField] protected int currentHealth;          // 현재 체력
    [SerializeField] protected int attackDamage = 10;      // 공격력 (PlayerHealth.TakeDamage가 int라면 int로 맞춤)
    [SerializeField] protected float attackRange = 2f;     // 공격 범위
    [SerializeField] protected float detectionRange = 10f; // 플레이어 감지 범위
    [SerializeField] protected float attackDelay = 0.5f;   // 공격 텀(딜레이)
    [SerializeField] protected bool isDead = false;        // 몬스터 사망 여부

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

    /// <summary>
    /// 몬스터가 플레이어를 볼 수 있는 상황인지 아닌지 확인하는 함수
    /// </summary>
    protected bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 레이어 마스크: "Player"는 감지 대상, "Wall"은 벽 등 시야를 막는 오브젝트
        int layerMask = LayerMask.GetMask("Player", "Wall");

        // 레이캐스트 발사
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, directionToPlayer, out RaycastHit hit, distanceToPlayer, layerMask))
        {
            // 만약 레이캐스트가 플레이어에 닿았다면 시야 확보
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("playerCheck");
                return true;
            }
            else
            {
                // 플레이어와의 사이에 다른 오브젝트가 있다면 차단됨
                return false;
            }
        }

        return false;
    }


    /// <summary>
    /// 플레이어를 바라보게 하는 함수
    /// </summary>
    protected void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
