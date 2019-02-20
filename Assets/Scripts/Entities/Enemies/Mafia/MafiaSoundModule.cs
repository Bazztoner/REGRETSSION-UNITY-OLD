using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MafiaSoundModule : EnemySoundModule
{
    public AudioClip[] enemyFounds;
    [SerializeField] float _shootDelay;

    public override void OnEnemyFound()
    {
        PlaySound(enemyFounds[Random.Range(0, enemyFounds.Length)]);
    }

    public override void OnAttack()
    {
        Invoke("DelayedAttack", _shootDelay);
    }

    void DelayedAttack()
    {
        PlaySound(attack);
    }
}
