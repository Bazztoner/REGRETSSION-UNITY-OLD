using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class M1GarandSoundModule : WeaponSoundModuleBase
{
    public AudioClip reload;
    public AudioClip lastShotSound;

    public virtual void OnReload()
    {
        PlaySound(reload);
    }

    public virtual void OnLastShot()
    {
        PlaySound(lastShotSound);
    }
}
