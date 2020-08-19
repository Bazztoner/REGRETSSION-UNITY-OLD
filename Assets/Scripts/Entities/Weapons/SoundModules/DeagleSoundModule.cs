using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeagleSoundModule : WeaponSoundModuleBase
{
    public AudioClip magIn, magOut, boltCatch;

    public virtual void OnMagIn()
    {
        PlaySound(magIn);
    }

    public virtual void OnMagOut()
    {
        PlaySound(magOut);
    }

    public virtual void OnBoltCatch()
    {
        PlaySound(boltCatch);
    }
}
