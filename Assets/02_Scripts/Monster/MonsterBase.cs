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
    [SerializeField] protected bool isStunned = false;  // 현재 스턴 상태 여부
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

        // 과부하 디버프 처리 (스택당 5% 피해 증가)
        if (activeDebuffs.ContainsKey("과부하"))
        {
            ActiveDebuff overload = activeDebuffs["과부하"];
            float bonusMultiplier = 1f + (0.05f * overload.stackCount);
            damage *= bonusMultiplier;
        }

        // 화상 디버프 처리 (다음 피해 1회 2배)
        if (activeDebuffs.ContainsKey("화상"))
        {
            damage *= 2f;
            activeDebuffs.Remove("화상"); // 한 번만 적용
        }

        currentHealth -= (int)damage;
        if (currentHealth <= 0)
            Die();
    }


    // ==============================
    //        디버프 시스템
    // ==============================

    public class DebuffData
    {
        public string debuffName;
        public float debuffDuration;
        public float debuffValue;
        public int maxStack;
    }

    private class ActiveDebuff
    {
        public DebuffData data;
        public float remainingTime;
        public float totalValue;
        public int stackCount;
        public Coroutine routine;

        public ActiveDebuff(DebuffData data, Coroutine routine)
        {
            this.data = data;
            remainingTime = data.debuffDuration;
            totalValue = data.debuffValue;
            stackCount = 1;
            this.routine = routine;
        }
    }

    private Dictionary<string, ActiveDebuff> activeDebuffs = new Dictionary<string, ActiveDebuff>();

    /// <summary>
    /// 디버프를 적용시키는 함수.
    /// </summary>
    public void ApplyDebuff(DebuffData newDebuff)
    {
        if (activeDebuffs.ContainsKey(newDebuff.debuffName))
        {
            ActiveDebuff existing = activeDebuffs[newDebuff.debuffName];

            // 스택형 디버프들
            if (newDebuff.debuffName == "독" || newDebuff.debuffName == "과부하")
            {
                if (existing.stackCount < newDebuff.maxStack)
                {
                    existing.stackCount++;
                    existing.totalValue += newDebuff.debuffValue;
                }

                existing.remainingTime = newDebuff.debuffDuration;
            }
            else
            {
                // 비스택형 디버프는 단순 갱신
                existing.remainingTime = newDebuff.debuffDuration;
                existing.totalValue = newDebuff.debuffValue;
            }

            return;
        }

        Coroutine routine = StartCoroutine(HandleDebuff(newDebuff));
        activeDebuffs.Add(newDebuff.debuffName, new ActiveDebuff(newDebuff, routine));
    }

    private IEnumerator HandleDebuff(DebuffData debuff)
    {
        ActiveDebuff active = activeDebuffs[debuff.debuffName];

        // 디버프 시작 시 1회만 적용되는 효과
        switch (debuff.debuffName)
        {
            case "약화":
                // 단순 상태값 true
                break;

            case "둔화":
                if (agent != null)
                    agent.speed *= 0.7f;
                break;

            case "기절":
                StartCoroutine(StunCoroutine(debuff.debuffDuration));
                break;

            case "혼란":
                StartCoroutine(ConfuseCoroutine(debuff.debuffDuration));
                break;
        }

        while (active.remainingTime > 0f && !isDead)
        {
            switch (debuff.debuffName)
            {
                case "독":
                    TakeDamage(active.totalValue * Time.deltaTime);
                    break;

                case "과부하":
                    TakeDamage(active.totalValue * Time.deltaTime);
                    break;
            }

            active.remainingTime -= Time.deltaTime;
            yield return null;
        }

        // 디버프 해제 시 처리
        switch (debuff.debuffName)
        {
            case "둔화":
                if (agent != null)
                    agent.speed /= 0.7f;
                break;

            case "약화":
                // 상태 초기화
                break;
        }

        activeDebuffs.Remove(debuff.debuffName);
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;  // 스턴 시작

        if (agent != null)
            agent.isStopped = true;

        yield return new WaitForSeconds(duration);

        if (agent != null && !isDead)
            agent.isStopped = false;

        isStunned = false; // 스턴 해제
    }

    /// <summary>
    /// 혼란 적용코루틴, 일시적으로 타겟 태그를 Monster로 바꿔줌
    /// </summary>
    private IEnumerator ConfuseCoroutine(float duration)
    {
        string originalTag = targetTag;
        targetTag = "Monster"; // 적대 대상 전환

        yield return new WaitForSeconds(duration);

        targetTag = originalTag;
    }

    /// <summary>
    /// 디버프 데이터를 받고 해당 디버프가 적용중인지 아닌지를 반환하는 함수
    /// </summary>
    public bool CheckDebuffs(DebuffData data)
    {
        return activeDebuffs.ContainsKey(data.debuffName);
    }

    /// <summary>
    /// 현재 적용중인 디버프의 갯수를 반환해주는 함수
    /// </summary>
    public int GetActiveDebuffCount()
    {
        int count = 0;

        foreach (KeyValuePair<string, ActiveDebuff> pair in activeDebuffs)
        {
            ActiveDebuff debuff = pair.Value;
            if (debuff != null && debuff.remainingTime > 0f)
            {
                count++;
            }
        }

        return count;
    }
}
