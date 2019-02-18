using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrugAddictModel : Enemy
{
    DrugAddict _logicModule;
    public int attackDamage;
    EnemyWeaponMelee _wpn;

    void Awake()
    {
        _logicModule = GetComponent<DrugAddict>();
    }

    protected override void Start()
    {
        base.Start();
        _wpn = GetComponentInChildren<EnemyWeaponMelee>();
        _wpn.Configure(attackDamage, this, _logicModule.LineOfSightModule.Target.GetComponent<PlayerController>());
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
        _wpn.gameObject.SetActive(false);
    }

    public override void AttackStart()
    {
        _wpn.AttackStart();
    }

    public override void AttackEnd()
    {
        _logicModule.AttackEnd();
        _wpn.AttackEnd();
    }

    public override void FlinchEnd()
    {
        _logicModule.FlinchEnd();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponentInParent<Door>() is Door door)
        {
            var doorCondition = !door.locked && !door.Opened;
            var stateCondition = StateMatchesWith("Chase", "Attack");

            if (doorCondition && stateCondition) door.Use();
        }
    }

    bool StateMatchesWith(params string[] stateNames)
    {
        var list = new FList<bool>();

        foreach (var item in stateNames)
        {
            list += _logicModule.GetCurrentState() == item;
        }

        return list.Any();
    }
}
