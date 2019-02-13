using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyMeleeHitbox : MonoBehaviour
{
    Collider _col;
    Enemy _owner;
    PlayerController _target;
    int _damage;

    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.enabled = false;
    }

    public void Configure(int damage, Enemy owner, PlayerController target)
    {
        _damage = damage;
        _owner = owner;
        _target = target;
        _col.enabled = true;
    }

    public void AttackEnd()
    {
        _col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(_target.gameObject))
        {
            _target.TakeDamage(_damage, DamageTypes.Melee);
            AttackEnd();
        }
    }
}
