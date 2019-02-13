using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ThrowingKnife : EnemyWeaponRanged
{
    [SerializeField] float _shootDelay;
    [SerializeField] DMM_ThrowingKnife _projectile;

    public override void AttackStart()
    {
        _storedDir = (_target.transform.position - _muzzle.transform.position).normalized;
        Invoke("ManageProjectile", _shootDelay);
    }

    public override void AttackEnd()
    {
        //CancelInvoke();
    }

    void ManageProjectile()
    {
        var knf = GameObject.Instantiate(_projectile);
        knf.Configure(_muzzle.transform.position, _storedDir, _damage);

    }
}
