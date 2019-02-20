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
    bool _evading = false;

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

    public void SetEvade(bool set)
    {
        _evading = set;
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

        _wpn.gameObject.SetActive(false);
    }

    public override void TakeDamage(int dmg, string damageType)
    {
        if (_evading) return;

        base.TakeDamage(dmg, damageType);
        _logicModule.OnTakeDamage(); 

    }

    public override void AttackStart()
    {
        _wpn.AttackStart();
    }

    public override void AttackEnd()
    {
        _wpn.AttackEnd();
        _logicModule.AttackEnd();
    }

    public override void EvadeEnd()
    {
        _logicModule.EvadeEnd();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponentInParent<Door>() is Door door)
        {
            var doorCondition = !door.locked && !door.Opened;
            var stateCondition = StateMatchesWith("Chase", "Evade", "ReturnToStartPosition", "Attack");

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