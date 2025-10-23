using UnityEngine;


public class RedShiftData
{
    public Vector3 moveDir;
    public float moveSpeed;
    public float shiftExtend;
    public float collisionDamage;
}

public class RedShift : AreaDamageBase, IInitializePoolable
{
    /// <summary>
    /// 방향
    /// </summary>
    [SerializeField]
    private Vector3 direction;

    /// <summary>
    /// 속도
    /// </summary>
    [SerializeField]
    private float speed = 10f;

    /// <summary>
    /// 스킬 데미지
    /// </summary>
    [SerializeField]
    private float _damage = 20f;

    public void Initialize(object data)
    {
        if(data is RedShiftData redShiftData)
        {
            direction = redShiftData.moveDir;
            speed = redShiftData.moveSpeed;
            SetAreaExtent(redShiftData.shiftExtend* GameManager.Instance.PlayerStatManager.playerAreaExtent);
            _damage = redShiftData.collisionDamage;
        }

        transform.localScale = Vector3.one * areaExtent;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Constants.LAYER_MASK_WALL))
        {
            // 벽에 닿으면 파동 종료
            ObjectPool.Instance.ReleaseToPoolByInterface(this);
        }


        else if (other.CompareTag(Constants.TAG_MONSTER))
        {
            MonsterBase monster = other.GetComponent<MonsterBase>();
            if (monster != null)
            {
                Vector3 direction = monster.transform.position - transform.position;
                direction.Normalize();


                monster.KnockBackEnemy(direction, _damage, 5f);

            }
        }
    }

}
