using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BrowningHPSoundModule : WeaponSoundModuleBase
{
    public AudioClip magIn, magOut, slideRelease;

    public virtual void OnMagIn()
    {
        PlaySound(magIn);
    }

    public virtual void OnMagOut()
    {
        PlaySound(magOut);
    }
    public virtual void OnSlideRelease()
    {
        PlaySound(slideRelease);
    }
}
