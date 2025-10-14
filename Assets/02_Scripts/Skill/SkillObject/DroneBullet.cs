using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = Constants.DRONE_BULLET_SPEED;
    [SerializeField]
    private float _bulletTime = Constants.DRONE_BULLET_TIME;
    [SerializeField]
    private float _bulletDamage;

    public void SetDamage(float damage)
    {
        _bulletDamage = damage;
    }


		private void Start()
		{
        //TODO 오브젝트 풀 적용 여부
        // 일정 시간 이후 총알 제거
				Destroy(gameObject,_bulletTime);
		}

		private void Update()
		{
				transform.Translate(Vector3.forward*_bulletSpeed*Time.deltaTime);
		}

		private void OnTriggerEnter(Collider other)
		{
				if (other.CompareTag(Constants.TAG_MONSTER))
				{
						IDamageAble damageable = other.GetComponent<IDamageAble>();
						if (damageable != null)
						{
								damageable.TakeDamage(_bulletDamage);
								Debug.Log("Monster Hit! Damage " + _bulletDamage);
						}
						Destroy(gameObject);
				}
				else if(!other.CompareTag(Constants.TAG_PLAYER))
				{
						Destroy(gameObject);
				}
		}
}
