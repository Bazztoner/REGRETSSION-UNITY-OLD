using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitscanBullet
{
    public float objDist;

    public HitscanBullet(Vector3 origin, Vector3 dir, float damage, int inputPellets)
    {
        var pellets = Mathf.Max(1, inputPellets);

        var rch = new RaycastHit();
        var damagableMask = HitscanLayers.BlockerLayerMask();
        var hitDamagable = Physics.Raycast(origin, dir.normalized, out rch, 100, damagableMask);
        var col = rch.collider;
        var dist = rch.distance;
        objDist = dist;

        if (hitDamagable)
        {
            Debug.DrawRay(origin, dir * dist, Color.magenta, 3);

            var appliableDamage = GetDamageByPellet(damage, pellets);

            var damageable = col.GetComponent(typeof(IDamageable)) as IDamageable;
            if (damageable != null)
            {
                damageable.TakeDamage(Mathf.RoundToInt(appliableDamage));
            }
        }
        else
        {
            objDist = 125;
            Debug.DrawRay(origin, dir * objDist, Color.magenta, 3);
        }
    }

    public float GetDamageByPellet(float damage, float pellets)
    {
        return damage / pellets;
    }
}

public struct HitscanLayers
{
    public static int DamagableLayerMask()
    {
        return LayerMask.GetMask("Enemy");
    }

    public static int BlockerLayerMask()
    {
        return ~(LayerMask.GetMask("Unrenderizable", "Ignore Raycast"));
    }
}
