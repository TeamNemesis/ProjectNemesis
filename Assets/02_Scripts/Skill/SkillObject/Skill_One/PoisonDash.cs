using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PoisonDashData
{
    public Player player;
    public float damage;
    public float extent;

    public PoisonDashData(Player player, float damage, float extent)
    {
        this.player = player;
        this.damage = damage;
        this.extent = extent;
    }
}

public class PoisonDash : AreaDamageBase,IInitializePoolable
{
    [SerializeField]
    private float _dashDamage;
    private Player _player;
    public float dashDamage { get { return _dashDamage; } }


    public void Initialize()
    {
        CheckTarget();
        StartCoroutine(ReleaseCoroutine());
    }


    public override void ActiveSkill(Transform target)
    {
        _player.playerModel.Heal(1);
        target.GetComponent<CharacterModelBase>().TakeDamage(dashDamage);
    }

    IEnumerator ReleaseCoroutine()
    {
        yield return new WaitForSeconds(Constants.SKILL_REMAIN);
        GameManager.Instance.PoolManager.ReleaseToPoolByInterface(this);
    }

    public void Initialize(object data)
    {
        if(data is PoisonDashData dashData)
        {
            _player = dashData.player;
            _dashDamage = dashData.damage;
            _areaExtent = dashData.extent * GameManager.Instance.PlayerStatManager.playerAreaExtent;
        }
    }
}
