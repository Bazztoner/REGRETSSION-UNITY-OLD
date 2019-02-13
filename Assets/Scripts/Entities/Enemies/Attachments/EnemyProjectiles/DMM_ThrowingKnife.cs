using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DMM_ThrowingKnife : MonoBehaviour
{
    public float speed;
    int _damage;
    Rigidbody _rb;
    Vector3 _dir;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Configure(Vector3 origin, Vector3 dir, int damage)
    {
        transform.position = origin;
        _dir = dir.normalized;
        transform.forward = _dir;
        _damage = damage;
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.LayerDifferentFrom("Unrenderizable", "Ignore Raycast"))
        {
            if (col.GetComponent(typeof(IDamageable)) is IDamageable dmgeable && col.gameObject.LayerDifferentFrom("Enemy"))
            {
                dmgeable.TakeDamage(_damage, DamageTypes.Bullet);
            }
            Destroy(gameObject);
        }
    }
}
