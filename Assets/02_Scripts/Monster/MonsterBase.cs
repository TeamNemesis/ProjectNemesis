using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterSize
{
    SMALL,
    MIDDLE,
    BIG
}


public class MonsterBase : CharacterModelBase, IInitializePoolable
{
    [Header("Base Stats")]
    [SerializeField] private float maxEliteHealth;
    [SerializeField] protected float attackDamage = 10;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackDelay = 0.5f;
    [SerializeField] protected float originalSpeed = 10f;
    [SerializeField] public string targetTag = Constants.TAG_PLAYER;
    [SerializeField] protected MonsterSize monsterSize = MonsterSize.SMALL;
    [SerializeField] protected int cost;
    [SerializeField] protected Rigidbody monsterRigidbody;
    [SerializeField] protected Collider monsterCollider;



    #region 넉백
    [SerializeField] private float _knockBackDamage;
    [SerializeField] private Coroutine _knockBackCoroutine;
    #endregion
    [SerializeField] protected Transform player;


    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform _target;
    
    /// <summary>
    /// 공격력 반환
    /// </summary>
    /// <returns></returns>
    public float GetAttackDamage()
    {
        return attackDamage;
    }
    public MonsterSize GetMonsterSize()
    {
        return monsterSize;
    }

    /// <summary>
    /// 공격력 설정
    /// </summary>
    /// <param name="attackDamage"></param>
    public void SetAttackDamage(float attackDamage)
    {
        this.attackDamage = attackDamage;
    }

    public Transform GetTarget()
    {
        return _target;
    }
    public int GetCost()
    {
        return cost;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
    }


    //private void Start()
    //{
    //    Initialize();
    //}

    public void Initialize(object data = null)
    {
        base.Initialize();

        // === 상태 초기화 ===
        isDead = false;
        isPushed = false;
        isStunned = false;
        isBindned = false;
        isWeaken = false;

        // === 컴포넌트 초기화 ===
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.speed = originalSpeed;
        }

        // === 타겟 설정 ===
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj != null)
            _target = targetObj.transform;

        // === 체력 초기화 ===
        SetCurrentHp(maxHealth);

        // === 디버프 초기화 ===
        debuffHandler.InitializeMonster(agent);

        // === 물리 초기화 (넉백 관련) ===
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // === 코루틴 정리 ===
        if (_knockBackCoroutine != null)
        {
            StopCoroutine(_knockBackCoroutine);
            _knockBackCoroutine = null;
        }
    }

    protected bool CanSeePlayer()
    {
        if (_target == null) return false;

        Vector3 dir = (_target.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, _target.position);

        // 혼란 상태면 거리만 체크 (시야 무시)
        DebuffHandler debuffHandler = GetComponent<DebuffHandler>();
        if (debuffHandler != null && debuffHandler.HasDebuff(Constants.DEBUFF_CONFUSION))
        {
            return true;  // 혼란 상태면 항상 true
        }

        int mask = LayerMask.GetMask(targetTag, Constants.LAYER_MASK_WALL);
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, dir, out RaycastHit hit, dist, mask))
        {
            if (hit.transform == _target)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    protected void LookAtPlayer()
    {
        Vector3 dir = (_target.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void SetEliteMaxHealth(int roomCount)
    {
        maxEliteHealth = (float)_maxHealth * 1 + (0.1f * roomCount);
        _maxHealth = (int)maxEliteHealth;
    }

    protected override void Die()
    {
        GameManager.Instance.CurrencyManager.AddCredit(cost);
        base.Die();
    }


    #region 넉백

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (isPushed)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(Constants.LAYER_MASK_WALL))
            {
                Debug.Log("충돌");
                TakeDamage(GameManager.Instance.PlayerStatManager.knockBackDamage * GameManager.Instance.PlayerStatManager.knockBackDamageMulti);
                EndKnockBack();
            }

        }
    }


    /// <summary>
    /// 넉백 실행
    /// </summary>
    /// <param name="pushDirection"></param>
    /// <param name="damage"></param>
    public void KnockBackEnemy(Vector3 pushDirection, float damage, float knockBackDistance)
    {
        TakeDamage(damage);

        if (_knockBackCoroutine != null)
        {
            Debug.Log("넉백 중이므로 넉백 무시");
            return;
        }
        if (monsterSize == MonsterSize.BIG)
        {
            Debug.Log("대형이므로 넉백 무시");
            return;
        }
        GetComponent<Collider>().isTrigger = false;
        isPushed = true;
        debuffHandler.ApplyDebuff(DebuffHandler.DebuffData.CreateStun(0.5f));
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(pushDirection, ForceMode.VelocityChange);

        _knockBackCoroutine = StartCoroutine(KnockBackCoroutine(knockBackDistance));
        StartCoroutine(KnockBackCoolTime());
    }


    /// <summary>
    /// 넉백 코루틴
    /// </summary>
    /// <param name="pushDirection"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    public IEnumerator KnockBackCoroutine(float knockBackDistance)
    {
        float time=0;
        Vector3 startPosition = transform.position;
        while (Vector3.Distance(transform.position, startPosition) < knockBackDistance)
        {
            time += Time.deltaTime;
            yield return null;
        }
        EndKnockBack();
    }

    public void EndKnockBack()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        isPushed = false;
        GetComponent<Collider>().isTrigger = true;
    }

    public IEnumerator KnockBackCoolTime(float knockBackCoolTime = Constants.KNOCKBACK_COOLTIME)
    {
        yield return new WaitForSeconds(knockBackCoolTime);
        _knockBackCoroutine = null;
    }
    #endregion
}
