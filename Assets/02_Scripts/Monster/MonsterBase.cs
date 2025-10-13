using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour, IDamageAble
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;        // 최대 체력
    [SerializeField] protected int currentHealth;          // 현재 체력
    [SerializeField] protected int attackDamage = 10;      // 공격력 (PlayerHealth.TakeDamage가 int라면 int로 맞춤)
    [SerializeField] protected float attackRange = 2f;     // 공격 범위
    [SerializeField] protected float detectionRange = 10f; // 플레이어 감지 범위
    [SerializeField] protected float attackDelay = 0.5f;   // 공격 텀(딜레이)
    [SerializeField] protected bool isDead = false;        // 몬스터 사망 여부
    [SerializeField] protected string targetTag = "Player";  // 타겟이 될 태그

    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;      // 네비게이션 에이전트 컴포넌트
    [SerializeField] protected Transform player;        // 플레이어 Transform (추적용)

    // 외부에서 죽는 이벤트 추가
    public Action OnDieEvent;

    // 부모 Start: NavMeshAgent와 Player Transform 자동 초기화, 체력 초기화
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
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
        if (OnDieEvent != null)
        {
            OnDieEvent();
        }
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

        // 레이어 마스크: targetTag는 감지 대상, "Wall"은 벽 등 시야를 막는 오브젝트
        int layerMask = LayerMask.GetMask(targetTag, "Wall");

        // 레이캐스트 발사
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, directionToPlayer, out RaycastHit hit, distanceToPlayer, layerMask))
        {
            // 만약 레이캐스트가 targetTag에 닿았다면 시야 확보
            if (hit.transform.CompareTag(targetTag))
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


    //IDamageAble 인터페이스 구현
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        // 데미지를 int로 변환하여 적용 나중에 float로 바꿀 수도
        currentHealth -= (int)damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private class ActiveDebuff
    {
        public DebuffData data;
        public float remainingTime;
        public float value;
        public bool isDebuffed;
        public Coroutine routine;

        public ActiveDebuff(DebuffData data, Coroutine routine)
        {
            this.data = data;
            remainingTime = data.debuffDuration;
            value = data.debuffValue;
            isDebuffed = data.isDibuffed;
            this.routine = routine;
        }
    }


    private Dictionary<string, ActiveDebuff> activeDebuffs = new Dictionary<string, ActiveDebuff>();


    public void ApplyDebuff(DebuffData newDebuff)
    {
        if (activeDebuffs.ContainsKey(newDebuff.debuffName))
        {
            // 이미 같은 이름의 디버프가 존재 할 경우 누적 처리
            ActiveDebuff existing = activeDebuffs[newDebuff.debuffName];
            existing.remainingTime = newDebuff.debuffDuration; // 지속시간 갱신
            existing.value += newDebuff.debuffValue; // 피해량 누적

            //// 슬로우 재적용
            //UpdateMoveSpeed();
            //return;
        }


        // 새 디버프 생성
        Coroutine routine = StartCoroutine(HandleDebuff(newDebuff));
        activeDebuffs.Add(newDebuff.debuffName, new ActiveDebuff(newDebuff, routine));

        // 슬로우 반영
        //UpdateMoveSpeed();
    }

    private IEnumerator HandleDebuff(DebuffData debuff)
    {
        while (activeDebuffs.ContainsKey(debuff.debuffName))
        {
            if (isDead)
            {
                yield break;
            }

            ActiveDebuff active = activeDebuffs[debuff.debuffName];

            // 초당 누적 피해 적용
            if (active.value > 0)
                TakeDamage(active.value * Time.deltaTime);

            active.remainingTime -= Time.deltaTime;

            if (active.remainingTime <= 0)
                break;

            yield return null;
        }

        // 디버프 해제 시
        activeDebuffs.Remove(debuff.debuffName);
    }

    //디버프 체커
    public bool CheckDebuffs(DebuffData data)
    {
        foreach (var item in activeDebuffs)
        {
            if (data.debuffName == item.Key)
            {
                return true;
            }
        }
        return false;
    }

}
