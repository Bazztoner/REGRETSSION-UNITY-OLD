using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleParticleSpawner : MonoBehaviour
{
    static SimpleParticleSpawner instance;
    public static SimpleParticleSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SimpleParticleSpawner>();
                if (instance == null)
                {
                    instance = new GameObject("new SimpleParticleSpawner Object").AddComponent<SimpleParticleSpawner>().GetComponent<SimpleParticleSpawner>();
                }
            }

            if (instance.allParticles == null) instance.allParticles = Resources.Load<SO_ParticleSpawnerList>("ScriptableObjects/DATA_ParticleSpawnerList");

            return instance;
        }
    }

    List<GameObject> _activeParts;
    List<GameObject> ActiveParticles
    {
        get
        {
            if (_activeParts == null) _activeParts = new List<GameObject>();
            return _activeParts;
        }

        set
        {
            _activeParts = value;
        }
    }

    public SO_ParticleSpawnerList allParticles;

    public struct ParticleID
    {
        public const int BULLET = 0;
        public const int MUZZLEFLASH = 1;
        public const int ROCKETHIT = 2;
        public const int PLASMABALLHIT = 3;
    }

    public GameObject GetParticleByID(int id)
    {
        return allParticles.particleList[id];
    }

    void Awake()
    {
        if (allParticles == null) allParticles = Resources.Load<SO_ParticleSpawnerList>("ScriptableObjects/DATA_ParticleSpawnerList");
    }

    public void SpawnParticle(GameObject part, Vector3 pos, Vector3 dir, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(part, pos, Quaternion.identity, prnt) : GameObject.Instantiate(part, pos, Quaternion.identity);
        p.transform.forward = dir.normalized;
        GameObject.Destroy(p, 3);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", 3.3f);
    }

    public void SpawnParticle(GameObject part, Vector3 pos, Vector3 dir, float lifeTime, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(part, pos, Quaternion.identity, prnt) : GameObject.Instantiate(part, pos, Quaternion.identity);
        p.transform.forward = dir.normalized;
        GameObject.Destroy(p, lifeTime);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", lifeTime + .3f);
    }

    public void SpawnParticle(GameObject part, Vector3 pos, Quaternion dir, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(part, pos, dir, prnt) : GameObject.Instantiate(part, pos, dir);
        GameObject.Destroy(p, 3);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", 3.3f);

    }

    public void SpawnParticle(GameObject part, Vector3 pos, Quaternion dir, float lifeTime, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(part, pos, dir, prnt) : GameObject.Instantiate(part, pos, dir);
        GameObject.Destroy(p, lifeTime);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", lifeTime + .3f);
    }

    public void SpawnParticle(int part, Vector3 pos, Vector3 dir, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(allParticles.particleList[part], pos, Quaternion.identity, prnt) : GameObject.Instantiate(allParticles.particleList[part], pos, Quaternion.identity);
        p.transform.forward = dir.normalized;
        GameObject.Destroy(p, 3);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", 3.3f);
    }

    public void SpawnParticle(int part, Vector3 pos, Vector3 dir, float lifeTime, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(allParticles.particleList[part], pos, Quaternion.identity, prnt) : GameObject.Instantiate(allParticles.particleList[part], pos, Quaternion.identity);
        p.transform.forward = dir.normalized;
        GameObject.Destroy(p, lifeTime);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", lifeTime + .3f);
    }

    public void SpawnParticle(int part, Vector3 pos, Quaternion dir, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(allParticles.particleList[part], pos, dir, prnt) : GameObject.Instantiate(allParticles.particleList[part], pos, dir);
        GameObject.Destroy(p, 3);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", 3.3f);
    }

    public void SpawnParticle(int part, Vector3 pos, Quaternion dir, float lifeTime, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(allParticles.particleList[part], pos, dir, prnt) : GameObject.Instantiate(allParticles.particleList[part], pos, dir);
        GameObject.Destroy(p, lifeTime);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", lifeTime + .3f);
    }

    void CleanParticles()
    {
        ActiveParticles = ActiveParticles.Where(x => x != null).ToList();
    }

    void ResetRound()
    {
        for (int i = 0; i < ActiveParticles.Count; i++)
        {
            Destroy(ActiveParticles[i]);
        }

        ActiveParticles = null;
    }
}
