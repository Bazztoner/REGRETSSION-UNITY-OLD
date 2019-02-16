using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScarSoundModule : WeaponSoundModuleBase
{
    public AudioClip magIn, magOut, magTap, boltCatch;

    public virtual void OnMagIn()
    {
        PlaySound(magIn);
    }

    public virtual void OnMagOut()
    {
        PlaySound(magOut);
    }

    public virtual void OnMagTap()
    {
        PlaySound(magTap);
    }

    public virtual void OnBoltCatch()
    {
        PlaySound(boltCatch);
    }
}
