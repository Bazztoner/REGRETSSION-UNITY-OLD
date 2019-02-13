using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class EnemyWeaponBase : MonoBehaviour
{
    protected Enemy _owner;
    protected PlayerController _target;
    protected int _damage;

    public virtual void Configure(int damage, Enemy owner, PlayerController target)
    {
        _damage = damage;
        _owner = owner;
        _target = target;
    }
    public abstract void AttackStart();
    public abstract void AttackEnd();
}
