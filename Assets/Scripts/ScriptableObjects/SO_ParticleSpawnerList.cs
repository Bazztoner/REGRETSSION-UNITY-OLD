using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "DATA_ParticleSpawnerList", menuName = "Scriptable Objects/Particles/ParticleSpawnerList")]
public class SO_ParticleSpawnerList : ScriptableObject
{
    [HideInInspector]
    public Dictionary<ParticleIDs, ParticleSystem[]> allParticles;

    public ParticleSystem[] bulletTracerGeneric;
    public ParticleSystem[] muzzleFlashGeneric;
    public ParticleSystem[] rocketHit;
    public ParticleSystem[] plasmaHit;

    public void Awake()
    {
        Init();
    }
    public void Init()
    {
        allParticles = new Dictionary<ParticleIDs, ParticleSystem[]>
        {
            [ParticleIDs.BULLET_TRACER_GENERIC] = bulletTracerGeneric,
            [ParticleIDs.MUZZLE_FLASH_GENERIC] = muzzleFlashGeneric,
            [ParticleIDs.ROCKET_HIT] = rocketHit,
            [ParticleIDs.PLASMA_HIT] = plasmaHit
        };
    }


}

public enum ParticleIDs
{
    BULLET_TRACER_GENERIC,
    MUZZLE_FLASH_GENERIC,
    ROCKET_HIT,
    PLASMA_HIT,
    Count
}