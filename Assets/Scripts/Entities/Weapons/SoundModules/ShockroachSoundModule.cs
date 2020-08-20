using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShockroachSoundModule : WeaponSoundModuleBase
{
    public AudioClip shootLoop;

    public void OnStartShoot()
    {
        PlaySound(shootLoop);
    }
}
