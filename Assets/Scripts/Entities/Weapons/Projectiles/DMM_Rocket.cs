using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DMM_Rocket : MonoBehaviour
{
    public float speed;
    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
	{
        _rb.MovePosition(_rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Enemy>())
        {
            //overlapSphere
            col.GetComponent<Enemy>().TakeDamage(155);

            SpawnParticle();
            Destroy(gameObject);
        }
    }

    void SpawnParticle()
    {
        var partID = SimpleParticleSpawner.ParticleID.ROCKETHIT;
        var part = SimpleParticleSpawner.Instance.particles[partID];

        SimpleParticleSpawner.Instance.SpawnParticle(part, transform.position, Quaternion.identity);
    }
}
