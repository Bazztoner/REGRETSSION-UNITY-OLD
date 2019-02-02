using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour, IDamageable
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

    void Start()
    {
        _hp = maxHp;
    }

    public virtual void TakeDamage(int dmg)
    {
        UpdateHP(dmg, false);

        if (HP <= 0) Die();
    }

    public virtual void Die()
    {
        //die
    }

    public virtual void AttackEnd()
    {

    }

    public virtual void FlinchEnd()
    {

    }

    public virtual void EvadeEnd()
    {

    }

    public void UpdateHP(int amount, bool add = true)
    {
        HP = add ? HP += amount : HP -= amount;
    }
}
