using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageDummy : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage, string damageType)
    {
        print("TAKEN " + damage + " FROM " + damageType);
    }
}
