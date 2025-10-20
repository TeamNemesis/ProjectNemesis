using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PoisonDash : AreaDamageBase, IPoolable
{
    [SerializeField]
    private int _dashDamage;
    public int dashDamage { get { return _dashDamage; } }

    public void SetDashDamage(int dashDamage)
    {
        _dashDamage = dashDamage;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Initialize()
    {
        CheckTarget();
        StartCoroutine(ReleaseCoroutine());
    }

    public void ReleaseObject()
    {

    }

    public override void ActiveSkill(Transform target)
    {
        GameManager.Instance.player.Heal(1);
        target.GetComponent<CharacterModelBase>().TakeDamage(dashDamage);
    }

    IEnumerator ReleaseCoroutine()
    {
        yield return new WaitForSeconds(Constants.SKILL_REMAIN);
        ObjectPool.Instance.ReleaseToPoolByInterface(this);
    }


}
