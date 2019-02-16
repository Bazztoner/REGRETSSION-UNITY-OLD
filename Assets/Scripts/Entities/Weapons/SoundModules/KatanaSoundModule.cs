using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KatanaSoundModule : WeaponSoundModuleBase
{
    public AudioClip enemyHit, wallHit, destructHit;

	public void OnEnemyHit()
    {
        PlaySound(enemyHit);
    }

    public void OnWallHit()
    {
        PlaySound(wallHit);
    }

    public void OnDestructibleHit()
    {
        PlaySound(destructHit);
    }
}
