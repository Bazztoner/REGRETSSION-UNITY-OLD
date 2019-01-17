using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_SuperShotgun : WeaponBase
{
    public float dispersionConeRadius;
    int _bullets = Animator.StringToHash("bullets");

    protected override void OnEnable()
    {
        _an.SetInteger(_bullets, _ammo);
        base.OnEnable();
    }

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot()) Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (_canReload()) Reload();
        }
    }

    protected override int GetAmmo()
    {
        return _ammo <= 0 ? 0 : _ammo;
    }

    protected override void SetAmmo(int ammo)
    {
        _ammo = ammo <= 0 ? 0 : ammo >= maxAmmo ? maxAmmo : ammo;
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && GetAmmo() > 0 && _drawn;
        _canReload = () => !_shooting && !_reloading && !_holstering && GetAmmo() < maxAmmo && _drawn;
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        UpdateAmmo(-1);
        _shooting = true;

        _an.CrossFadeInFixedTime("shoot", .1f);

        yield return new WaitForEndOfFrame();

        ManageBullet();

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        _shooting = false;
    }

    protected override void ManageBullet()
    {
        for (int i = 0; i < pellets; i++)
        {
            var dispersionFactor = 1 / dispersionConeRadius;

            float xSpread = Random.Range(-dispersionFactor, dispersionFactor);
            float ySpread = Random.Range(-(dispersionFactor/2), dispersionFactor/2);

            var dirToCrosshair = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;

            var dir = (dirToCrosshair + new Vector3(xSpread, ySpread, 0));

            var b = new HitscanBullet(_muzzle.transform.position, dir.normalized, damage, pellets);

            var bulletParticleID = SimpleParticleSpawner.ParticleID.BULLET;

            var bulletParticle = SimpleParticleSpawner.Instance.particles[bulletParticleID].GetComponentInChildren<ParticleSystem>();
            var speed = bulletParticle.main.startSpeed.constant * bulletParticle.main.simulationSpeed;
            var lifeTime = b.objDist / speed;

            SimpleParticleSpawner.Instance.SpawnParticle(bulletParticle.gameObject, _muzzle.transform.position, dir.normalized, lifeTime);

        }

        var muzzleFlashID = SimpleParticleSpawner.ParticleID.MUZZLEFLASH;
        var muzzleFlashParticle = SimpleParticleSpawner.Instance.particles[muzzleFlashID].GetComponentInChildren<ParticleSystem>();

        var muzzleDir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        muzzleDir.Normalize();

        SimpleParticleSpawner.Instance.SpawnParticle(muzzleFlashParticle.gameObject, _muzzle.transform.position, muzzleDir.normalized, _muzzle.transform);
    }
}
