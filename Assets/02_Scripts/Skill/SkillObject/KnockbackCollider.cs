using UnityEngine;

public struct KnockBackColliderData
{
    public Vector3 colliderSize;
    public float knockBackDamage;
    public Transform monsterTransform;

    public KnockBackColliderData(Vector3 size,float damage ,Transform transform)
    {
        colliderSize = size;
        knockBackDamage = damage;
        monsterTransform = transform;
    }
}


public class KnockbackCollider : PoolableObject,IInitializePoolable
{
    private Vector3 _colliderSize;

    private float _knockBackDamage;

    private Transform _parentTranform;

    [SerializeField]
    Transform testTransform;
    public void Start()
    {
        KnockBackColliderData data = new KnockBackColliderData(Vector3.one*2f,30f ,testTransform);
        Initialize(data);
    }

    public void Initialize(object data)
    {
        if(data is KnockBackColliderData colliderData)
        {
            _colliderSize = colliderData.colliderSize;
            _knockBackDamage = colliderData.knockBackDamage;
            _parentTranform = colliderData.monsterTransform;
        }
        transform.localScale = _colliderSize;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
				
		}
}
