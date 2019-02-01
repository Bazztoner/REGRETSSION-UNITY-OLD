using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DMM_Rocket : MonoBehaviour
{
    public float speed;
    public int damage;
    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        var dmgeable = col.GetComponent(typeof(IDamageable)) as IDamageable;
        if (col.gameObject.LayerDifferentFrom(LayerMask.NameToLayer("Unrenderizable"), LayerMask.NameToLayer("Ignore Raycast"), LayerMask.NameToLayer("Player")))
        {
            if (dmgeable != null)
            {
                //overlapSphere
                dmgeable.TakeDamage(damage);
            }

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
