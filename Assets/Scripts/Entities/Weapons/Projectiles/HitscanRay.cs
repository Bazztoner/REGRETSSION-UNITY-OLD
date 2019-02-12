using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitscanRay
{
    public float objDist;

    public HitscanRay(Vector3 origin, Vector3 dir, float damage, int maxEnemies)
    {
        var damagableMask = HitscanLayers.DamagableLayerMask();
        var enemiesHit = Physics.RaycastAll(origin, dir.normalized, 100, damagableMask).Take(maxEnemies).ToArray();
        float objDist;

        if (enemiesHit.Any())
        {
            for (int i = 0; i < enemiesHit.Length; i++)
            {
                var col = enemiesHit[i].collider;
                var dist = enemiesHit[i].distance;

                Debug.DrawRay(origin, dir * dist, Color.red, 3);

                var damageable = col.GetComponent(typeof(IDamageable)) as IDamageable;
                if (damageable != null)
                {
                    damageable.TakeDamage(Mathf.RoundToInt(damage), DamageTypes.Bullet);
                }
            }
        }
        else
        {
            objDist = 125;
            Debug.DrawRay(origin, dir * objDist, Color.red, 3);
        }
    }
}
