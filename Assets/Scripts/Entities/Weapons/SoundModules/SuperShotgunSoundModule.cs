using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SuperShotgunSoundModule : WeaponSoundModuleBase
{
    public AudioClip insertShell, breakOpen, breakClose;

    public virtual void OnMagOut()
    {
        PlaySound(breakOpen);
    }

    public virtual void OnMagIn()
    {
        PlaySound(insertShell);
    }

    public virtual void OnMagTap()
    {
        PlaySound(breakClose);
    }
}
