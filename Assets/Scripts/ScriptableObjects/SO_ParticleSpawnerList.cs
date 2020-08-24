using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "DATA_ParticleSpawnerList", menuName = "Scriptable Objects/Particles/ParticleSpawnerList")]
public class SO_ParticleSpawnerList : ScriptableObject
{
    public GameObject[] particleList;
}
