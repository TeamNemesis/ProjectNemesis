using UnityEngine;


public struct RedShiftData
{
    public float moveSpeed;
    public float collisionDamage;
}

public class RedShift : AreaDamageBase, IInitializePoolable
{

    private float speed = 10f;
    private float _damage = 20f;
    private Vector3 direction;

    public void Initialize(object data)
    {
        if(data is RedShiftData redShiftData)
        {

        }
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
