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

            if (instance.ParticleData == null) instance.ParticleData = Resources.Load<SO_ParticleSpawnerList>("ScriptableObjects/DATA_ParticleSpawnerList");

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

    public SO_ParticleSpawnerList ParticleData;

    public GameObject GetParticleByID(ParticleIDs id)
    {
        if (ParticleData.allParticles == null) ParticleData.Init();

        var list = ParticleData.allParticles[id];
        var rnd = Random.Range(0, list.Length);
        return list[rnd].gameObject;
    }

    void Awake()
    {
        ParticleData = Resources.Load<SO_ParticleSpawnerList>("ScriptableObjects/DATA_ParticleSpawnerList");
        ParticleData.Init();
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

    public void SpawnParticle(ParticleIDs part, Vector3 pos, Vector3 dir, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(GetParticleByID(part), pos, Quaternion.identity, prnt) : GameObject.Instantiate(GetParticleByID(part), pos, Quaternion.identity);
        p.transform.forward = dir.normalized;
        GameObject.Destroy(p, 3);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", 3.3f);
    }

    public void SpawnParticle(ParticleIDs part, Vector3 pos, Vector3 dir, float lifeTime, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(GetParticleByID(part), pos, Quaternion.identity, prnt) : GameObject.Instantiate(GetParticleByID(part), pos, Quaternion.identity);
        p.transform.forward = dir.normalized;
        GameObject.Destroy(p, lifeTime);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", lifeTime + .3f);
    }

    public void SpawnParticle(ParticleIDs part, Vector3 pos, Quaternion dir, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(GetParticleByID(part), pos, dir, prnt) : GameObject.Instantiate(GetParticleByID(part), pos, dir);
        GameObject.Destroy(p, 3);
        ActiveParticles.Add(p);
        Invoke("CleanParticles", 3.3f);
    }

    public void SpawnParticle(ParticleIDs part, Vector3 pos, Quaternion dir, float lifeTime, Transform prnt = null)
    {
        var p = prnt ? GameObject.Instantiate(GetParticleByID(part), pos, dir, prnt) : GameObject.Instantiate(GetParticleByID(part), pos, dir);
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
