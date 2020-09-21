using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FNFALSoundModule : WeaponSoundModuleBase
{
    public AudioClip magIn, magOut, startReload, boltCatch;

    public virtual void OnMagIn()
    {
        PlaySound(magIn);
    }

    public virtual void OnMagOut()
    {
        PlaySound(magOut);
    }

    public virtual void OnStartReload()
    {
        PlaySound(startReload);
    }

    public virtual void OnBoltCatch()
    {
        PlaySound(boltCatch);
    }
}
