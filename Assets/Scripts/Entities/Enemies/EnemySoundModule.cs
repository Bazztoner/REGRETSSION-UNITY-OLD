using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public abstract class EnemySoundModule : MonoBehaviour
{
    public AudioClip enemyFound, attack, death;
    AudioSource _audioSrc;

    protected virtual void Awake()
    {
        _audioSrc = GetComponent<AudioSource>();
    }

    protected void PlaySound(AudioClip sound)
    {
        _audioSrc.PlayOneShot(sound);
    }

    public virtual void OnEnemyFound()
    {
        PlaySound(enemyFound);
    }

    public virtual void OnAttack()
    {
        PlaySound(attack);
    }

    public virtual void OnDeath()
    {
        _audioSrc.enabled = false;
        _audioSrc.enabled = true;
        PlaySound(death);
    }
}
