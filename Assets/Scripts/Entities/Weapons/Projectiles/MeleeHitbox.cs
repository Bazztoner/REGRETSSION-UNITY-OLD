using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeHitbox : MonoBehaviour
{
    float _damage;

	public void Configure(float damage)
    {
        _damage = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.LayerMatchesWith("Enemy", "Destructible"))
        {
            var damageable = other.GetComponent(typeof(IDamageable)) as IDamageable;
            if (damageable != null)
            {
                damageable.TakeDamage(Mathf.RoundToInt(_damage), DamageTypes.Melee);
            }
        }
    }
}
