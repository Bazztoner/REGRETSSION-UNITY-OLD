using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestructibleBase : MonoBehaviour, IDamageable
{
    [SerializeField] float _life;

    public void TakeDamage(int damage, string damageType)
    {
        if (damageType == DamageTypes.Melee || damageType == DamageTypes.Explosive)
        {
            gameObject.SetActive(false);
        }
    }
}
