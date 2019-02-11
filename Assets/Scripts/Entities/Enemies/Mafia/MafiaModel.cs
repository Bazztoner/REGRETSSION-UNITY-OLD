using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MafiaModel : Enemy
{
    Mafia _logicModule;

    void Awake()
    {
        _logicModule = GetComponent<Mafia>();
    }

    public override void Die()
    {
        var dirToTarget = _logicModule.player.transform.position - transform.position;

        var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

        var frontalHit = angleToTarget < 90;

        _logicModule.Die(frontalHit);
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        _logicModule.OnTakeDamage(); 

    }

    public override void AttackEnd()
    {
        _logicModule.AttackEnd();
    }

    public override void EvadeEnd()
    {
        _logicModule.EvadeEnd();
    }
}