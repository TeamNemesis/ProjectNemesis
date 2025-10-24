using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterSize
{
    SMALL,
    MIDDLE,
    BIG
}


public class MonsterBase : CharacterModelBase
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


    #region 넉백
    [SerializeField] private float _knockBackDamage;
    [SerializeField] private Coroutine _knockBackCoroutine;
    #endregion
    [SerializeField] protected Transform player;


    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform _target;

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


    private void Start()
    {
        Initialize();
    }


    public override void Initialize()
    {
        base.Initialize();
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;

        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj != null)
            _target = targetObj.transform;

        SetCurrentHp(maxHealth);

        debuffHandler.InitializeMonster(agent);
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


    #region 넉백

    private void OnCollisionEnter(Collision collision)
    {
        if (isPushed)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(Constants.LAYER_MASK_WALL))
            {
                Debug.Log("충돌");
                TakeDamage(GameManager.Instance.PlayerStatManager.knockBackDamage * GameManager.Instance.PlayerStatManager.knockBackDamageMulti * GameManager.Instance.PlayerStatManager.totalMultiDamage);
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
        TakeDamage(damage * GameManager.Instance.PlayerStatManager.totalMultiDamage);
        if (monsterSize == MonsterSize.BIG)
        {
            return;
        }
        GetComponent<Collider>().isTrigger = false;
        isPushed = true;
        debuffHandler.ApplyDebuff(DebuffHandler.DebuffData.CreateStun(0.5f));
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(pushDirection, ForceMode.VelocityChange);

        if (_knockBackCoroutine != null)
        {
            StopCoroutine(_knockBackCoroutine);
        }
        _knockBackCoroutine = StartCoroutine(KnockBackCoroutine(knockBackDistance));
    }


    /// <summary>
    /// 넉백 코루틴
    /// </summary>
    /// <param name="pushDirection"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    public IEnumerator KnockBackCoroutine(float knockBackDistance)
    {
        Vector3 startPosition = transform.position;
        while (Vector3.Distance(transform.position, startPosition) < knockBackDistance)
        {
            yield return null;
        }
        GetComponent<Rigidbody>().isKinematic = true;
        isPushed = false;
        GetComponent<Collider>().isTrigger = true;
        _knockBackCoroutine = null;
    }
    #endregion
}
