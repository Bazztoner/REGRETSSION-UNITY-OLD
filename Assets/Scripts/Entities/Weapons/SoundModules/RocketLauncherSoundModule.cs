using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RocketLauncherSoundModule : WeaponSoundModuleBase
{
    public AudioClip reloadClip;

    public void OnReload()
    {
        PlaySound(reloadClip);
    }
}
