using System;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour, IDamageAble
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackDelay = 0.5f;
    [SerializeField] public bool isStunned = false;
    [SerializeField] public bool isDead = false;
    [SerializeField] public string targetTag = "Player";

    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform player;
    [SerializeField] protected DebuffHandler debuffHandler;

    public Action OnDieEvent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        debuffHandler = GetComponent<DebuffHandler>();

        GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
        if (playerObj != null)
            player = playerObj.transform;

        currentHealth = maxHealth;
    }

    protected virtual void Die()
    {
        if (OnDieEvent != null)
            OnDieEvent();

        isDead = true;
        Destroy(gameObject);
    }

    protected bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        int mask = LayerMask.GetMask(targetTag, "Wall");

        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, dir, out RaycastHit hit, dist, mask))
        {
            if (hit.transform.CompareTag(targetTag))
            {
                Debug.Log("playerCheck");
                return true;
            }
            return false;
        }
        return false;
    }

    protected void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // 과부하 디버프
        if (debuffHandler != null && debuffHandler.HasDebuff("과부하"))
        {
            int stacks = debuffHandler.GetStackCount("과부하");
            float bonus = 1f + (0.05f * stacks);
            damage *= bonus;
        }

        // 화상 디버프
        if (debuffHandler != null && debuffHandler.HasDebuff("화상"))
        {
            damage *= 2f;
        }

        currentHealth -= (int)damage;
        if (currentHealth <= 0)
            Die();
    }
}
