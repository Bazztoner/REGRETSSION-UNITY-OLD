using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pistol : EnemyWeaponRanged
{
    [SerializeField] float _shootDelay;
    bool _attacking;
    LineRenderer _line;

    protected override void Awake()
    {
        base.Awake();
        _line = GetComponentInChildren<LineRenderer>();
        _line.enabled = false;
    }

    void Update()
    {
        if (_attacking)
        {
            _line.SetPosition(0, _muzzle.transform.position);
        }
    }

    public override void AttackStart()
    {
        _attacking = true;
        _storedDir = (_target.transform.position - _muzzle.transform.position).normalized;

        _line.enabled = true;
        _line.SetPosition(1, _target.transform.position);

        Invoke("ManageProjectile", _shootDelay);
    }

    public override void AttackEnd()
    {
        _line.enabled = false;
        _attacking = false;
    }

    void ManageProjectile()
    {
        _line.enabled = false;
        _attacking = false;

        var b = new EnemyHitscanBullet(_muzzle.transform.position, _storedDir.normalized, _damage);
        var particleID = SimpleParticleSpawner.ParticleID.BULLET;

        var particle = SimpleParticleSpawner.Instance.GetParticleByID(particleID).GetComponentInChildren<ParticleSystem>();
        var speed = particle.main.startSpeed.constant * particle.main.simulationSpeed;
        var lifeTime = b.objDist / speed;
        SimpleParticleSpawner.Instance.SpawnParticle(particle.gameObject, _muzzle.transform.position, _storedDir.normalized, lifeTime);
    }
}
