using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyWeaponMelee : EnemyWeaponBase
{
    EnemyMeleeHitbox _hitbox;

    void Awake()
    {
        _hitbox = GetComponentInChildren<EnemyMeleeHitbox>();
    }

    public override void AttackStart()
    {
        _hitbox.Configure(_damage, _owner, _target);
    }

    public override void AttackEnd()
    {
        _hitbox.AttackEnd();
    }
}
