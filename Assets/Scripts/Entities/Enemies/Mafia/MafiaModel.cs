using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MafiaModel : Enemy
{
    Mafia _logicModule;
    EnemyWeaponRanged _wpn;
    public int damage, dropChance;
    public AmmoPickupBase bulletDrop;

    void Awake()
    {
        _logicModule = GetComponent<Mafia>();
    }

    protected override void Start()
    {
        base.Start();
        _wpn = GetComponentInChildren<EnemyWeaponRanged>();
        _wpn.Configure(damage, this, _logicModule.LineOfSightModule.Target.GetComponent<PlayerController>());
    }

    public override void Die()
    {
        var dirToTarget = _logicModule.player.transform.position - transform.position;

        var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

        var frontalHit = angleToTarget < 90;

        _logicModule.Die(frontalHit);

        if (Random.Range(0, 101) <= dropChance)
        {
            GameObject.Instantiate(bulletDrop, transform.position, Quaternion.identity);
        }
    }

    public override void TakeDamage(int dmg, string damageType)
    {
        base.TakeDamage(dmg, damageType);
        _logicModule.OnTakeDamage(); 

    }

    public override void AttackStart()
    {
        _wpn.AttackStart();
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