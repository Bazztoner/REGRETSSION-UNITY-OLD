using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MafiaSoundModule : EnemySoundModule
{
    public AudioClip[] enemyFounds;

    public override void OnEnemyFound()
    {
        PlaySound(enemyFounds[Random.Range(0, enemyFounds.Length)]);
    }
}
