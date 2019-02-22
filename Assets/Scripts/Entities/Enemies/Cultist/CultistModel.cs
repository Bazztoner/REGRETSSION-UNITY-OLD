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

        _meleeWpn.gameObject.SetActive(false);
        _rangedWpn.gameObject.SetActive(false);
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
        _meleeWpn.AttackEnd();
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

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponentInParent<Door>() is Door door)
        {
            var doorCondition = !door.locked && !door.Opened;
            var stateCondition = StateMatchesWith("Chase", "Search", "Berserk", "Attack");

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
