using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pistol : EnemyWeaponRanged
{
    [SerializeField] float _shootDelay;

    public override void AttackStart()
    {
        _storedDir = (_target.transform.position - _muzzle.transform.position).normalized;
        Invoke("ManageProjectile", _shootDelay);
    }

    public override void AttackEnd()
    {
        //CancelInvoke();
    }

    void ManageProjectile()
    {
        var b = new EnemyHitscanBullet(_muzzle.transform.position, _storedDir.normalized, _damage);
        var particleID = SimpleParticleSpawner.ParticleID.BULLET;

        var particle = SimpleParticleSpawner.Instance.particles[particleID].GetComponentInChildren<ParticleSystem>();
        var speed = particle.main.startSpeed.constant * particle.main.simulationSpeed;
        var lifeTime = b.objDist / speed;
        SimpleParticleSpawner.Instance.SpawnParticle(particle.gameObject, _muzzle.transform.position, _storedDir.normalized, lifeTime);
    }
}
