using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HS10SoundModule : WeaponSoundModuleBase
{
    public AudioClip boltCatch;
    public AudioClip[] insertShell;

    public void OnBoltCatch()
    {
        PlaySound(boltCatch);
    }

    public void OnInsertShell()
    {
        PlaySound(insertShell[Random.Range(0, insertShell.Length)]);
    }
}
