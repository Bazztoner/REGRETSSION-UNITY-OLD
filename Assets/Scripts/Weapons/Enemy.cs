using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour,IDamageable
{
    public float hp = 200;

    int pl = 0;

    public void ResetHP()
    {

    }

    void Start()
    {
        StartCoroutine(Puto());
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        pl++;
    }

    public IEnumerator Puto()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            if (pl >0 && hp != 200)
            {
                print("daño " + pl * 8.33333f + " || plt " + pl);
                pl = 0;
                hp = 200;
            }
        }
    }
}
