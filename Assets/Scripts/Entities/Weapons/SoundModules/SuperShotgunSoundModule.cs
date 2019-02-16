using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SuperShotgunSoundModule : WeaponSoundModuleBase
{
    public AudioClip magIn, magOut, magTap;

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
}
