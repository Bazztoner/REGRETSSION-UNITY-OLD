using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    void TakeDamage(int damage, string damageType);
}

public struct DamageTypes
{
    public const string Melee = "MeleeDamageType";
    public const string Bullet = "BulletDamageType";
    public const string Explosive = "ExplosiveDamageType";
}
