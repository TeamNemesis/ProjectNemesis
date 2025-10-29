using JetBrains.Annotations;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;


public class GravityFlareRocketData
{
    public Vector3 dir;
    public GravityFlareRocketExplosionData explosionData;
    public GravityFlareRocketExplosion explosionPrefab;

    public GravityFlareRocketData(Vector3 dir, GravityFlareRocketExplosionData data, GravityFlareRocketExplosion explosionPrefab)
    {
        this.dir = dir;
        this.explosionData = data;
        this.explosionPrefab = explosionPrefab;
    }
}

public class GravityFlareRocket : PoolableObject,IInitializePoolable
{
    private Vector3 dir;
    [SerializeField]
    private float moveSpeed;

    private GravityFlareRocketExplosionData explosionData;
    [SerializeField]
    private GravityFlareRocketExplosion _explosionPrefab;
    public void Initialize(object data)
    {
       if(data is GravityFlareRocketData rocketData)
        {
            dir = rocketData.dir;
            explosionData = rocketData.explosionData;
            _explosionPrefab = rocketData.explosionPrefab;
        }
    }

    private void Update()
    {
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            return;
        }
        Vector3 position = transform.position;
        position.y = 0;
        GameManager.Instance.PoolManager.GetFromPool(
            _explosionPrefab, position, Quaternion.identity, null, explosionData).GetComponent<GravityFlareRocketExplosion>().Initialize();
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }

}
