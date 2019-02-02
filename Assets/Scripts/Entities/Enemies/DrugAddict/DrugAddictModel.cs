using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrugAddictModel : Enemy
{
    DrugAddict _logicModule;

    void Awake()
    {
        _logicModule = GetComponent<DrugAddict>();
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
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
        GetComponent<DrugAddict>().AttackEnd();
    }

    public override void FlinchEnd()
    {
        GetComponent<DrugAddict>().FlinchEnd();
    }
}
