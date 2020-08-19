using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RailCannonSoundModule : WeaponSoundModuleBase
{
    public AudioClip clipIn, clipOut;
    public void OnClipOut()
    {
        PlaySound(clipOut);
    }
    public void OnClipIn()
    {
        PlaySound(clipIn);
    }

    
}
