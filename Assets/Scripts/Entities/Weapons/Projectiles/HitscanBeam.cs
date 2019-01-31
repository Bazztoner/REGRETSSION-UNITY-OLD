using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitscanBeam
{
    public HitscanBeam(Vector3 origin, Vector3 dir, float damage, Vector3 size)
    {
        var damagableMask = HitscanLayers.BlockerLayerMask();
        var enemiesHit = Physics.BoxCastAll(origin + new Vector3(0, 0,size.z), size, dir, Quaternion.identity, size.z, damagableMask);

        if (enemiesHit.Any())
        {
            for (int i = 0; i < enemiesHit.Length; i++)
            {
                var col = enemiesHit[i].collider;
                var dist = enemiesHit[i].distance;

                var damageable = col.GetComponent(typeof(IDamageable)) as IDamageable;
                if (damageable != null)
                {
                    damageable.TakeDamage(Mathf.RoundToInt(damage));
                }
            }
        }

        Debug.DrawRay(origin + new Vector3(size.x, size.y, 0), dir * size.z, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(-size.x, size.y, 0), dir * size.z, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(size.x, -size.y, 0), dir * size.z, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(-size.x, -size.y, 0), dir * size.z, Color.blue, 3);
    }

    public HitscanBeam(Vector3 origin, Vector3 dir, float damage, float xSize, float ySize, float zSize)
    {
        var damagableMask = HitscanLayers.BlockerLayerMask();
        var enemiesHit = Physics.BoxCastAll(origin + new Vector3(0, 0, zSize), new Vector3(xSize, ySize, zSize), dir, Quaternion.identity, zSize, damagableMask);

        if (enemiesHit.Any())
        {
            for (int i = 0; i < enemiesHit.Length; i++)
            {
                var col = enemiesHit[i].collider;
                var dist = enemiesHit[i].distance;

                var damageable = col.GetComponent(typeof(IDamageable)) as IDamageable;

                damageable.TakeDamage(Mathf.RoundToInt(damage));
            }
        }

        Debug.DrawRay(origin + new Vector3(xSize, ySize, 0), dir * zSize, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(xSize, -ySize, 0), dir * zSize, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(-xSize, ySize, 0), dir * zSize, Color.blue, 3);
        Debug.DrawRay(origin + new Vector3(-xSize, -ySize, 0), dir * zSize, Color.blue, 3);
    }
}
