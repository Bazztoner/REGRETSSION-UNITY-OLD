using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeHitscan
{
    public MeleeHitscan(Vector3 origin, Vector3 dir, float damage, Vector3 size)
    {
        var hitDamageable = Physics.Raycast(origin, dir, out RaycastHit rch, 2);
        var col = rch.collider;

        if (hitDamageable)
        {
            if (col.GetComponent(typeof(IDamageable)) is IDamageable damageable)
            {
                damageable.TakeDamage(Mathf.RoundToInt(damage), DamageTypes.Melee);
            }
        }

        Debug.DrawRay(origin, dir, Color.blue, 2);
    }
}

