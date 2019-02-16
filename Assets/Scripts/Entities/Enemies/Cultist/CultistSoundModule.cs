using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CultistSoundModule : EnemySoundModule
{
    public AudioClip rage;

    public void OnRage()
    {
        if (Random.Range(0, 100) <= 35) PlaySound(rage);
    }
}
