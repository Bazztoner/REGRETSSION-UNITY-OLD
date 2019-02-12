using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeHitscan
{
    public MeleeHitscan(Vector3 origin, Vector3 dir, float damage, Vector3 size)
    {
        var damagableMask = HitscanLayers.DamagableLayerMask();
        RaycastHit rch;
        var hitDamageable = Physics.Raycast(origin, dir, out rch, 2, damagableMask);
        var col = rch.collider;

        if (hitDamageable)
        {
            var damageable = col.GetComponent(typeof(IDamageable)) as IDamageable;
            if (damageable != null)
            {
                damageable.TakeDamage(Mathf.RoundToInt(damage), DamageTypes.Melee);
            }
        }

        Debug.DrawRay(origin, dir, Color.blue, 2);
    }
}

