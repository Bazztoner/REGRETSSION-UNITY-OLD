using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MafiaAnimModule : MonoBehaviour
{
    Animator _an;

    public List<string> DeathAnimations = new List<string>() { "normal_die1", "normal_die2", "normal_die3" };

    void Awake()
    {
        _an = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        _an.CrossFadeInFixedTime("idle", .1f);
    }

    public void SetDuck()
    {
        _an.CrossFadeInFixedTime("goduck", .1f);
    }

    public void SetRoll(bool left)
    {
        var sufix = left ? "_left" : "_right";

        _an.CrossFadeInFixedTime("roll" + sufix, .1f);
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
        _an.CrossFadeInFixedTime("shoot", .1f);
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
