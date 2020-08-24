using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DMM_Rocket : MonoBehaviour
{
    public float speed, radius;
    public int hitDamage, splashDamage;
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
        if (col.gameObject.LayerDifferentFrom(LayerMask.NameToLayer("Unrenderizable"), LayerMask.NameToLayer("Ignore Raycast"), LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Pickable")))
        {
            if (col.GetComponent(typeof(IDamageable)) is IDamageable dmgeable)
            {
                dmgeable.TakeDamage(hitDamage, DamageTypes.Explosive);
            }

            var overlapped = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy", "Player", "Destructible")).Select(x => x.GetComponent(typeof(IDamageable)) as IDamageable).Where(x => x!=null).ToArray();

            foreach (var item in overlapped)
            {
                item.TakeDamage(splashDamage, DamageTypes.Explosive);
            }

            SpawnParticle();
            Destroy(gameObject);
        }
    }

    void SpawnParticle()
    {
        var partID = SimpleParticleSpawner.ParticleID.ROCKETHIT;
        var part = SimpleParticleSpawner.Instance.GetParticleByID(partID);

        SimpleParticleSpawner.Instance.SpawnParticle(part, transform.position, Quaternion.identity);
    }
}
