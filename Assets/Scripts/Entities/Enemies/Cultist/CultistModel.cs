using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CultistModel : Enemy
{
    Cultist _logicModule;

    void Awake()
    {
        _logicModule = GetComponent<Cultist>();
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

    public override void AttackEnd()
    {
        _logicModule.AttackEnd();
    }

    public override void FlinchEnd()
    {
        _logicModule.FlinchEnd();
    }
}
