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
        var hitDamagable = Physics.Raycast(origin, dir.normalized, out RaycastHit rch, 100, ~LayerMask.GetMask("SeeThrough", "Pickable", "Ignore Raycast"));
        var col = rch.collider;
        var dist = rch.distance;
        objDist = dist;

        if (hitDamagable)
        {
            Debug.DrawRay(origin, dir * dist, Color.magenta, 3);

            var appliableDamage = GetDamageByPellet(damage, pellets);

            if (col.GetComponent(typeof(IDamageable)) is IDamageable damageable)
            {
                damageable.TakeDamage(Mathf.RoundToInt(appliableDamage), DamageTypes.Bullet);
            }
            else
            {

                //parts on hit
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

/// <summary>
/// ~LayerMask: chocar contra todo excepto layerMask
/// LayerMask: solo contra máscara
/// </summary>
public struct HitscanLayers
{
    public static int DamagableLayerMask()
    {
        return (LayerMask.GetMask("Enemy", "Destructible"));
    }

    public static int BlockerLayerMask()
    {
        return ~(LayerMask.GetMask("Unrenderizable", "Ignore Raycast", "Pickable"));
    }
}
