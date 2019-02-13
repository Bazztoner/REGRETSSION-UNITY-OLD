using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitscanRay
{
    public float objDist;

    public HitscanRay(Vector3 origin, Vector3 dir, float damage, int maxEnemies)
    {
        var enemiesHit = Physics.RaycastAll(origin, dir.normalized, 100, ~LayerMask.GetMask("SeeThrough")).Where(x => x.collider.gameObject.LayerMatchesWith("Enemy")).Take(maxEnemies).ToArray();
        float objDist;

        if (enemiesHit.Any())
        {
            for (int i = 0; i < enemiesHit.Length; i++)
            {
                var col = enemiesHit[i].collider;
                var dist = enemiesHit[i].distance;

                Debug.DrawRay(origin, dir * dist, Color.red, 3);

                if (col.GetComponent(typeof(IDamageable)) is IDamageable damageable)
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
