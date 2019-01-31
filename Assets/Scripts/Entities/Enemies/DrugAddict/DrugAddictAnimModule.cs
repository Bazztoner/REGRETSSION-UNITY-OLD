using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrugAddictAnimModule : MonoBehaviour
{
    Animator _an;

    public List<string> AttackAnimations = new List<string>() { "knife1", "knife2", "knife3" };
    public List<string> DeathAnimations = new List<string>() { "normal_die1", "normal_die2", "normal_die3" };
    public List<string> PainAnimations = new List<string>() { "flinch1", "flinch2" };

    void Awake()
    {
        _an = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        _an.CrossFadeInFixedTime("idle", .1f);
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
        var rndIndx = Random.Range(0, AttackAnimations.Count);
        _an.CrossFadeInFixedTime(AttackAnimations[rndIndx], .1f);
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
