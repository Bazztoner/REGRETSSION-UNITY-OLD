using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyWeaponRanged : EnemyWeaponBase
{
    EnemyRangedMuzzle _muzzle;

    void Awake()
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
