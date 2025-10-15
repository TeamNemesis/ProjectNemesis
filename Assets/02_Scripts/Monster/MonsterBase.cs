using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : CharacterModelBase
{
    [Header("Stats")]
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackDelay = 0.5f;
    [SerializeField] public string targetTag = Constants.TAG_PLAYER;



    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform player;




    public override void Initialize()
    {
        base.Initialize();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
        if (playerObj != null)
            player = playerObj.transform;

        SetCurrentHp(maxHealth);

        debuffHandler.InitializeMonster(agent);
    }



    protected bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        int mask = LayerMask.GetMask(targetTag, Constants.LAYER_MASK_WALL);

        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, dir, out RaycastHit hit, dist, mask))
        {
            if (hit.transform.CompareTag(targetTag))
            {
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

    #region Test
    public void Start()
    {
        Initialize();

    }

    public void Use(Transform transform)
    {
        Debug.Log("monster poison");


        DebuffHandler.DebuffData poison = new DebuffHandler.DebuffData(Constants.DEBUFF_POISON, 6f, 5f);



        DebuffHandler debuffHandler = transform.GetComponent<DebuffHandler>();
        debuffHandler.ApplyDebuff(poison);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Use(player);
        }
    }
    #endregion
}
