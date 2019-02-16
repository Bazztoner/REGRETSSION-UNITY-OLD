using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrugAddictSoundModule : EnemySoundModule
{
    public AudioClip[] flinchs;
    public AudioClip[] deaths;

    public void OnFlinch()
    {
        PlaySound(flinchs[Random.Range(0, flinchs.Length)]);
    }

    public override void OnDeath()
    {
        PlaySound(deaths[Random.Range(0, deaths.Length)]);
    }
}
