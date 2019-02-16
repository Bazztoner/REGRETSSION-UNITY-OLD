using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class WeaponSoundModuleBase : MonoBehaviour
{
    public AudioClip shoot, draw;
    AudioSource _src;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
    }

    protected void PlaySound(AudioClip snd)
    {
        if (_src == null) _src = GetComponent<AudioSource>();

        if (snd != null) _src.PlayOneShot(snd);
        else Debug.Log("Sound not found");
        
    }

    public virtual void OnShoot()
    {
        PlaySound(shoot);
    }

    public virtual void OnDraw()
    {
        PlaySound(draw);
    }
}
