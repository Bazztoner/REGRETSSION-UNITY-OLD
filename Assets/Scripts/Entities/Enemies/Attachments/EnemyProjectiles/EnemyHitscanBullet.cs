using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyHitscanBullet
{
    public float objDist;
    int _damage;

    public EnemyHitscanBullet(Vector3 origin, Vector3 dir, int damage)
    {
        var hitDamagable = Physics.Raycast(origin, dir.normalized, out RaycastHit rch, 100, ~LayerMask.GetMask("SeeThrough"));
        var col = rch.collider;
        var dist = rch.distance;
        objDist = dist;
        _damage = damage;

        if (hitDamagable)
        {
            Debug.DrawRay(origin, dir * dist, Color.red, 3);

            if (col.GetComponent(typeof(IDamageable)) is IDamageable damageable && col.gameObject.LayerDifferentFrom("Enemy"))
            {
                damageable.TakeDamage(_damage, DamageTypes.Bullet);
            }
            else
            {
                //parts on hit
            }
        }
        else
        {
            objDist = 125;
            Debug.DrawRay(origin, dir * objDist, Color.white, 3);
        }
    }
}
