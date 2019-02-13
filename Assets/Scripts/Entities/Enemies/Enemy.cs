using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    public int maxHp;

    int _hp;

    public int HP
    {
        get { return _hp; }
        private set
        {
            _hp = value;
            if (_hp > maxHp) _hp = maxHp;
            else if (_hp < 0) _hp = 0;
        }
    }

    protected virtual void Start()
    {
        _hp = maxHp;
    }

    public virtual void TakeDamage(int dmg, string damageType)
    {
        UpdateHP(dmg, false);

        if (HP <= 0) Die();
    }

    public abstract void Die();

    public abstract void AttackStart();

    public abstract void AttackEnd();

    public virtual void EvadeEnd()
    {

    }

    public virtual void FlinchEnd()
    {

    }

    public void UpdateHP(int amount, bool add = true)
    {
        HP = add ? HP += amount : HP -= amount;
    }
}
