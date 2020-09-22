using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class M72LAWSoundModule : WeaponSoundModuleBase
{
	public AudioClip discard;

	public void OnDiscard()
    {
        PlaySound(discard);
    }
}
