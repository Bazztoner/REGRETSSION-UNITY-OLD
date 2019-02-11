using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CultistAnimModule : MonoBehaviour
{
    Animator _an;

    public List<string> IdleAnimation = new List<string>() { "idle1", "idle2", "idle3" };
    public List<string> DeathAnimations = new List<string>() { "die_backwards", "die_spin", "die_gutshot" };
    public List<string> PainAnimations = new List<string>() { "flinch1", "flinch2" };

    void Awake()
    {
        _an = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        var rndIndx = Random.Range(0, IdleAnimation.Count);
        _an.CrossFadeInFixedTime(IdleAnimation[rndIndx], .1f);
    }

    public void SetFlinch()
    {
        var rndIndx = Random.Range(0, PainAnimations.Count);
        _an.CrossFadeInFixedTime(PainAnimations[rndIndx], .1f);
    }

    public void SetWalk()
    {
        _an.CrossFadeInFixedTime("walk", .1f);
    }

    public void SetRun()
    {
        _an.CrossFadeInFixedTime("run", .1f);
    }

    public void SetAttack()
    {
        _an.CrossFadeInFixedTime("knife_throw", .1f);
    }

    public void SetBerserk()
    {
        _an.CrossFadeInFixedTime("berserk", .1f);
    }

    public void SetDeath(bool frontalHit)
    {
        if (!frontalHit) _an.CrossFadeInFixedTime("forward_die", .1f);
        else
        {
            var rndIndx = Random.Range(0, DeathAnimations.Count);
            _an.CrossFadeInFixedTime(DeathAnimations[rndIndx], .1f);
        }
    }
}
