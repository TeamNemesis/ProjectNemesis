using UnityEngine;

public class PlayerBladeSpecialAttacker : PlayerSpecialAttacker
{
    public override WeaponType WeaponType => WeaponType.Blade;

    [Header("Grapple Settings")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private LayerMask grappleMask; // 벽, 기둥 등

    [SerializeField] private LineRenderer line; // 프리팹

    private Vector3 targetPoint;
    private Coroutine grappleRoutine;
    private bool isMoving = false;
    private bool hasHit = false;

    public override void StartAttackRoutine()
    {
        base.StartAttackRoutine();
    }

    protected override void Fire()
    {
        throw new System.NotImplementedException();
    }
}