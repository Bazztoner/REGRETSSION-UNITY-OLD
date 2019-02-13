using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CultistModel : Enemy
{
    Cultist _logicModule;
    EnemyWeaponMelee _meleeWpn;
    EnemyWeaponRanged _rangedWpn;
    public int meleeDamage;
    public int rangedDamage;

    void Awake()
    {
        _logicModule = GetComponent<Cultist>();
    }

    protected override void Start()
    {
        base.Start();
        _meleeWpn = GetComponentInChildren<EnemyWeaponMelee>();
        _meleeWpn.Configure(meleeDamage, this, _logicModule.LineOfSightModule.Target.GetComponent<PlayerController>());
        _rangedWpn = GetComponentInChildren<EnemyWeaponRanged>();
        _rangedWpn.Configure(rangedDamage, this, _logicModule.LineOfSightModule.Target.GetComponent<PlayerController>());
    }

    public override void TakeDamage(int dmg, string damageType)
    {
        base.TakeDamage(dmg, damageType);
        _logicModule.OnTakeDamage();
    }

    public override void Die()
    {
        var dirToTarget = _logicModule.player.transform.position - transform.position;

        var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

        var frontalHit = angleToTarget < 90;

        _logicModule.Die(frontalHit);
    }

    /// <summary>
    /// Melee
    /// </summary>
    public override void AttackStart()
    {
        _meleeWpn.AttackStart();
    }

    public override void AttackEnd()
    {
        _logicModule.AttackEnd();
    }

    /// <summary>
    /// Melee
    /// </summary>
    public void RangedAttackStart()
    {
        _rangedWpn.AttackStart();
    }

    public void RangedAttackEnd()
    {
        _rangedWpn.AttackEnd();
    }

    public override void FlinchEnd()
    {
        _logicModule.FlinchEnd();
    }
}
