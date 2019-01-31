using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrugAddictModelModule : Enemy
{
    public override void Die()
    {
        var logicModule = GetComponent<DrugAddict>();

        var dirToTarget = logicModule.player.transform.position - transform.position;

        var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

        var frontalHit = angleToTarget < 90;

        logicModule.Die(frontalHit);
    }

    public override void AttackEnd()
    {
        GetComponent<DrugAddict>().AttackEnd();
    }
}
