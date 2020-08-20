using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DisplacerSoundModule : WeaponSoundModuleBase
{
    public AudioClip chargeClip;

    public void OnCharge()
    {
        PlaySound(chargeClip);
    }
}
