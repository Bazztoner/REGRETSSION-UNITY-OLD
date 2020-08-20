using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitscanBeam
{
    public HitscanBeam(Vector3 origin, Vector3 dir, float damage, Vector3 size)
    {
        var enemiesHit = Physics.BoxCastAll(origin + new Vector3(0, 0,size.z), size, dir, Quaternion.identity, size.z).Where(x => x.collider.gameObject.LayerMatchesWith("Enemy")).ToArray();

        if (enemiesHit.Any())
        {
            for (int i = 0; i < enemiesHit.Length; i++)
            {
                var col = enemiesHit[i].collider;
                var dist = enemiesHit[i].distance;

                if (col.GetComponent(typeof(IDamageable)) is IDamageable damageable)
                {
                    damageable.TakeDamage(Mathf.RoundToInt(damage), DamageTypes.Laser);
                }
            }
        }

        Debug.DrawRay(origin + new Vector3(size.x, size.y, 0), dir * size.z, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(-size.x, size.y, 0), dir * size.z, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(size.x, -size.y, 0), dir * size.z, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(-size.x, -size.y, 0), dir * size.z, Color.blue, 3);
    }
}
