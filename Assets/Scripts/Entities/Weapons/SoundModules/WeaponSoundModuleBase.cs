using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class WeaponSoundModuleBase : MonoBehaviour
{
    public AudioClip[] shootClips;
    public AudioClip draw;
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
        if (shootClips.Length <= 0) return;

        var rndShoot = Random.Range(0, shootClips.Length);
        PlaySound(shootClips[rndShoot]);
    }

    public virtual void OnDraw()
    {
        PlaySound(draw);
    }

    public virtual void ForceStop()
    {
        _src.Stop();
    }
}
