using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pistol : EnemyWeaponRanged
{
    [SerializeField] float _shootDelay;
    [SerializeField] Transform _laserSight;
    Renderer _laserSightRenderer;

    protected override void Awake()
    {
        base.Awake();
        _laserSightRenderer = _laserSight.GetComponent<Renderer>();
        _laserSightRenderer.enabled = false;
    }

    public override void AttackStart()
    {
        _storedDir = (_target.transform.position - _muzzle.transform.position).normalized;

        _laserSight.parent = null;

        var distance = Vector3.Distance(_target.transform.position, _muzzle.transform.position);
        var promPos = Vector3.Lerp(_muzzle.transform.position, _target.transform.position, .5f);

        _laserSight.localScale = new Vector3(_laserSight.localScale.x, distance / 2, _laserSight.localScale.z);
        //_laserSight.parent = _muzzle.transform;
        _laserSight.position = promPos;
        _laserSight.up = _storedDir.normalized;

        _laserSightRenderer.enabled = true;

        Invoke("ManageProjectile", _shootDelay);
    }

    public override void AttackEnd()
    {
        _laserSightRenderer.enabled = false;
        //CancelInvoke();
    }

    void ManageProjectile()
    {
        _laserSightRenderer.enabled = false;
        var b = new EnemyHitscanBullet(_muzzle.transform.position, _storedDir.normalized, _damage);
        var particleID = SimpleParticleSpawner.ParticleID.BULLET;

        var particle = SimpleParticleSpawner.Instance.particles[particleID].GetComponentInChildren<ParticleSystem>();
        var speed = particle.main.startSpeed.constant * particle.main.simulationSpeed;
        var lifeTime = b.objDist / speed;
        SimpleParticleSpawner.Instance.SpawnParticle(particle.gameObject, _muzzle.transform.position, _storedDir.normalized, lifeTime);
    }
}
