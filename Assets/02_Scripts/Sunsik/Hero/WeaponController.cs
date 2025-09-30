
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] Collider _weaponCollider;
    HashSet<Collider> _alreadyHits = new();

    public void EnableWeaponCollider()
    {
        _alreadyHits.Clear();
        _weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        _weaponCollider.enabled = false;
        _alreadyHits.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_alreadyHits.Contains(other)) return;
        _alreadyHits.Add(other);
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeHit(1);
        }
    }
}