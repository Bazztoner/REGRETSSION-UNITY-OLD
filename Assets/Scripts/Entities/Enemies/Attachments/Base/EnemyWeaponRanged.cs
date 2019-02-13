using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyWeaponRanged : EnemyWeaponBase
{
    protected EnemyRangedMuzzle _muzzle;
    protected Vector3 _storedDir;

    protected virtual void Awake()
    {
        _muzzle = GetComponentInChildren<EnemyRangedMuzzle>();
    }

    public override void AttackStart()
    {

    }

    public override void AttackEnd()
    {

    }
}
